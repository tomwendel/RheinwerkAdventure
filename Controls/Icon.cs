using System;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Controls
{
    /// <summary>
    /// Icon Control
    /// </summary>
    internal class Icon : Control
    {
        /// <summary>
        /// Die zu anzeigende Textur.
        /// </summary>
        public string Texture { get; set; }

        public Icon(ScreenComponent manager)
            : base(manager)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            if (!string.IsNullOrEmpty(Texture))
            {
                Texture2D texture = Manager.GetIcon(Texture);
                spriteBatch.Draw(texture, new Rectangle(offset.X + Position.X, offset.Y + Position.Y, Position.Width, Position.Height), Color.White);
            }
        }
    }
}

