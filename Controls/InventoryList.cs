using System;
using System.Linq;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Rendering;

namespace RheinwerkAdventure.Controls
{
    /// <summary>
    /// Listencontrol zur Auflistung des Inventars
    /// </summary>
    internal class InventoryList : VerticalListControl<InventoryItem>
    {
        public InventoryList(ScreenComponent manager) 
            : base(manager)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            int x = offset.X + Position.X;
            int y = offset.Y + Position.Y;
            foreach (var item in Items.Where(i => i.Visible))
            {
                // Hintergrund rendern
                Manager.Button.Draw(spriteBatch, new Rectangle(x, y, Position.Width, 50));

                // Icon rendern
                if (!string.IsNullOrEmpty(item.Icon))
                {
                    Texture2D icon = Manager.GetIcon(item.Icon);
                    int margin = (50 - 24) / 2;
                    spriteBatch.Draw(icon, new Rectangle(x + margin, y + margin, 24, 24), Color.White);
                }

                // Text-Inhalt zentriert rendern
                string text = item.ToString();
                if (item.Count > 1)
                    text += string.Format(" ({0})", item.Count);
                
                Vector2 size = Manager.Font.MeasureString(text);
                spriteBatch.DrawString(Manager.Font, text, new Vector2(x + 50, y + ((50f - size.Y) / 2f)), Color.White);
                y += 55;
            }
        }
    }

    /// <summary>
    /// Inventar-Item
    /// </summary>
    internal class InventoryItem : ListItem
    {
        /// <summary>
        /// Icon-Schlüssel
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Anzahl enthaltener Items des selben Typs
        /// </summary>
        public int Count { get; set; }

        public InventoryItem()
        {
            // Standardmäßig deaktiviert, damit keine Auswahl passiert.
            Enabled = false;
        }
    }
}

