using System;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert eine Kachel einer Area.
    /// </summary>
    internal class Tile
    {
        /// <summary>
        /// Gibt an ob diese Tile den Spieler an der Bewegung hindert.
        /// </summary>
        public bool Blocked { get; set; }

        public Tile()
        {
        }
    }
}

