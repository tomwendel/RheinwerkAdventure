using System;
using RheinwerkAdventure.Model;

namespace RheinwerkAdventure.Networking
{
    /// <summary>
    /// Cache-Klasse zur Haltung der wichtigsten Item-Daten.
    /// </summary>
    internal class ItemCacheEntry
    {
        /// <summary>
        /// Referenz auf das Spiel-Item
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// Referenz auf das Inventory-Item (falls im Inventar)
        /// </summary>
        public IInventory Inventory { get; set; }

        /// <summary>
        /// Referenz auf das Area (falls auf dem Spielfeld)
        /// </summary>
        public Area Area { get; set; }

        /// <summary>
        /// Frame des letzten Updates
        /// </summary>
        public int LastUpdate { get; set; }
    }
}

