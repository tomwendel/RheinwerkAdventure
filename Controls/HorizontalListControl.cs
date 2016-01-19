using System;
using System.Linq;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Controls
{
    /// <summary>
    /// Horizontale Spezialisierung des ListControls.
    /// </summary>
    internal abstract class HorizontalListControl<T> : ListControl<T> where T : ListItem
    {
        public HorizontalListControl(ScreenComponent manager) 
            : base(manager)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // Alle potentiell selektierbaren Elemente ermitteln
            var availableItems = Items.Where(i => i.Enabled && i.Visible).ToList();

            // Links-Klick verarbeiten
            if (Manager.Game.Input.Left)
            {
                // Wenn nichts selektiert ist wird der letzte Eintrag aus der Liste markiert.
                if (SelectedItem == null)
                {
                    SelectedItem = availableItems.LastOrDefault();
                }
                else
                {
                    // Ermittlung des Index des aktuellen Elementes
                    int index = availableItems.IndexOf(SelectedItem);
                    index = Math.Max(0, index - 1);
                    SelectedItem = availableItems[index];
                }
                Manager.Game.Input.Handled = true;
            }

            // Rechts-Klick verarbeiten
            if (Manager.Game.Input.Right)
            {
                // Wenn nichts selektiert ist wird der erste Eintrag aus der Liste markiert.
                if (SelectedItem == null)
                {
                    SelectedItem = availableItems.FirstOrDefault();
                }
                else
                {
                    // Ermittlung des Index des aktuellen Elementes
                    int index = availableItems.IndexOf(SelectedItem);
                    index = Math.Min(availableItems.Count - 1, index + 1);
                    SelectedItem = availableItems[index];
                }
                Manager.Game.Input.Handled = true;
            }

            base.Update(gameTime);
        }
    }
}

