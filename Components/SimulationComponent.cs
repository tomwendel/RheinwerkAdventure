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
            Area area = new Area(30, 20);
            World.Areas.Add(area);
            for (int x = 0; x < area.Width; x++)
            {
                for (int y = 0; y < area.Height; y++)
                {
                    area.Tiles[x, y] = new Tile();
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
                    character.Position += character.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            #endregion

            base.Update(gameTime);
        }
    }
}

