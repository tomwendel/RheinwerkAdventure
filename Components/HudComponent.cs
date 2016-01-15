using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public HudComponent(RheinwerkGame game) : base(game)
        {
            this.game = game;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            hudFont = Game.Content.Load<SpriteFont>("HudFont");
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            // Ausgabe der ersten Debug-Info
            spriteBatch.DrawString(hudFont, "Development Version", new Vector2(20, 20), Color.White);

            spriteBatch.End();
        }
    }
}

