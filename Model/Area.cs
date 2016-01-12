using System;
using System.Collections.Generic;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert einen Teilbereich der Welt
    /// </summary>
    internal class Area
    {
        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        /// <summary>
        /// Auflistung aller enthaltener Zellen
        /// </summary>
        public Tile[,] Tiles { get; private set; }

        /// <summary>
        /// Auflistung aller enthaltener Items
        /// </summary>
        public List<Item> Items { get; private set; }

        public Area(int width, int height)
        {
            Width = width;
            Height = height;

            Tiles = new Tile[width, height];
            Items = new List<Item>();
        }
    }
}

