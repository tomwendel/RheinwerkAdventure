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

        private Dictionary<Item, ItemRenderer> itemRenderer;

        private Texture2D coin;

        /// <summary>
        /// Kamera-Einstellungen für diese Szene.
        /// </summary>
        public Camera Camera { get; private set; }

        public SceneComponent(RheinwerkGame game)
            : base(game)
        {
            this.game = game;
            textures = new Dictionary<string, Texture2D>();
            itemRenderer = new Dictionary<Item, ItemRenderer>();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Camera = new Camera(GraphicsDevice.Viewport.Bounds.Size);

            // Initiale Kameraposition (temporär)
            Vector2 areaSize = new Vector2(
                game.Simulation.World.Areas[0].Width,
                game.Simulation.World.Areas[0].Height);
            Camera.SetFocusExplizit(game.Simulation.Player.Position, areaSize);

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

            using (Stream stream = File.OpenRead(Path.Combine(Environment.CurrentDirectory, "Content") + "\\" + "coin_silver.png"))
            {
                coin = Texture2D.FromStream(GraphicsDevice, stream);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Platziert den Kamerafokus auf den Spieler.
            Vector2 areaSize = new Vector2(
                game.Simulation.World.Areas[0].Width,
                game.Simulation.World.Areas[0].Height);
            Camera.SetFocus(game.Simulation.Player.Position, areaSize);
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
                    RenderItems(area, offset, gameTime);
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
        private void RenderItems(Area area, Point offset, GameTime gameTime)
        {
            // Item Renderer für alle Items erzeugen
            foreach (var item in area.Items)
                if (!itemRenderer.ContainsKey(item))
                    itemRenderer.Add(item, new ItemRenderer(item, Camera, coin, new Point(32, 32), 70, 8, new Point(16,26), 1f));

            foreach (var renderer in itemRenderer.Values)
            {
                renderer.Draw(spriteBatch, offset, gameTime);
            }
        }
    }
}

