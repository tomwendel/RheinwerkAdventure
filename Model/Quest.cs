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

        /// <summary>
        /// Setzt den Fortschritt des Quests.
        /// </summary>
        public void Progress(string id)
        {
            State = QuestState.Active;
            SetProgress(id);
        }

        /// <summary>
        /// Markiert das Quest als gescheitert.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void Fail(string id)
        {
            State = QuestState.Failed;
            SetProgress(id);
        }

        /// <summary>
        /// Markiert das Quest als erfolgreich beendet.
        /// </summary>
        public void Success(string id)
        {
            State = QuestState.Succeeded;
            SetProgress(id);
        }

        private void SetProgress(string id)
        {
            CurrentProgress = QuestProgresses.FirstOrDefault(q => q.Id.Equals(id));
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
        Inactive,

        /// <summary>
        /// Gestartet.
        /// </summary>
        Active,

        /// <summary>
        /// Verbockt.
        /// </summary>
        Failed,

        /// <summary>
        /// Erfolgreich erledigt.
        /// </summary>
        Succeeded,
    }
}

