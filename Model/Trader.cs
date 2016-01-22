using System;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert den Händler im Spiel.
    /// </summary>
    internal class Trader : Character, IInteractable
    {
        public Action<RheinwerkGame, IInteractor, IInteractable> OnInteract { get; set; }

        public Trader()
        {
            Texture = "trader.png";
            Name = "Trader";
            Icon = "tradericon.png";
        }
    }
}

