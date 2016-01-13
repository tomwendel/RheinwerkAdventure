using System;
using System.Linq;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Model;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Game Komponente zur ständigen Berechnung des Spielverlaufs im Model.
    /// </summary>
    internal class SimulationComponent : GameComponent
    {
        private readonly RheinwerkGame game;

        /// <summary>
        /// Referenz auf das zentrale Spielmodell.
        /// </summary>
        public World World { get; private set; }

        /// <summary>
        /// Referenz auf den aktuellen Spieler.
        /// </summary>
        public Player Player { get; private set; }

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

            // Die erste Area mit leeren Tiles füllen.
            Area area = new Area(2, 30, 20);
            World.Areas.Add(area);
            for (int x = 0; x < area.Width; x++)
            {
                for (int y = 0; y < area.Height; y++)
                {
                    area.Layers[0].Tiles[x, y] = new Tile();
                    area.Layers[1].Tiles[x, y] = new Tile();

                    if (x == 0 || y == 0 || x == area.Width - 1 || y == area.Height - 1)
                        area.Layers[0].Tiles[x, y].Blocked = true;
                }
            }

            // Den Spieler einfügen.
            Player = new Player() { Position = new Vector2(15, 10), Radius = 0.25f };
            area.Items.Add(Player);

            // Einen Diamanten einfügen.
            Diamant diamant = new Diamant() { Position = new Vector2(10, 10), Radius = 0.25f };
            area.Items.Add(diamant);
        }

        public override void Update(GameTime gameTime)
        {
            #region Player Input

            Player.Velocity = game.Input.Movement * 10f;

            #endregion

            #region Character Movement

            foreach (var area in World.Areas)
            {
                foreach (var character in area.Items.OfType<Character>())
                {
                    // Neuberechnung der Character-Position.
                    character.Position += character.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Kollisionsprüfung mit allen restlichen Items.
                    foreach (var item in area.Items) {

                        if (item == character) continue;

                        Vector2 distance = item.Position - character.Position;
                        float overlap = item.Radius + character.Radius - distance.Length();
                        if (overlap > 0f)
                        {
                            Vector2 resolution = distance * (overlap / distance.Length());
                            if (item.Fixed && !character.Fixed)
                            {
                                // Item fixiert
                                character.Position -= resolution;
                            }
                            else if (!item.Fixed && character.Fixed)
                            {
                                // Character fixiert
                                item.Position += resolution;
                            }
                            else if (!item.Fixed && !character.Fixed)
                            {
                                // keiner fixiert
                                float totalMass = item.Mass + character.Mass;
                                character.Position -= resolution * (item.Mass / totalMass);
                                item.Position += resolution * (character.Mass / totalMass);
                            }
                        }
                    }
                }
            }

            #endregion

            base.Update(gameTime);
        }
    }
}

