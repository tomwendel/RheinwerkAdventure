using System;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Controls
{
    /// <summary>
    /// Rahmencontrol zur Abgrenzung von Control-Gruppen
    /// </summary>
    internal class Panel : Control
    {
        public Panel(ScreenComponent manager)
            : base(manager)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            Manager.Border.Draw(spriteBatch, 
                new Rectangle(
                    Position.X + offset.X, 
                    Position.Y + offset.Y, 
                    Position.Width, 
                    Position.Height));
        }
    }
}

