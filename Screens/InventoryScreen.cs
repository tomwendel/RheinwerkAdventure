using System;
using System.Linq;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Controls;

namespace RheinwerkAdventure.Screens
{
    internal class InventoryScreen : Screen
    {
        public InventoryScreen(ScreenComponent manager) 
            : base(manager, new Point(400, 300))
        {
            Controls.Add(new Panel(manager) { Position = new Rectangle(20, 20, 360, 40) });
            Controls.Add(new Label(manager) { Text = "Rucksack", Position = new Rectangle(40, 30, 0, 0) });

            InventoryList list = new InventoryList(manager) { Position = new Rectangle(20, 70, 360, 200) };
            foreach (var itemGroup in manager.Game.Local.Player.Inventory.GroupBy(i => i.GetType()))
            {
                list.Items.Add(new InventoryItem() { 
                    Text = itemGroup.First().Name, 
                    Icon = itemGroup.First().Icon, 
                    Count = itemGroup.Count() });
            }
            Controls.Add(list);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Manager.Game.Input.Handled)
            {
                if (Manager.Game.Input.Close || Manager.Game.Input.Inventory)
                {
                    Manager.CloseScreen();
                    Manager.Game.Input.Handled = true;
                }
            }
        }
    }
}

