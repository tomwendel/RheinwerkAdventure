using System;
using System.Collections.Generic;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert einen Spieler-Charakter.
    /// </summary>
    internal class Player : Character, IAttackable, IAttacker, IInteractor
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
        public ICollection<Item> AttackableItems { get; private set; }

        /// <summary>
        /// Angriffsradius in dem Schaden ausgeteilt wird.
        /// </summary>
        public float AttackRange { get; set; }

        /// <summary>
        /// Schaden der pro Angriff verursacht wird.
        /// </summary>
        public int AttackValue { get; set; }

        /// <summary>
        /// Interne auflistung aller Items im Interaktionsradius.
        /// </summary>
        public ICollection<Item> InteractableItems { get; private set; }

        /// <summary>
        /// Interaktionsradius in dem interagiert werden kann.
        /// </summary>
        public float InteractionRange { get; set; }

        public Player()
        {
            AttackableItems = new List<Item>();
            InteractableItems = new List<Item>();
            MaxHitpoints = 4;
            Hitpoints = 4;
            AttackRange = 0.5f;
            AttackValue = 1;
            InteractionRange = 0.5f;
        }
    }
}

