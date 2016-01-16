using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RheinwerkAdventure.Model;
using System.IO;

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

        private Texture2D landscape;

        public SceneComponent (RheinwerkGame game) : base(game)
        {
            this.game = game;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Hilfspixel erstellen
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new [] { Color.White });

            // Landscape-Texture direkt aus dem Stream laden
            string mapPath = Path.Combine(Environment.CurrentDirectory, "Maps");
            using (Stream stream = File.OpenRead(mapPath + "\\landscape.png"))
            {
                landscape = Texture2D.FromStream(GraphicsDevice, stream);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Bildschirm leeren
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Erste Area referenzieren (versuchsweise)
            Area area = game.Simulation.World.Areas[0];

            // Skalierungsfaktor für eine Vollbild-Darstellung der Area ausrechnen
            float scaleX = (GraphicsDevice.Viewport.Width - 20) / area.Width;
            float scaleY = (GraphicsDevice.Viewport.Height - 20) / area.Height;

            spriteBatch.Begin();

            // Ausgabe der Spielfeld-Zellen
            for (int x = 0; x < area.Width; x++)
            {
                for (int y = 0; y < area.Height; y++)
                {
                    // Ermitteln, ob diese Zelle blockiert
                    bool blocked = false;
                    for (int l = 0; l < area.Layers.Length; l++)
                        blocked |= area.Layers[l].Tiles[x, y].Blocked;

                    int offsetX = (int)(x * scaleX) + 10;
                    int offsetY = (int)(y * scaleY) + 10;

                    // Zelle mit der Standard-Textur (Gras) ausmalen
                    spriteBatch.Draw(landscape, new Rectangle(offsetX, offsetY, (int)scaleX, (int)scaleY), new Rectangle(448, 128, 32, 32), Color.White);
                }
            }

            // Ausgabe der Spielfeld-Items
            foreach (var item in area.Items)
            {
                // Ermittlung der Item-Farbe.
                Color color = Color.Yellow;
                if (item is Player)
                    color = Color.Red;

                // Positionsermittlung und Ausgabe des Spielelements.
                int posX = (int)((item.Position.X - item.Radius) * scaleX) + 10;
                int posY = (int)((item.Position.Y - item.Radius) * scaleY) + 10;
                int size = (int)((item.Radius * 2) * scaleX);
                spriteBatch.Draw(pixel, new Rectangle(posX, posY, size, size), color);
            }

            spriteBatch.End();
        }
    }
}

