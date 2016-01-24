using System;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Beschreibt den Fortschrittsgrad einer Quest.
    /// </summary>
    internal class QuestProgress
    {
        /// <summary>
        /// ID des Fortschritts (wird für die Identifikation beim Setzen gebraucht)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Beschreibungstext.
        /// </summary>
        public string Description { get; set; }
    }
}

