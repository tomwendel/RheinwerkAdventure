using System;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Standard-KI für normale Character. Char läuft so umher.
    /// </summary>
    internal class WalkingAi : Ai
    {
        private float range;

        private Vector2? center;

        private TimeSpan delay;

        protected Random Random { get; private set; }

        public WalkingAi(Character host, float range) : base(host) 
        {
            this.range = range;
            Random = new Random();
            delay = TimeSpan.Zero;
        }

        public override void OnUpdate(Area area, GameTime gameTime)
        {
            // Initiales Zentrum festlegen
            if (!center.HasValue)
                center = Host.Position;

            if (!Walking)
            {
                // Delay abwarten
                if (delay > TimeSpan.Zero)
                {
                    delay -= gameTime.ElapsedGameTime;
                    return;
                }

                // Neuen Zielpunkt wählen
                Vector2 variation = new Vector2(
                    (float)(Random.NextDouble() * 2 - 1.0), 
                    (float)(Random.NextDouble() * 2 - 1.0));
                WalkTo(center.Value + variation * 2 * range, 0.4f);
                delay = TimeSpan.FromSeconds(2);
            }
        }
    }
}

