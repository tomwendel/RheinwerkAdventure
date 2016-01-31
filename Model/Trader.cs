using System;
using RheinwerkAdventure.Screens;
using System.Collections.Generic;
using RheinwerkAdventure.Components;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert den Händler im Spiel.
    /// </summary>
    internal class Trader : Character, IInteractable, IInventory
    {
        private List<Item> inventory;

        public Action<SimulationComponent, IInteractor, IInteractable> OnInteract { get; set; }

        /// <summary>
        /// Das Händler-Inventar.
        /// </summary>
        public ICollection<Item> Inventory { get { return inventory; } }

        public Trader(int id) : base(id)
        {
            Texture = "trader.png";
            Name = "Hardwig";
            Icon = "tradericon.png";

            inventory = new List<Item>();

            OnInteract = (simulation, player, trader) =>
            {
                    RheinwerkGame game = simulation.Game as RheinwerkGame;
                    simulation.ShowInteractionScreen(player as Player, new TradeScreen(game.Screen, trader as IInventory, player as IInventory));
            };
        }
    }
}

