using System;
using System.Linq;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Rendering;

namespace RheinwerkAdventure.Controls
{
    /// <summary>
    /// Vertikales Menü-Control
    /// </summary>
    internal class MenuList : VerticalListControl<ListItem>
    {
        public MenuList(ScreenComponent manager) 
            : base(manager)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            int x = offset.X + Position.X;
            int y = offset.Y + Position.Y;
            foreach (var item in Items.Where(i => i.Visible))
            {
                // Style des aktuellen Elementes ermitteln
                NineTileRenderer renderer = Manager.Button;
                float alpha = (item.Enabled ? 1f : 0.3f);
                if (item.Equals(SelectedItem)) renderer = Manager.ButtonHovered;

                // Hintergrund rendern
                renderer.Draw(spriteBatch, new Rectangle(x, y, Position.Width, 50), alpha);

                // Text-Inhalt zentriert rendern
                Vector2 size = Manager.Font.MeasureString(item.ToString());
                spriteBatch.DrawString(Manager.Font, item.ToString(), new Vector2(x + ((Position.Width - size.X) / 2f), y + ((50f - size.Y) / 2f)), Color.White * alpha);
                y += 55;
            }
        }
    }
}

