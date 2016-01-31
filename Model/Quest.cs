using System;
using System.Linq;
using System.Collections.Generic;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert eine Quest
    /// </summary>
    internal class Quest
    {
        /// <summary>
        /// Name der Quest
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Aktueller Status der Quest.
        /// </summary>
        public QuestState State { get; set; }

        /// <summary>
        /// Gibt den aktuellen Fortschritt an.
        /// </summary>
        public QuestProgress CurrentProgress { get; set; }

        /// <summary>
        /// Auflistung der möglichen Fortschrittsoptionen.
        /// </summary>
        public List<QuestProgress> QuestProgresses { get; set; }

        public Quest()
        {
            QuestProgresses = new List<QuestProgress>();
            State = QuestState.Inactive;
        }
    }

    /// <summary>
    /// Auflistung der möglichen Quest-States.
    /// </summary>
    internal enum QuestState
    {
        /// <summary>
        /// Nicht gestartet.
        /// </summary>
        Inactive = 1,

        /// <summary>
        /// Gestartet.
        /// </summary>
        Active = 2,

        /// <summary>
        /// Verbockt.
        /// </summary>
        Failed = 3,

        /// <summary>
        /// Erfolgreich erledigt.
        /// </summary>
        Succeeded = 4,
    }
}

