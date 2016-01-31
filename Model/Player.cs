using System;
using System.Collections.Generic;
using RheinwerkAdventure.Components;
using System.IO;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert einen Spieler-Charakter.
    /// </summary>
    internal class Player : Character, IAttackable, IAttacker, IInteractor, IInventory
    {
        private List<Item> inventory;

        /// <summary>
        /// Maximale Anzahl Trefferpunkte im gesunden Zustand.
        /// </summary>
        public int MaxHitpoints { get; set; }

        /// <summary>
        /// Anzahl verfügbarer Trefferpunkte.
        /// </summary>
        public int Hitpoints { get; set; }

        /// <summary>
        /// Zeigt an, ob der Spieler noch in einem Portal steht.
        /// </summary>
        public bool InPortal { get; set; }

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
        /// Interne auflistung aller Items im Interaktionsradius.
        /// </summary>
        public ICollection<IInteractable> InteractableItems { get; private set; }

        /// <summary>
        /// Interaktionsradius in dem interagiert werden kann.
        /// </summary>
        public float InteractionRange { get; set; }

        /// <summary>
        /// Listet die Items im Inventar auf.
        /// </summary>
        public ICollection<Item> Inventory { get { return inventory; } }

        /// <summary>
        /// Interaktionsradius in dem interagiert werden kann.
        /// </summary>
        public bool InteractSignal { get; set; }

        /// <summary>
        /// Interner Flag um bevorstehenden Angriff zu signalisieren.
        /// </summary>
        public bool AttackSignal { get; set; }

        /// <summary>
        /// Aufruf bei ankommenden Treffern.
        /// </summary>
        public Action<SimulationComponent, IAttacker, IAttackable> OnHit { get; set; }

        public Player(int id) : base(id)
        {
            inventory = new List<Item>();
            AttackableItems = new List<IAttackable>();
            InteractableItems = new List<IInteractable>();
            Fixed = false;
            MaxHitpoints = 4;
            Hitpoints = 4;
            AttackRange = 1f;
            AttackValue = 1;
            TotalRecovery = TimeSpan.FromSeconds(0.3);
            InteractionRange = 0.8f;
            Texture = "char.png";
            Name = "Player";
            Icon = "charicon.png";
        }

        /// <summary>
        /// Serialisiert alle Update Infos.
        /// </summary>
        public override void SerializeUpdate(BinaryWriter writer)
        {
            base.SerializeUpdate(writer);

            // Überträgt zusätzlich das Attack-Signal
            writer.Write(AttackSignal);
        }

        /// <summary>
        /// Deserialisiert Update Daten.
        /// </summary>
        public override void DeserializeUpdate(BinaryReader reader)
        {
            base.DeserializeUpdate(reader);

            // Liest das Attack-Signal aus.
            AttackSignal = reader.ReadBoolean();
        }

        /// <summary>
        /// Serialisiert alle Key Update Infos.
        /// </summary>
        public override void SerializeKeyUpdate(BinaryWriter writer)
        {
            base.SerializeKeyUpdate(writer);

            // Serialisiert im Key Update die Hitpoints.
            writer.Write(Hitpoints);
        }

        /// <summary>
        /// Deserialisiert Key Update Daten.
        /// </summary>
        public override void DeserializeKeyUpdate(BinaryReader reader)
        {
            base.DeserializeKeyUpdate(reader);

            // Deserialisiert die Hitpoints im Key Update.
            Hitpoints = reader.ReadInt32();
        }
    }
}

