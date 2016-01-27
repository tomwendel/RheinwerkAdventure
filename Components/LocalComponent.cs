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
        public Player Player { get; set; }

        public LocalComponent(RheinwerkGame game)
            : base(game)
        {
            this.game = game;
        }

        /// <summary>
        /// Ermittelt die Area in der sich der lokale Spieler aktuell befindet.
        /// </summary>
        /// <returns>The current area.</returns>
        public Area GetCurrentArea()
        {
            if (Player == null || game.Simulation.World == null)
                return null;

            return game.Simulation.World.Areas.FirstOrDefault(a => a.Items.Contains(game.Local.Player));
        }

        public override void Update(GameTime gameTime)
        {
            // Nur wenn Komponente aktiviert wurde.
            if (!Enabled)
                return;

            // Nur arbeiten, wenn Player gesetzt wurde.
            if (Player == null)
                return;

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

