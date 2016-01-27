using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Rendering;
using RheinwerkAdventure.Components;

namespace RheinwerkAdventure.Controls
{
    /// <summary>
    /// Auflistung von Buttons auf die volle Breite
    /// </summary>
    internal class DialogButtons : HorizontalListControl<ListItem>
    {
        public DialogButtons(ScreenComponent manager) 
            : base(manager)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            var items = Items.Where(i => i.Visible).ToArray();

            int x = offset.X + Position.X;
            int y = offset.Y + Position.Y;
            int width = 0;
            if (items.Length > 0)
                width = (Position.Width - ((items.Length - 1) * 5)) / items.Length;

            foreach (var item in items)
            {
                // Style des aktuellen Elementes ermitteln
                NineTileRenderer renderer = Manager.Button;
                float alpha = (item.Enabled ? 1f : 0.3f);
                if (item.Equals(SelectedItem)) renderer = Manager.ButtonHovered;

                // Hintergrund rendern
                renderer.Draw(spriteBatch, new Rectangle(x, y, width, Position.Height), alpha);

                // Text-Inhalt zentriert rendern
                Vector2 size = Manager.Font.MeasureString(item.ToString());
                spriteBatch.DrawString(Manager.Font, item.ToString(), new Vector2(x + ((width - size.X) / 2f), y + ((Position.Height - size.Y) / 2f)), Color.White * alpha);
                x += width + 5;
            }
        }
    }
}

