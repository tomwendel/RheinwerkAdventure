using System;
using System.Collections.Generic;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Einzelne Dialog-Option
    /// </summary>
    internal class Dialog
    {
        /// <summary>
        /// Link auf den zurück-Dialog.
        /// </summary>
        public Dialog Back { get; set; }

        /// <summary>
        /// Dialog-Option.
        /// </summary>
        public string Option { get; set; }

        /// <summary>
        /// Antwort des Characters
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gibt an ob diese Option angezeigt wird.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Gibt an ob man ab hier raus kann.
        /// </summary>
        public bool CanExit { get; set; }

        /// <summary>
        /// Liste der möglichen Dialog-Optionen.
        /// </summary>
        public List<Dialog> Options { get; set; }

        /// <summary>
        /// Delegat für den Aufruf dieser Option.
        /// </summary>
        public Action<RheinwerkGame, Item> OnShow { get; set; }

        public Dialog()
        {
            Options = new List<Dialog>();
            Visible = true;
        }
    }
}

