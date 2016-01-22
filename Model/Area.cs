using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert einen Teilbereich der Welt
    /// </summary>
    internal class Area
    {
        /// <summary>
        /// Name des Bereichs.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gibt den Song für diese Area an.
        /// </summary>
        public string Song { get; set; }

        /// <summary>
        /// Hintergrundfarbe des Bereiches.
        /// </summary>
        public Color Background { get; set; }

        /// <summary>
        /// Breite des Spielbereichs.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Höhe des Spielbereichs.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Auflistung der Objekt-Layer.
        /// </summary>
        public Layer[] Layers { get; private set; }

        /// <summary>
        /// Auflistung der Portale zu anderen Areas.
        /// </summary>
        public List<Portal> Portals { get; private set; }

        /// <summary>
        /// Auflistung aller enthaltener Items
        /// </summary>
        public List<Item> Items { get; private set; }

        /// <summary>
        /// Auflistung potentieller Startpunkte für den Spieler
        /// </summary>
        public List<Vector2> Startpoints { get; private set; }

        /// <summary>
        /// Zentrales Repository für Zellentemplates (Tiles)
        /// </summary>
        public Dictionary<int,Tile> Tiles { get; private set; }

        public Area(int layers, int width, int height)
        {
            // Sicherheitsprüfungen
            if (width < 5)
                throw new ArgumentException("Spielbereich muss mindestens 5 Zellen breit sein");
            if (height < 5)
                throw new ArgumentException("Spielfeld muss mindestens 5 Zellen hoch sein");

            Width = width;
            Height = height;

            // Erzeugung der unterschiedlichen Layer.
            Layers = new Layer[layers];
            for (int l = 0; l < layers; l++)
                Layers[l] = new Layer(width, height);

            // Leere Liste der Spielelemente.
            Items = new List<Item>();

            // Leere Liste der Portale erstellen
            Portals = new List<Portal>();

            // Leere Liste von Tiles erstellen.
            Tiles = new Dictionary<int, Tile>();

            // Leere Liste von Startpunkten.
            Startpoints = new List<Vector2>();
        }

        /// <summary>
        /// Ermittelt über alle vorhandenen Layer hinweg, ob diese Zelle durch einen entsprechendes Tile blockiert wird.
        /// Ist der Index außerhalb des Spielfeldes gilt die Zelle grundsätzlich als blockierte Zelle.
        /// </summary>
        /// <returns>Gibt an ob die angefragte Zelle von Spielelementen betreten werden kann.</returns>
        /// <param name="x">Spalte</param>
        /// <param name="y">Zeile</param>
        public bool IsCellBlocked(int x, int y)
        {
            // Sonderfall außerhalb des Spielfeldes
            if (x < 0 || y < 0 || x > Width - 1 || y > Height - 1)
                return true;

            // Schleife über alle Layer um einen Blocker zu finden.
            for (int l = 0; l < Layers.Length; l++)
            {
                int tileId = Layers[l].Tiles[x, y];
                if (tileId == 0)
                    continue;

                Tile tile = Tiles[tileId];

                // Blocker gefunden -> Zelle ist blockiert
                if (tile.Blocked)
                    return true;
            }

            // Keinen Blocker gefunden -> Zelle begehbar.
            return false;
        }
    }
}

