using System;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Basisklasse für alle Spielitems auf dem Spielfeld.
    /// </summary>
    internal class Item : ICollidable
    {
        // Internes Feld zur Haltung des temporären Move-Vektors.
        internal Vector2 move = Vector2.Zero;

        /// <summary>
        /// Die Masse des Objektes.
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        /// Gibt an, ob dieses Element verschiebbar oder am Spielfeld fixiert ist.
        /// </summary>
        public bool Fixed { get; set; }

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
            // Standard-Werte für Kollisionselemente
            Fixed = false;
            Mass = 1f;
        }
    }
}

