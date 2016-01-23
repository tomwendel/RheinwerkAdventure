using System;
using System.Linq;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Model;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Container für den lokalen Spieler
    /// </summary>
    internal class LocalComponent : GameComponent
    {
        private readonly RheinwerkGame game;

        /// <summary>
        /// Referenz auf den aktuellen Spieler.
        /// </summary>
        public Player Player { get; private set; }

        public LocalComponent(RheinwerkGame game)
            : base(game)
        {
            this.game = game;

            // Den Spieler einfügen.
            game.Simulation.InsertPlayer(Player = new Player());
        }

        /// <summary>
        /// Ermittelt die Area in der sich der lokale Spieler aktuell befindet.
        /// </summary>
        /// <returns>The current area.</returns>
        public Area GetCurrentArea()
        {
            return game.Simulation.World.Areas.FirstOrDefault(a => a.Items.Contains(game.Local.Player));
        }

        public override void Update(GameTime gameTime)
        {
            if (!game.Input.Handled)
            {
                Player.Velocity = game.Input.Movement * Player.MaxSpeed;

                // Interaktionen signalisieren
                if (game.Input.Interact)
                    Player.InteractSignal = true;

                // Angriff signalisieren
                if (game.Input.Attack)
                    Player.AttackSignal = true;

                game.Input.Handled = true;
            }
            else
            {
                Player.Velocity = Vector2.Zero;
            }
        }
    }
}

