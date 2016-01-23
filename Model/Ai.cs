using System;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Basis-Klasse für eigenständige KIs
    /// </summary>
    internal abstract class Ai
    {
        /// <summary>
        /// Hält eine Referenz auf den Character.
        /// </summary>
        protected Character Host { get; private set; }

        /// <summary>
        /// Gibt an ob die KI im Wandermodus ist.
        /// </summary>
        protected bool Walking { get { return destination.HasValue; } }

        /// <summary>
        /// Speichert den Startpunkt des Wanderauftrags.
        /// </summary>
        private Vector2? startPoint;

        /// <summary>
        /// Speichert den Zielpunkt des Wanderauftrags.
        /// </summary>
        private Vector2? destination;

        private float speed;

        public Ai(Character host)
        {
            Host = host;
            startPoint = null;
            destination = null;
        }

        public void Update(Area area, GameTime gameTime)
        {
            OnUpdate(area, gameTime);

            // Bewegung
            if (destination.HasValue)
            {
                Vector2 expectedDistance = destination.Value - startPoint.Value;
                Vector2 currentDistance = Host.Position - startPoint.Value;

                // Prüfen ob das Ziel erreicht (oder überschritten) wurde.
                if (currentDistance.LengthSquared() > expectedDistance.LengthSquared())
                {
                    startPoint = null;
                    destination = null;
                    Host.Velocity = Vector2.Zero;
                }
                else
                {
                    // Kurs festlegen
                    Vector2 direction = destination.Value - Host.Position;
                    direction.Normalize();
                    Host.Velocity = direction * speed * Host.MaxSpeed;
                }
            }
        }

        public abstract void OnUpdate(Area area, GameTime gameTime);

        protected void WalkTo(Vector2 destination, float speed)
        {
            startPoint = Host.Position;
            this.destination = destination;
            this.speed = speed;
        }
    }
}

