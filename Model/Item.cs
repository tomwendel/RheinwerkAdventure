using System;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Basisklasse für alle Spielitems auf dem Spielfeld.
    /// </summary>
    internal class Item
    {
        /// <summary>
        /// Position des Spielelementes.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Kollisionsradius des Spielelementes.
        /// </summary>
        /// <value>The radius.</value>
        public float Radius { get; set; }

        public Item()
        {
        }
    }
}

