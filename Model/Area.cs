using System;
using System.Collections.Generic;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert einen Teilbereich der Welt
    /// </summary>
    internal class Area
    {
        /// <summary>
        /// Auflistung aller enthaltener Zellen
        /// </summary>
        public List<Tile> Tiles { get; private set; }

        /// <summary>
        /// Auflistung aller enthaltener Items
        /// </summary>
        public List<Item> Items { get; private set; }

        public Area()
        {
            Tiles = new List<Tile>();
            Items = new List<Item>();
        }
    }
}

