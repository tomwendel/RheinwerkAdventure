using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RheinwerkAdventure.Model;
using System.IO;
using System.Collections.Generic;
using RheinwerkAdventure.Rendering;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Game Komponente zur Ausgabe der Spiel-Szene an den Bildschirm.
    /// </summary>
    internal class SceneComponent : DrawableGameComponent
    {
        private readonly RheinwerkGame game;

        private SpriteBatch spriteBatch;

        private Texture2D pixel;

        private Dictionary<string, Texture2D> textures;

        /// <summary>
        /// Kamera-Einstellungen für diese Szene.
        /// </summary>
        public Camera Camera { get; private set; }

        public SceneComponent(RheinwerkGame game)
            : base(game)
        {
            this.game = game;
            textures = new Dictionary<string, Texture2D>();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Camera = new Camera(GraphicsDevice.Viewport.Bounds.Size);

            // Hilfspixel erstellen
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new [] { Color.White });

            // Erforderliche Texturen ermitteln
            List<string> requiredTextures = new List<string>();
            foreach (var area in game.Simulation.World.Areas)
                foreach (var tile in area.Tiles.Values)
                    if (!requiredTextures.Contains(tile.Texture))
                        requiredTextures.Add(tile.Texture);

            // Erforderlichen Texturen direkt aus dem Stream laden
            string mapPath = Path.Combine(Environment.CurrentDirectory, "Maps");
            foreach (var textureName in requiredTextures)
            {
                using (Stream stream = File.OpenRead(mapPath + "\\" + textureName))
                {
                    Texture2D texture = Texture2D.FromStream(GraphicsDevice, stream);
                    textures.Add(textureName, texture);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Platziert den Kamerafokus auf den Spieler.
            Camera.SetFocus(game.Simulation.Player.Position);
        }

        public override void Draw(GameTime gameTime)
        {
            // Erste Area referenzieren (versuchsweise)
            Area area = game.Simulation.World.Areas[0];

            // Bildschirm leeren
            GraphicsDevice.Clear(area.Background);

            spriteBatch.Begin();

            // Berechnet den Render-Offset mit Hilfe der Kamera-Einstellungen
            Point offset = (Camera.Offset * Camera.Scale).ToPoint();

            // Alle Layer der Render-Reihenfolge nach durchlaufen
            for (int l = 0; l < area.Layers.Length; l++)
            {
                RenderLayer(area, area.Layers[l], offset);
                if (l == 4)
                    RenderItems(area, offset);
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Rendert einen Layer der aktuellen Szene
        /// </summary>
        private void RenderLayer(Area area, Layer layer, Point offset)
        {
            for (int x = 0; x < area.Width; x++)
            {
                for (int y = 0; y < area.Height; y++)
                {
                    // Prüfen, ob diese Zelle ein Tile enthält
                    int tileId = layer.Tiles[x, y];
                    if (tileId == 0)
                        continue;

                    // Tile ermitteln
                    Tile tile = area.Tiles[tileId];
                    Texture2D texture = textures[tile.Texture];

                    // Position ermitteln
                    int offsetX = (int)(x * Camera.Scale) - offset.X;
                    int offsetY = (int)(y * Camera.Scale) - offset.Y;

                    // Zelle mit der Standard-Textur (Gras) ausmalen
                    spriteBatch.Draw(texture, new Rectangle(offsetX, offsetY, (int)Camera.Scale, (int)Camera.Scale), tile.SourceRectangle, Color.White);
                }
            }
        }

        /// <summary>
        /// Rendert die Spielelemente der aktuellen Szene
        /// </summary>
        private void RenderItems(Area area, Point offset)
        {
            // Ausgabe der Spielfeld-Items
            foreach (var item in area.Items)
            {
                // Ermittlung der Item-Farbe.
                Color color = Color.Yellow;
                if (item is Player)
                    color = Color.Red;

                // Positionsermittlung und Ausgabe des Spielelements.
                int posX = (int)((item.Position.X - item.Radius) * Camera.Scale) - offset.X;
                int posY = (int)((item.Position.Y - item.Radius) * Camera.Scale) - offset.Y;
                int size = (int)((item.Radius * 2) * Camera.Scale);
                spriteBatch.Draw(pixel, new Rectangle(posX, posY, size, size), color);
            }
        }
    }
}

