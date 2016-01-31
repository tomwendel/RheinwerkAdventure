using System;
using System.Linq;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Controls;
using RheinwerkAdventure.Model;

namespace RheinwerkAdventure.Screens
{
    internal class TradeScreen : Screen
    {
        TraderList list;

        IInventory trader;

        IInventory player;

        public TradeScreen(ScreenComponent manager, IInventory trader, IInventory player)
            : base(manager, new Point(400, 300))
        {
            this.trader = trader;
            this.player = player;

            Controls.Add(new Panel(manager) { Position = new Rectangle(20, 20, 360, 40) });
            Controls.Add(new Label(manager) { Text = "Shop", Position = new Rectangle(40, 30, 0, 0) });

            // Verkaufbare Items des Trader-Inventars auf die Liste setzen
            list = new TraderList(manager) { Position = new Rectangle(20, 70, 360, 210) };
            list.OnInteract += OnInteract;
            foreach (var item in trader.Inventory.Where(i => i.Value.HasValue).OrderBy(i => i.Value))
                list.Items.Add(new TradingItem()
                    { 
                        Tag = item,
                        Text = item.Name, 
                        Icon = item.Icon, 
                        Value = item.Value.Value
                    });
            Controls.Add(list);
            CheckAvailability();
        }

        private void CheckAvailability()
        {
            int coins = player.Inventory.Count(i => i is Coin);
            list.SelectedItem = null;
            foreach (var item in list.Items)
                item.Enabled = item.Value <= coins;
            list.SelectedItem = list.Items.Where(i => i.Enabled).FirstOrDefault();
        }

        private void OnInteract(TradingItem item)
        {
            // Item aus der Liste entfernen
            list.SelectedItem = null;
            list.Items.Remove(item);

            // Item Transfer
            Item i = item.Tag  as Item;
            Manager.Game.Simulation.Transfer(i, trader, player);
            var coins = player.Inventory.OfType<Coin>().ToArray();
            for (int j = 0; j < item.Value; j++) 
                Manager.Game.Simulation.Transfer(coins[j], player, null);

            // Finaler Cleanup
            CheckAvailability();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Manager.Game.Input.Handled)
            {
                if (Manager.Game.Input.Close)
                {
                    Manager.CloseScreen();
                    Manager.Game.Input.Handled = true;
                }
            }
        }
    }
}

