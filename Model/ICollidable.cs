using System;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Interface für alle kollidierbaren Spielelemente.
    /// </summary>
    internal interface ICollidable
    {
        /// <summary>
        /// Die Masse des Objektes.
        /// </summary>
        float Mass { get; }

        /// <summary>
        /// Gibt an, ob dieses Element verschiebbar oder am Spielfeld fixiert ist.
        /// </summary>
        bool Fixed { get; }
    }
}

