using System;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Interface für alle angreifbaren Spielelemente
    /// </summary>
    internal interface IAttackable
    {
        /// <summary>
        /// Anzahl verfügbarer Trefferpunkte.
        /// </summary>
        int Hitpoints { get; }
    }
}

