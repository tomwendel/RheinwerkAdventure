using System;
using System.Linq;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Controls
{
    /// <summary>
    /// List-Control für Dialog-Auswahl.
    /// </summary>
    internal class DialogList : VerticalListControl<ListItem>
    {
        public DialogList(ScreenComponent manager) 
            : base(manager) { }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            int x = offset.X + Position.X;
            int y = offset.Y + Position.Y;
            foreach (var item in Items.Where(i => i.Visible))
            {
                // Arrow rendern
                if (item == SelectedItem)
                {
                    Texture2D arrow = Manager.Arrow;
                    spriteBatch.Draw(arrow, new Vector2(x, y), Color.White);
                }

                // Text rendern
                spriteBatch.DrawString(Manager.Font, item.ToString(), new Vector2(x + 30, y), Color.White);
                y += 30;
            }
        }
    }
}

