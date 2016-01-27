using System;
using System.Linq;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Rendering;

namespace RheinwerkAdventure.Controls
{
    /// <summary>
    /// Auflistung von kaufbaren Items für den Handelsdialog
    /// </summary>
    internal class TraderList : HorizontalListControl<TradingItem>
    {
        public TraderList(ScreenComponent manager)
            : base(manager)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            int x = offset.X + Position.X;
            int y = offset.Y + Position.Y;
            var items = Items.Where(i => i.Visible);
            int width = 120;
            x += (Position.Width - (width * items.Count())) / 2;

            foreach (var item in items)
            {
                // Style des aktuellen Elementes ermitteln
                NineTileRenderer renderer = Manager.Button;
                float alpha = (item.Enabled ? 1f : 0.3f);
                if (item.Equals(SelectedItem)) renderer = Manager.ButtonHovered;

                // Hintergrund rendern
                renderer.Draw(spriteBatch, new Rectangle(x, y, width, Position.Height), alpha);

                // Icon rendern
                if (!string.IsNullOrEmpty(item.Icon))
                {
                    Texture2D icon = Manager.GetIcon(item.Icon);
                    int margin = (width - icon.Width * 2) / 2;
                    spriteBatch.Draw(icon, new Rectangle(x + margin, y + 20, 42, 42), Color.White * alpha);
                }

                // Text-Inhalt zentriert rendern
                string text = item.ToString();
                Vector2 size = Manager.Font.MeasureString(text);
                spriteBatch.DrawString(Manager.Font, text, new Vector2(x + ((width - size.X) / 2), y + 100f), Color.White * alpha);

                // Preis
                text = item.Value.ToString();
                size = Manager.Font.MeasureString(text);
                spriteBatch.DrawString(Manager.Font, text, new Vector2(x + ((width - size.X) / 2), y + 120f), Color.White * alpha);

                x += width;
            }
        }
    }

    /// <summary>
    /// Erweitertes List Item mit den Handelsspezifischen Daten.
    /// </summary>
    internal class TradingItem : ListItem
    {
        /// <summary>
        /// ID der Icon-Textur zu diesem Item.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Handelswert in Münzen.
        /// </summary>
        public int Value  { get; set; }
    }
}

