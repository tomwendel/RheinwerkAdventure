using System;
using RheinwerkAdventure.Components;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Interface für alle Spielelemente mit denen interagiert werden kann.
    /// </summary>
    internal interface IInteractable
    {
        /// <summary>
        /// Delegat für aktiven Interaktionsversuch des Spielers.
        /// </summary>
        Action<SimulationComponent, IInteractor, IInteractable> OnInteract { get; }
    }
}

