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
        /// Breite des Spielbereichs.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Höhe des Spielbereichs.
        /// </summary>
        public int Height { get; private set; }

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
            if (width < 5)
                throw new ArgumentException("Spielbereich muss mindestens 5 Zellen breit sein");
            if (height < 5)
                throw new ArgumentException("Spielfeld muss mindestens 5 Zellen hoch sein");

            Width = width;
            Height = height;

            Tiles = new Tile[width, height];
            Items = new List<Item>();
        }
    }
}

