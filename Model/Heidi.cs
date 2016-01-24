using System;
using System.Linq;
using RheinwerkAdventure.Screens;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert die Questgeberin Heidi.
    /// </summary>
    internal class Heidi : Character, IInteractable
    {
        private Dialog dialog;

        private Dialog before;

        private Dialog after;

        private Quest quest;

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
            dialog.Options.Add(new Dialog()
                { 
                    Option = "Erzaehl mal was ueber das Dorf",
                    Message = "Hier war alles friedlich bis zuletzt. Jetzt haben wir Orcs hier.",
                    Back = dialog,
                });

            // Option 2 (vor dem Quest)
            dialog.Options.Add(before = new Dialog()
                {
                    Option = "Kann ich was tun?",
                    Message = "Ja bitte. Mir wurde meine goldene Muenze geraubt.",
                    Back = dialog,
                    OnShow = (game, item) =>
                    {
                        // Questfortschritt setzen
                        quest.Progress("search");
                        before.Visible = false;
                    }
                });

            dialog.Options.Add(after = new Dialog()
                {
                    Option = "Hier ist deine Muenze",
                    Message = "Wow! Ich und das Dorf werden dir das nicht vergessen! *schmatz*",
                    Back = dialog,
                    OnShow = (game, item) =>
                    {
                        // Questgegenstand entfernen
                        Player player = item as Player;
                        var coin = player.Inventory.SingleOrDefault(i => i is GoldenCoin);
                        if (coin != null)
                            player.Inventory.Remove(coin);

                        // Quest Fortschritt auf Success 
                        quest.Success("success");
                        after.Visible = false;
                    }
                });
        }

        private void DoInteract(RheinwerkGame game, IInteractor interactor, IInteractable interactable)
        {
            quest = game.Simulation.World.Quests.SingleOrDefault(q => q.Name == "Heidis Quest");
            before.Visible = quest.State == QuestState.Inactive;
            after.Visible = quest.State == QuestState.Active && quest.CurrentProgress.Id == "return";
            game.Screen.ShowScreen(new DialogScreen(game.Screen, this, interactor as Player, dialog));
        }
    }
}

