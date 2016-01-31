using System;
using System.Collections.Generic;
using RheinwerkAdventure.Model;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Hilfsklasse zum dynamischen Laden von Areas
    /// </summary>
    internal static class MapLoader
    {
        /// <summary>
        /// Lädt alle Areas die sich aktuell im Maps-Verzeichnis befinden.
        /// </summary>
        public static Area[] LoadAll(ref int nextId)
        {
            // Alle json-Files im Map-Folder suchen
            string mapPath = Path.Combine(Environment.CurrentDirectory, "Maps");
            var files = Directory.GetFiles(mapPath, "*.json");

            // Alle gefundenen json-Files laden
            Area[] result = new Area[files.Length];
            for (int i = 0; i < files.Length; i++)
                result[i] = LoadFromJson(files[i], ref nextId);

            return result;
        }

        /// <summary>
        /// Lädt die angegebene Datei in der Hoffnung um eine Area.
        /// </summary>
        public static Area LoadFromJson(string file, ref int nextId)
        {
            FileInfo info = new FileInfo(file);
            using (Stream stream = File.OpenRead(file))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    // json Datei auslesen
                    string json = sr.ReadToEnd();

                    // Deserialisieren
                    FileArea result = JsonConvert.DeserializeObject<FileArea>(json);

                    // Neue Area öffnen und mit den Root-Daten füllen
                    FileLayer[] tileLayer = result.layers.Where(l => l.type == "tilelayer").ToArray();
                    FileLayer objectLayer = result.layers.Where(l => l.type == "objectgroup").FirstOrDefault();
                    Area area = new Area(tileLayer.Length, result.width, result.height);
                    area.Name = info.Name.Substring(0, info.Name.Length - 5);

                    // Song auslesen
                    if (result.properties != null)
                        area.Song = result.properties.Song;

                    // Hintergrundfarbe interpretieren
                    area.Background = new Color(128, 128, 128);
                    if (!string.IsNullOrEmpty(result.backgroundcolor))
                    {
                        // Hexwerte als Farbwert parsen
                        area.Background = new Color(
                            Convert.ToInt32(result.backgroundcolor.Substring(1, 2), 16),
                            Convert.ToInt32(result.backgroundcolor.Substring(3, 2), 16),
                            Convert.ToInt32(result.backgroundcolor.Substring(5, 2), 16));
                    }

                    // Tiles zusammen suchen
                    for (int i = 0; i < result.tilesets.Length; i++)
                    {
                        FileTileset tileset = result.tilesets[i];

                        int start = tileset.firstgid;
                        int perRow = tileset.imagewidth / tileset.tilewidth;
                        int width = tileset.tilewidth;

                        for (int j = 0; j < tileset.tilecount; j++)
                        {
                            int x = j % perRow;
                            int y = j / perRow;

                            // Block-Status ermitteln
                            bool block = false;
                            if (tileset.tileproperties != null)
                            {
                                FileTileProperty property;
                                if (tileset.tileproperties.TryGetValue(j, out property))
                                    block = property.Block;
                            }

                            // Tile erstellen
                            Tile tile = new Tile()
                            { 
                                Texture = tileset.image,
                                SourceRectangle = new Rectangle(x * width, y * width, width, width),
                                Blocked = block
                            };

                            // In die Auflistung aufnehmen
                            area.Tiles.Add(start + j, tile);
                        }
                    }

                    // TileLayer erstellen
                    for (int l = 0; l < tileLayer.Length; l++)
                    {
                        FileLayer layer = tileLayer[l];

                        for (int i = 0; i < layer.data.Length; i++)
                        {
                            int x = i % area.Width;
                            int y = i / area.Width;
                            area.Layers[l].Tiles[x, y] = layer.data[i];
                        }
                    }

                    // Object Layer analysieren
                    if (objectLayer != null)
                    {
                        // Portals - Übertragungspunkte zu anderen Karten
                        foreach (var portal in objectLayer.objects.Where(o => o.type == "Portal"))
                        {
                            Rectangle box = new Rectangle(
                                                portal.x / result.tilewidth,
                                                portal.y / result.tileheight,
                                                portal.width / result.tilewidth,
                                                portal.height / result.tileheight
                                            );

                            area.Portals.Add(new Portal() { DestinationArea = portal.name, Box = box });
                        }

                        // Items (Spielelemente)
                        foreach (var item in objectLayer.objects.Where(o => o.type == "Item"))
                        {
                            Vector2 pos = new Vector2(
                                (item.x + (item.width / 2f)) / result.tilewidth,
                                (item.y + (item.height / 2f)) / result.tileheight);

                            switch (item.name)
                            {
                                case "coin": area.Items.Add(new Coin(nextId++) { Position = pos }); break;
                                case "goldencoin": area.Items.Add(new GoldenCoin(nextId++) { Position = pos }); break;
                                case "decard": area.Items.Add(new Decard(nextId++) { Position = pos }); break;
                                case "heidi": area.Items.Add(new Heidi(nextId++) { Position = pos }); break;
                                case "orc": area.Items.Add(new Orc(nextId++) { Position = pos }); break;
                                case "trader": 
                                    Trader trader = new Trader(nextId++) { Position = pos };
                                    trader.Inventory.Add(new IronSword(nextId++) { });
                                    trader.Inventory.Add(new WoodSword(nextId++) { });
                                    trader.Inventory.Add(new Gloves(nextId++) { });
                                    area.Items.Add(trader);
                                    break;
                            }
                        }

                        // Player (Startpunkte)
                        foreach (var player in objectLayer.objects.Where(o => o.type == "Player"))
                        {
                            Vector2 pos = new Vector2(
                                (player.x + (player.width / 2)) / result.tilewidth,
                                (player.y + (player.height / 2)) / result.tileheight);

                            area.Startpoints.Add(pos);
                        }
                    }

                    return area;
                }
            }
        }

        /// <summary>
        /// Root Objekt der Area-Datei.
        /// </summary>
        private class FileArea
        {
            /// <summary>
            /// Hintergrundfarbe der Karte als Hexcode
            /// </summary>
            public string backgroundcolor { get; set; }

            /// <summary>
            /// Abzahl Zellen in der Breite
            /// </summary>
            public int width { get; set; }

            /// <summary>
            /// Anzahl Zellen in der Höhe
            /// </summary>
            public int height { get; set; }

            /// <summary>
            /// Breite eines einzelnen Tiles
            /// </summary>
            public int tilewidth { get; set; }

            /// <summary>
            /// Höhe eines einzelnen Tiles
            /// </summary>
            public int tileheight { get; set; }

            /// <summary>
            /// Auflistung der Layer.
            /// </summary>
            public FileLayer[] layers { get; set; }

            /// <summary>
            /// Auflistung der Tilesets.
            /// </summary>
            public FileTileset[] tilesets { get; set; }

            /// <summary>
            /// Auflistung zusätzlicher Properties
            /// </summary>
            public FileAreaProperty properties { get; set; }
        }

        /// <summary>
        /// Layerdaten
        /// </summary>
        private class FileLayer
        {
            /// <summary>
            /// Fortlaufende Index-Liste der Tiles.
            /// </summary>
            public int[] data { get; set; }

            /// <summary>
            /// Gibt den Layer-Typ an (tilelayer oder objectgroup)
            /// </summary>
            public string type { get; set; }

            /// <summary>
            /// Auflistung der Objekte (für den Fall eines Object-Layers)
            /// </summary>
            public Obj[] objects { get; set; }
        }

        /// <summary>
        /// Tilesetdaten
        /// </summary>
        private class FileTileset
        {
            /// <summary>
            /// Erste Id der enthaltenen Tiles.
            /// </summary>
            public int firstgid { get; set; }

            /// <summary>
            /// Name der Textur.
            /// </summary>
            public string image { get; set; }

            /// <summary>
            /// Breite eines einzelnen Tiles.
            /// </summary>
            public int tilewidth { get; set; }

            /// <summary>
            /// Breite des Bildes.
            /// </summary>
            public int imagewidth { get; set; }

            /// <summary>
            /// Anzahl enthaltener Tiles.
            /// </summary>
            public int tilecount { get; set; }

            /// <summary>
            /// Auflistung zusätzlicher Properties von Tiles.
            /// </summary>
            public Dictionary<int, FileTileProperty> tileproperties { get; set; }
        }

        /// <summary>
        /// Ein Object auf einem Object-Layer.
        /// </summary>
        private class Obj
        {
            /// <summary>
            /// Name
            /// </summary>
            public string name { get; set; }

            /// <summary>
            /// Object-Type
            /// </summary>
            public string type { get; set; }

            /// <summary>
            /// X-Position
            /// </summary>
            public int x { get; set; }

            /// <summary>
            /// Y-Position
            /// </summary>
            public int y { get; set; }

            /// <summary>
            /// Breite
            /// </summary>
            public int width { get; set; }

            /// <summary>
            /// Höhe
            /// </summary>
            public int height { get; set; }
        }

        /// <summary>
        /// Zusätzliche "Custom Properties"
        /// </summary>
        private class FileTileProperty
        {
            /// <summary>
            /// Gibt an ob das Tile den Spieler blockiert
            /// </summary>
            public bool Block { get; set; }
        }

        /// <summary>
        /// Zusätzliche Properties in der Area.
        /// </summary>
        private class FileAreaProperty
        {
            /// <summary>
            /// Name des zu verwendenden Songs.
            /// </summary>
            public string Song { get; set; }
        }
    }
}

