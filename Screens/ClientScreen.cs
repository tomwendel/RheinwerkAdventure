using System;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Controls;

namespace RheinwerkAdventure.Screens
{
    internal class ClientScreen : Screen
    {
        private ListItem cancelItem = new ListItem() { Text = "Abbrechen" };

        private Label status;

        public ClientScreen(ScreenComponent manager) : base(manager, new Point(400, 300))
        {
            Controls.Add(new Panel(manager) { Position = new Rectangle(20, 20, 360, 40) });
            Controls.Add(new Label(manager) { Text = "Verbinden", Position = new Rectangle(40, 30, 0, 0) });
            Controls.Add(new LoadingIcon(manager) { Position = new Rectangle(330, 30, 32, 24) });

            Controls.Add(status = new Label(manager) { Position = new Rectangle(40, 80, 0, 0) });

            DialogButtons buttons = new DialogButtons(manager) { Position = new Rectangle(20, 300 - 70, 360, 50) };
            buttons.Items.Add(cancelItem);
            Controls.Add(buttons);

            buttons.SelectedItem = cancelItem;

            buttons.OnInteract += OnInteract;
        }

        private void OnInteract(ListItem item)
        {
            // Bei Abbruch
            if (item == cancelItem)
            {
                Manager.Game.Client.Close();
                Manager.CloseScreen();
            }
        }

        public override void Update(GameTime gameTime)
        {
            status.Text = string.Format("Status: {0}, Id: {1}, Player: {2}", 
                Manager.Game.Client.State, 
                Manager.Game.Client.ClientId, 
                Manager.Game.Client.PlayerCount);

            // Wenns los geht den Screen schließen
            if (Manager.Game.Client.State == ClientState.Running)
            {
                Manager.CloseScreen();
                Manager.CloseScreen();
                Manager.CloseScreen();
            }

            if (!Manager.Game.Input.Handled)
            {
                if (Manager.Game.Input.Close)
                {
                    Manager.Game.Client.Close();
                    Manager.CloseScreen();
                    Manager.Game.Input.Handled = true;
                }
            }
        }

        public override void OnShow()
        {
            Manager.Game.Client.Connect();            
            base.OnShow();
        }
    }
}

