using System;
using System.Collections.Generic;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Interface für alle aktiven Angreifer.
    /// </summary>
    internal interface IAttacker
    {
        /// <summary>
        /// Intern geführte Liste aller angreifbaren Elemente in der Nähe.
        /// </summary>
        ICollection<IAttackable> AttackableItems { get; }

        /// <summary>
        /// Angriffsradius in dem Schaden ausgeteilt wird.
        /// </summary>
        float AttackRange { get; }

        /// <summary>
        /// Schaden der pro Angriff verursacht wird.
        /// </summary>
        int AttackValue { get; }

        /// <summary>
        /// Gibt die Zeitspanne an, die der Character zur Erholung von einem Schlag benötigt.
        /// </summary>
        TimeSpan TotalRecovery { get; }

        /// <summary>
        /// Gibt die noch verbleibende Erholungszeit an, bevor erneut geschlagen werden kann.
        /// </summary>
        TimeSpan Recovery { get; set; }

        /// <summary>
        /// Interner Flag um bevorstehenden Angriff zu signalisieren.
        /// </summary>
        bool AttackSignal { get; set; }
    }
}

