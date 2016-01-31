using System;
using System.Collections.Generic;
using RheinwerkAdventure.Components;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert einen Orc.
    /// </summary>
    internal class Orc : Character, IAttackable, IAttacker
    {
        /// <summary>
        /// Maximale Anzahl Trefferpunkte im gesunden Zustand.
        /// </summary>
        public int MaxHitpoints { get; set; }

        /// <summary>
        /// Anzahl verfügbarer Trefferpunkte.
        /// </summary>
        public int Hitpoints { get; set; }

        /// <summary>
        /// Intern geführte Liste aller angreifbaren Elemente in der Nähe.
        /// </summary>
        public ICollection<IAttackable> AttackableItems { get; private set; }

        /// <summary>
        /// Angriffsradius in dem Schaden ausgeteilt wird.
        /// </summary>
        public float AttackRange { get; set; }

        /// <summary>
        /// Schaden der pro Angriff verursacht wird.
        /// </summary>
        public int AttackValue { get; set; }

        /// <summary>
        /// Gibt die Zeitspanne an, die der Character zur Erholung von einem Schlag benötigt.
        /// </summary>
        public TimeSpan TotalRecovery { get; set; }

        /// <summary>
        /// Gibt die noch verbleibende Erholungszeit an, bevor erneut geschlagen werden kann.
        /// </summary>
        public TimeSpan Recovery { get; set; }

        /// <summary>
        /// Interner Flag um bevorstehenden Angriff zu signalisieren.
        /// </summary>
        public bool AttackSignal { get; set; }

        /// <summary>
        /// Aufruf bei ankommenden Treffern.
        /// </summary>
        public Action<SimulationComponent, IAttacker, IAttackable> OnHit { get; set; }

        public Orc(int id) : base(id)
        {
            AttackableItems = new List<IAttackable>();
            MaxHitpoints = 2;
            Hitpoints = 2;
            AttackRange = 0.8f;
            AttackValue = 1;
            TotalRecovery = TimeSpan.FromSeconds(0.6);
            Texture = "orc.png";
            Name = "Orc";
            Icon = "orcicon.png";
            Ai = new AggressiveAi(this, 4f);
        }
    }
}

