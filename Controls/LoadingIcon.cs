using System;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Controls
{
    internal class LoadingIcon : Control
    {
        int frame = 0;

        Point size = new Point(32, 24);

        public LoadingIcon(ScreenComponent manager) 
            : base(manager)
        {
        }

        public override void Update(GameTime gameTime)
        {
            frame = (int)gameTime.TotalGameTime.TotalMilliseconds / 80;
            frame %= 8;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            var source = new Rectangle(frame * size.X, 0, size.X, size.Y);
            var destination = new Rectangle(offset.X + Position.X, offset.Y + Position.Y, Position.Width, Position.Height);
            spriteBatch.Draw(Manager.WaitingCoin, destination, source, Color.White);
        }
    }
}

