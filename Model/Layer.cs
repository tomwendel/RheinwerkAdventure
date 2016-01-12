using System;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert eine Objekt-Schicht innerhalb eines Bereichs.
    /// </summary>
    internal class Layer
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

        public Layer(int width, int height)
        {
            // Sicherheitsprüfungen
            if (width < 5)
                throw new ArgumentException("Spielbereich muss mindestens 5 Zellen breit sein");
            if (height < 5)
                throw new ArgumentException("Spielfeld muss mindestens 5 Zellen hoch sein");

            Width = width;
            Height = height;

            // Leeres Array der Tiles erzeugen.
            Tiles = new Tile[width, height];
        }
    }
}

