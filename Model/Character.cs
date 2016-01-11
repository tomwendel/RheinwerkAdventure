using System;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert eine sich eigenständig bewegende Spieleinheit.
    /// </summary>
    internal class Character : Item
    {
        /// <summary>
        /// Geschwindigkeitsvektor.
        /// </summary>
        public Vector2 Velocity { get; set; }

        public Character()
        {
        }
    }
}

