using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RheinwerkAdventure.Components
{
    internal class SceneComponent : DrawableGameComponent
    {
        private readonly RheinwerkGame game;

        private SpriteBatch spriteBatch;

        private Texture2D pixel;

        public SceneComponent (RheinwerkGame game) : base(game)
        {
            this.game = game;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new [] { Color.White });
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
        }
    }
}

