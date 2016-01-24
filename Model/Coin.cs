using System;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert die Münzen im Spiel.
    /// </summary>
    internal class Coin : Item, ICollectable
    {
        public Action<RheinwerkGame, Item> OnCollect { get; set; }

        public Coin()
        {
            // Standard-Masse für Diamanten
            Mass = 0.5f;
            Texture = "coin_silver.png";
            Name = "Coin";
            Icon = "coinicon.png";
        }
    }
}

