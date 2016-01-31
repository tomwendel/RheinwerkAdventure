using System;
using RheinwerkAdventure.Components;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert die Münzen im Spiel.
    /// </summary>
    internal class Coin : Item, ICollectable
    {
        public Action<SimulationComponent, Item> OnCollect { get; set; }

        public Coin(int id) : base(id)
        {
            // Standard-Masse für Diamanten
            Mass = 0.5f;
            Texture = "coin_silver.png";
            Name = "Coin";
            Icon = "coinicon.png";
        }
    }
}

