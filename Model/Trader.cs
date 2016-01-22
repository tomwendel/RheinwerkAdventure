using System;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert den Händler im Spiel.
    /// </summary>
    internal class Trader : Character, IInteractable
    {
        public Action<Player> OnInteract { get; set; }

        public Trader()
        {
            Texture = "trader.png";
            Name = "Trader";
            Icon = "tradericon.png";
        }
    }
}

