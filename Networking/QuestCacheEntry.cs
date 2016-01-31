using System;
using RheinwerkAdventure.Model;

namespace RheinwerkAdventure.Networking
{
    /// <summary>
    /// Cache-Item für Quests
    /// </summary>
    internal class QuestCacheEntry
    {
        /// <summary>
        /// Name der Quest
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Progress-Stage der Quest
        /// </summary>
        public string Progress { get; set; }

        /// <summary>
        /// Status der Quest
        /// </summary>
        public QuestState State  { get; set; }
    }
}

