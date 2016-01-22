using System;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Controls
{
    internal class Icon : Control
    {
        public string Texture { get; set; }

        public Icon(ScreenComponent manager)
            : base(manager)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            if (!string.IsNullOrEmpty(Texture))
            {
                Texture2D texture = Manager.Icons[Texture];
                spriteBatch.Draw(texture, new Rectangle(offset.X + Position.X, offset.Y + Position.Y, Position.Width, Position.Height), Color.White);
            }
        }
    }
}

