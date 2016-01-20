using System;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert die Münzen im Spiel.
    /// </summary>
    internal class Diamant : Item, ICollectable
    {
        public Diamant()
        {
            // Standard-Masse für Diamanten
            Mass = 0.5f;
            Texture = "coin_silver.png";
        }
    }
}

