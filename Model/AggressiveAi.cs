using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Agressive KI für Gegner. Reagiert bei Sichtkontakt
    /// </summary>
    internal class AggressiveAi : Ai
    {
        private Vector2? startPoint;

        private IAttacker attacker;

        private Item target;

        private float range;

        public AggressiveAi(Character host, float range) : base(host) 
        {
            this.range = range;
            attacker = (IAttacker)host;
        }

        public override void OnUpdate(Area area, GameTime gameTime)
        {
            // Referenzpunkt ermitteln
            if (!startPoint.HasValue)
                startPoint = Host.Position;

            // Nach zielen ausschau halten
            if (target == null)
            {
                var potentialTargets = area.Items.
                    Where(i => (i.Position - Host.Position).LengthSquared() < range * range). // Filter nach Angriffsreichweite
                    Where(i => i.GetType() != Host.GetType()).                                // Items vom selben Typ verschonen
                    OrderBy(i => (i.Position - Host.Position).LengthSquared()).               // Sortiert nach Entfernung
                    OfType<IAttackable>().                                                    // Gefiltert nach Angreifbarkeit
                    Where(a => a.Hitpoints > 0);                                              // Gefiltert nach Lebendigkeit

                target = potentialTargets.FirstOrDefault() as Item;
            }

            // Ziel angreifen
            if (target != null)
            {
                attacker.AttackSignal = true;

                // Bei zu großem Abstand vom Ziel ablassen
                if ((target.Position - Host.Position).LengthSquared() > range * range ||
                    (target as IAttackable).Hitpoints <= 0)
                {
                    target = null;
                    WalkTo(startPoint.Value, 0.4f);
                }
                else
                {
                    WalkTo(target.Position, 0.6f);
                }
            }
        }
    }
}

