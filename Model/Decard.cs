using System;
using RheinwerkAdventure.Screens;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert den Helfer am Brunnen.
    /// </summary>
    internal class Decard : Character, IInteractable
    {
        /// <summary>
        /// Delegat für aktiven Interaktionsversuch des Spielers.
        /// </summary>
        public Action<RheinwerkGame, IInteractor, IInteractable> OnInteract { get; set; }

        public Decard()
        {
            Texture = "decard.png";
            Name = "Decard";
            Icon = "decardicon.png";

            OnInteract = DoInteract;

            Ai = new WalkingAi(this, 0.4f);
        }

        private void DoInteract(RheinwerkGame game, IInteractor interactor, IInteractable interactable)
        {
            game.Screen.ShowScreen(new ShoutScreen(game.Screen, this, "Bleib ein Weilchen und hoer zu!"));
        }
    }
}

