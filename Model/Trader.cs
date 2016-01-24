using System;
using RheinwerkAdventure.Screens;
using System.Collections.Generic;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert den Händler im Spiel.
    /// </summary>
    internal class Trader : Character, IInteractable, IInventory
    {
        private List<Item> inventory;

        public Action<RheinwerkGame, IInteractor, IInteractable> OnInteract { get; set; }

        /// <summary>
        /// Das Händler-Inventar.
        /// </summary>
        public ICollection<Item> Inventory { get { return inventory; } }

        public Trader()
        {
            Texture = "trader.png";
            Name = "Hardwig";
            Icon = "tradericon.png";

            inventory = new List<Item>();
            inventory.Add(new IronSword() { });
            inventory.Add(new WoodSword() { });
            inventory.Add(new Gloves() { });

            OnInteract = (game, player, trader) =>
            {
                game.Screen.ShowScreen(new TradeScreen(game.Screen, trader as IInventory, player as IInventory));
            };
        }
    }
}

