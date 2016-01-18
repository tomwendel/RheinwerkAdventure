using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RheinwerkAdventure.Model;
using System.IO;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Game Komponente für die Anzeige des Headup Displays (Lebensanzeige,...)
    /// </summary>
    internal class HudComponent : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;

        private RheinwerkGame game;

        private SpriteFont hudFont;

        private Texture2D hearts;

        public HudComponent(RheinwerkGame game) : base(game)
        {
            this.game = game;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            hudFont = Game.Content.Load<SpriteFont>("HudFont");

            string mapPath = Path.Combine(Environment.CurrentDirectory, "Content");
            using (Stream stream = File.OpenRead(mapPath + "\\hearts.png"))
            {
                hearts = Texture2D.FromStream(GraphicsDevice, stream);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            Area area = game.Simulation.World.Areas[0];
            Vector2 position = game.Simulation.Player.Position;

            string debugText = string.Format("{0} ({1:0}/{2:0})", area.Name, position.X, position.Y);

            // Ausgabe der ersten Debug-Info
            spriteBatch.DrawString(hudFont, debugText, new Vector2(10, 10), Color.White);

            int totalHearts = game.Simulation.Player.MaxHitpoints;
            int filledHearts = game.Simulation.Player.Hitpoints;

            int offset = GraphicsDevice.Viewport.Width - (totalHearts * 34) - 10;

            for (int i = 0; i < totalHearts; i++)
            {
                Rectangle source = new Rectangle(0, (filledHearts > i ? 0 : 67), 32, 32);
                Rectangle destination = new Rectangle(offset + (i * 34), 10, 32, 32);

                spriteBatch.Draw(hearts, destination, source, Color.White);
            }

            spriteBatch.End();
        }
    }
}

