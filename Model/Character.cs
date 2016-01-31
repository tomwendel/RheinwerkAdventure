using System;
using Microsoft.Xna.Framework;
using System.IO;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert eine sich eigenständig bewegende Spieleinheit.
    /// </summary>
    internal class Character : Item
    {
        /// <summary>
        /// Gibt die maximale Fortbeschwegungsgeschwindigkeit des Characters an.
        /// </summary>
        public float MaxSpeed { get; set; }

        /// <summary>
        /// Geschwindigkeitsvektor.
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// KI Basis
        /// </summary>
        public Ai Ai { get; set; }

        public Character(int id) : base(id)
        {
            MaxSpeed = 3f;
            Radius = 0.4f;
        }

        /// <summary>
        /// Serialisiert alle Update Infos.
        /// </summary>
        public override void SerializeUpdate(BinaryWriter writer)
        {
            base.SerializeUpdate(writer);

            // Serialisiert zusätzlich die Velocity im Update
            writer.Write(Velocity.X);
            writer.Write(Velocity.Y);
        }

        /// <summary>
        /// Serialisiert alle KeyUpdate Infos.
        /// </summary>
        public override void SerializeKeyUpdate(BinaryWriter writer)
        {
            base.SerializeKeyUpdate(writer);

            // Serialisiert zusätzlich die Velocity im Update
            writer.Write(Velocity.X);
            writer.Write(Velocity.Y);
        }

        /// <summary>
        /// Deserialisiert Key Update Daten.
        /// </summary>
        public override void DeserializeKeyUpdate(BinaryReader reader)
        {
            base.DeserializeKeyUpdate(reader);

            // Velocity wieder deserialisieren
            Velocity = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        /// <summary>
        /// Deserialisiert Update Daten.
        /// </summary>
        public override void DeserializeUpdate(BinaryReader reader)
        {
            base.DeserializeUpdate(reader);

            // Velocity wieder deserialisieren
            Velocity = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }
    }
}

