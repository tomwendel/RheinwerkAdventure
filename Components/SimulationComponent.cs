using System;
using System.Linq;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Model;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using RheinwerkAdventure.Screens;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Game Komponente zur ständigen Berechnung des Spielverlaufs im Model.
    /// </summary>
    internal class SimulationComponent : GameComponent
    {
        // Sicherheitslücke gegen Rundungsfehler
        private float gap = 0.00001f;

        private readonly RheinwerkGame game;

        /// <summary>
        /// Referenz auf das zentrale Spielmodell.
        /// </summary>
        public World World { get; private set; }

        /// <summary>
        /// Referenz auf den aktuellen Spieler.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Referenz auf die aktuelle Area in der sich der Spieler gerade befindet.
        /// </summary>
        public Area Area { get; private set; }

        public SimulationComponent(RheinwerkGame game)
            : base(game)
        {
            this.game = game;

            // Zu Beginn eine neue Spielwelt erzeugen.
            NewGame();
        }

        public void NewGame()
        {
            World = new World();

            // Town Area
            Area town = LoadFromJson("town");
            town.Song = "town";
            town.Portals.Add(new Portal() { DestinationArea = "wood", Box = new Rectangle(0, 15, 1, 5) });
            town.Portals.Add(new Portal() { DestinationArea = "shop", Box = new Rectangle(24, 14, 1, 1) });
            World.Areas.Add(town);

            // Shop Area
            Area shop = LoadFromJson("shop");
            shop.Song = "house";
            shop.Portals.Add(new Portal() { DestinationArea = "town", Box = new Rectangle(0, 9, 9, 1) });
            World.Areas.Add(shop);

            // Wood Area
            Area wood = LoadFromJson("wood");
            wood.Song = "town";
            wood.Portals.Add(new Portal() { DestinationArea = "town", Box = new Rectangle(29, 16, 1, 4) });
            World.Areas.Add(wood);

            // Den Spieler einfügen.
            Player = new Player() { Position = new Vector2(16f, 10f) };
            Area = town;
            Area.Items.Add(Player);

            // Decard einfügen
            Decard decard = new Decard() { Position = new Vector2(18f, 16) };
            decard.OnInteract = (p) =>
            {
                game.Screen.ShowScreen(new ShoutScreen(game.Screen, decard, "Bleib ein Weilchen und hoer zu!"));
            };
            Area.Items.Add(decard);

            // Ein paar Münzen einfügen.
            Area.Items.Add(new Coin() { Position = new Vector2(10, 10) });
            Area.Items.Add(new Coin() { Position = new Vector2(10, 6) });
            Area.Items.Add(new Coin() { Position = new Vector2(20, 6) });
        }

        public override void Update(GameTime gameTime)
        {
            #region Player Input

            if (!game.Input.Handled)
            {
                Player.Velocity = game.Input.Movement * Player.MaxSpeed;
            }
            else
            {
                Player.Velocity = Vector2.Zero;
            }

            #endregion

            #region Character Movement

            List<Action> transfers = new List<Action>();
            foreach (var area in World.Areas)
            {
                // Schleife über alle sich aktiv bewegenden Spiel-Elemente
                foreach (var character in area.Items.OfType<Character>())
                {
                    // Neuberechnung der Character-Position.
                    character.move += character.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Attacker identifizieren
                    IAttacker attacker = null;
                    if (character is IAttacker)
                    {
                        attacker = (IAttacker)character;
                        attacker.AttackableItems.Clear();
                    }

                    // Interactor identifizieren
                    IInteractor interactor = null;
                    if (character is IInteractor)
                    {
                        interactor = (IInteractor)character;
                        interactor.InteractableItems.Clear();
                    }

                    // Kollisionsprüfung mit allen restlichen Items.
                    foreach (var item in area.Items)
                    {
                        // Kollision mit sich selbst ausschließen
                        if (item == character)
                            continue;

                        // Distanz berechnen
                        Vector2 distance = (item.Position + item.move) - (character.Position + character.move);

                        // Ermittlung der angreifbaren Items.
                        if (attacker != null &&
                            item is IAttackable &&
                            distance.Length() - attacker.AttackRange - item.Radius < 0f)
                        {
                            attacker.AttackableItems.Add(item);
                        }

                        // Ermittlung der interagierbaren Items.
                        if (interactor != null &&
                            item is IInteractable &&
                            distance.Length() - interactor.InteractionRange - item.Radius < 0f)
                        {
                            interactor.InteractableItems.Add(item);
                        }

                        // Überschneidung berechnen & darauf reagieren
                        float overlap = item.Radius + character.Radius - distance.Length();
                        if (overlap > 0f)
                        {
                            Vector2 resolution = distance * (overlap / distance.Length());
                            if (item.Fixed && !character.Fixed)
                            {
                                // Item fixiert
                                character.move -= resolution;
                            }
                            else if (!item.Fixed && character.Fixed)
                            {
                                // Character fixiert
                                item.move += resolution;
                            }
                            else if (!item.Fixed && !character.Fixed)
                            {
                                // keiner fixiert
                                float totalMass = item.Mass + character.Mass;
                                character.move -= resolution * (item.Mass / totalMass);
                                item.move += resolution * (character.Mass / totalMass);
                            }

                            // Kombination aus Collectable und Iventory
                            if (item is ICollectable && character is IInventory)
                            {
                                //  -> Character sammelt Item ein
                                transfers.Add(() =>
                                    {
                                        area.Items.Remove(item);
                                        (character as IInventory).Inventory.Add(item);
                                        item.Position = Vector2.Zero;

                                        if (character == Player)
                                            game.Sound.PlayCoin();
                                    });
                            }
                        }
                    }
                }

                // Kollision mit blockierten Zellen
                foreach (var item in area.Items)
                {
                    bool collision = false;
                    int loops = 0;

                    do
                    {
                        // Grenzbereiche für die zu überprüfenden Zellen ermitteln
                        Vector2 position = item.Position + item.move;
                        int minCellX = (int)(position.X - item.Radius);
                        int maxCellX = (int)(position.X + item.Radius);
                        int minCellY = (int)(position.Y - item.Radius);
                        int maxCellY = (int)(position.Y + item.Radius);

                        collision = false;
                        float minImpact = 2f;
                        int minAxis = 0;

                        // Schleife über alle betroffenen Zellen zur Ermittlung der ersten Kollision
                        for (int x = minCellX; x <= maxCellX; x++)
                        {
                            for (int y = minCellY; y <= maxCellY; y++)
                            {
                                // Zellen ignorieren die den Spieler nicht blockieren
                                if (!area.IsCellBlocked(x, y))
                                    continue;

                                // Zellen ignorieren die vom Spieler nicht berührt werden
                                if (position.X - item.Radius > x + 1 ||
                                    position.X + item.Radius < x ||
                                    position.Y - item.Radius > y + 1 ||
                                    position.Y + item.Radius < y)
                                    continue;

                                collision = true;

                                // Kollisionszeitpunkt auf X-Achse ermitteln
                                float diffX = float.MaxValue;
                                if (item.move.X > 0)
                                    diffX = position.X + item.Radius - x + gap;
                                if (item.move.X < 0)
                                    diffX = position.X - item.Radius - (x + 1) - gap;
                                float impactX = 1f - (diffX / item.move.X);

                                // Kollisionszeitpunkt auf Y-Achse ermitteln
                                float diffY = float.MaxValue;
                                if (item.move.Y > 0)
                                    diffY = position.Y + item.Radius - y + gap;
                                if (item.move.Y < 0)
                                    diffY = position.Y - item.Radius - (y + 1) - gap;
                                float impactY = 1f - (diffY / item.move.Y);

                                // Relevante Achse ermitteln
                                // Ergibt sich aus dem spätesten Kollisionszeitpunkt
                                int axis = 0;
                                float impact = 0;
                                if (impactX > impactY)
                                {
                                    axis = 1;
                                    impact = impactX;
                                }
                                else
                                {
                                    axis = 2;
                                    impact = impactY;
                                }

                                // Ist diese Kollision eher als die bisher erkannten
                                if (impact < minImpact)
                                {
                                    minImpact = impact;
                                    minAxis = axis;
                                }
                            }
                        }

                        // Im Falle einer Kollision in diesem Schleifendurchlauf...
                        if (collision)
                        {
                            // X-Anteil ab dem Kollisionszeitpunkt kürzen
                            if (minAxis == 1)
                                item.move *= new Vector2(minImpact, 1f);

                            // Y-Anteil ab dem Kollisionszeitpunkt kürzen
                            if (minAxis == 2)
                                item.move *= new Vector2(1f, minImpact);
                        }
                        loops++;
                    }
                    while(collision && loops < 2);

                    // Finaler Move-Vektor auf die Position anwenden.
                    item.Position += item.move;
                    item.move = Vector2.Zero;

                    // Portal anwenden (nur Player)
                    if (area.Portals != null && item is Player)
                    {
                        Player player = item as Player;
                        bool inPortal = false;

                        foreach (var portal in area.Portals)
                        {
                            if (item.Position.X > portal.Box.Left &&
                                item.Position.X <= portal.Box.Right &&
                                item.Position.Y > portal.Box.Top &&
                                item.Position.Y <= portal.Box.Bottom)
                            {
                                inPortal = true;
                                if (player.InPortal)
                                    continue;

                                // Ziel-Area und Portal finden
                                Area destinationArea = World.Areas.First(a => a.Name.Equals(portal.DestinationArea));
                                Portal destinationPortal = destinationArea.Portals.First(p => p.DestinationArea.Equals(area.Name));

                                // Neue Position des Spielers finden
                                Vector2 position = new Vector2(
                                                       destinationPortal.Box.X + (destinationPortal.Box.Width / 2f), 
                                                       destinationPortal.Box.Y + (destinationPortal.Box.Height / 2f));

                                // Transfer in andere Area vorbereiten
                                transfers.Add(() =>
                                    {
                                        area.Items.Remove(item);
                                        destinationArea.Items.Add(item);
                                        item.Position = position;

                                        if (item == Player)
                                            Area = destinationArea;
                                    });
                            }
                        }

                        player.InPortal = inPortal;
                    }
                }
            }

            // Transfers durchführen
            foreach (var transfer in transfers)
            {
                transfer();
            }

            // Interaktionen durchführen
            if (game.Input.Interact)
            {
                // Alle Items in der Nähe aufrufen
                foreach (var item in Player.InteractableItems)
                {
                    var interactable = item as IInteractable;
                    interactable.OnInteract(Player);
                }
                game.Input.Handled = true;
            }

            #endregion

            base.Update(gameTime);
        }

        private Area LoadFromJson(string name)
        {
            string rootPath = Path.Combine(Environment.CurrentDirectory, "Maps");
            using (Stream stream = File.OpenRead(rootPath + "\\" + name + ".json"))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    // json Datei auslesen
                    string json = sr.ReadToEnd();

                    // Deserialisieren
                    FileArea result = JsonConvert.DeserializeObject<FileArea>(json);

                    // Neue Area öffnen und mit den Root-Daten füllen
                    Area area = new Area(result.layers.Length, result.width, result.height);
                    area.Name = name;

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

                    // Layer erstellen
                    for (int l = 0; l < result.layers.Length; l++)
                    {
                        FileLayer layer = result.layers[l];

                        for (int i = 0; i < layer.data.Length; i++)
                        {
                            int x = i % area.Width;
                            int y = i / area.Width;
                            area.Layers[l].Tiles[x, y] = layer.data[i];
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
            /// Auflistung der Layer.
            /// </summary>
            public FileLayer[] layers { get; set; }

            /// <summary>
            /// Auflistung der Tilesets.
            /// </summary>
            public FileTileset[] tilesets { get; set; }
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
        /// Zusätzliche "Custom Properties"
        /// </summary>
        public class FileTileProperty
        {
            /// <summary>
            /// Gibt an ob das Tile den Spieler blockiert
            /// </summary>
            public bool Block { get; set; }
        }
    }
}

