using System;
using System.Collections.Generic;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert eine Instanz der Welt.
    /// </summary>
    internal class World
    {
        /// <summary>
        /// Auflistung aller Areas
        /// </summary>
        public List<Area> Areas { get; private set; }

        /// <summary>
        /// Auflistung aller Quests
        /// </summary>
        public List<Quest> Quests { get; private set; }

        /// <summary>
        /// GIbt die ID an die an das neue Element vergeben werden kann
        /// </summary>
        public int NextId { get; set; }

        public World()
        {
            Areas = new List<Area>();
            Quests = new List<Quest>();
            NextId = 1;
        }
    }
}

