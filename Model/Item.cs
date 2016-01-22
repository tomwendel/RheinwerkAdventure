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
        /// Anzeigename dieses Items.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Iconname dieses Items.
        /// </summary>
        public string Icon { get; set; }

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
        public float Radius { get; set; }

        /// <summary>
        /// Name der zu verwendenden Textur.
        /// </summary>
        public string Texture { get; set; }

        public Item()
        {
            // Standard-Werte für Kollisionselemente
            Fixed = true;
            Mass = 1f;
            Radius = 0.25f;
            Name = "Item";
        }
    }
}

