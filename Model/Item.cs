using System;
using Microsoft.Xna.Framework;
using System.IO;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Basisklasse für alle Spielitems auf dem Spielfeld.
    /// </summary>
    internal class Item : ICollidable
    {
        // Internes Feld zur Haltung des temporären Move-Vektors.
        internal Vector2 move = Vector2.Zero;

        /// <summary>
        /// Id dieses Items.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Anzeigename dieses Items.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Iconname dieses Items.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Kosten im Handel oder null, falls unverkäuflich
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// Die Masse des Objektes.
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        /// Gibt an, ob dieses Element verschiebbar oder am Spielfeld fixiert ist.
        /// </summary>
        public bool Fixed { get; set; }

        /// <summary>
        /// Position des Spielelementes.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Kollisionsradius des Spielelementes.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Name der zu verwendenden Textur.
        /// </summary>
        public string Texture { get; set; }

        /// <summary>
        /// Action die bei jedem Schleifendurchlauf aufgerufen wird.
        /// </summary>
        public Action<RheinwerkGame, Area, Item, GameTime> Update { get; set; }

        public Item(int id)
        {
            Id = id;

            // Standard-Werte für Kollisionselemente
            Fixed = true;
            Mass = 1f;
            Radius = 0.25f;
            Name = "Item";
        }

        /// <summary>
        /// Serialisiert das Item für einen Insert.
        /// </summary>
        public virtual void SerializeInsert(BinaryWriter writer)
        {
            writer.Write(Position.X);
            writer.Write(Position.Y);
        }

        /// <summary>
        /// Serialisiert alle KeyUpdate Infos.
        /// </summary>
        public virtual void SerializeKeyUpdate(BinaryWriter writer)
        {
            writer.Write(Position.X);
            writer.Write(Position.Y);
        }

        /// <summary>
        /// Serialisiert alle Update Infos.
        /// </summary>
        public virtual void SerializeUpdate(BinaryWriter writer)
        {
        }

        /// <summary>
        /// Deserialisiert die Insert-Daten.
        /// </summary>
        public virtual void DeserializeInsert(BinaryReader reader)
        {
            Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        /// <summary>
        /// Deserialisiert Key Update Daten.
        /// </summary>
        public virtual void DeserializeKeyUpdate(BinaryReader reader)
        {
            Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        /// <summary>
        /// Deserialisiert Update Daten.
        /// </summary>
        public virtual void DeserializeUpdate(BinaryReader reader)
        {
        }
    }
}

