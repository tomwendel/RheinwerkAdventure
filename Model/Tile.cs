using System;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert eine Kachel einer Area.
    /// </summary>
    internal class Tile
    {
        /// <summary>
        /// Lage des Tiles innerhalb der Tiles-Textur.
        /// </summary>
        /// <value>The source rectangle.</value>
        public Rectangle SourceRectangle { get; set; }

        /// <summary>
        /// Gibt an ob diese Tile den Spieler an der Bewegung hindert.
        /// </summary>
        public bool Blocked { get; set; }

        public Tile()
        {
        }
    }
}

