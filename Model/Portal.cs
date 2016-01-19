using System;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Portal-Bereich für die Übergänge zwischen Areas.
    /// </summary>
    internal class Portal
    {
        /// <summary>
        /// Name der Ziel-Area zu der das Portal führt.
        /// </summary>
        public string DestinationArea { get; set; }

        /// <summary>
        /// Portal-Bereich.
        /// </summary>
        public Rectangle Box { get; set; }
    }
}

