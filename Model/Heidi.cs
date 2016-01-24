using System;
using RheinwerkAdventure.Screens;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert die Questgeberin Heidi.
    /// </summary>
    internal class Heidi : Character, IInteractable
    {
        private Dialog dialog;

        /// <summary>
        /// Delegat für aktiven Interaktionsversuch des Spielers.
        /// </summary>
        public Action<RheinwerkGame, IInteractor, IInteractable> OnInteract { get; set; }

        public Heidi()
        {
            Texture = "heidi.png";
            Name = "Heidi";
            Icon = "heidiicon.png";

            OnInteract = DoInteract;

            // Einstiegssatz
            dialog = new Dialog()
                {
                    Message = "Hallo junger Held.",
                    CanExit = true,
                };

            // Option 1
            dialog.Options.Add(new Dialog() { 
                Option = "Erzaehl mal was ueber das Dorf",
                Message = "Hier war alles friedlich bis zuletzt. Jetzt haben wir Orcs hier.",
                Back = dialog,
            });

            // Option 2
            dialog.Options.Add(new Dialog() {
                Option = "Kann ich was tun?",
                Message = "Ja bitte. Mir wurde meine goldene Muenze geraubt.",
                Back = dialog,
            });
        }

        private void DoInteract(RheinwerkGame game, IInteractor interactor, IInteractable interactable)
        {
            game.Screen.ShowScreen(new DialogScreen(game.Screen, this, dialog));
        }
    }
}

