using System;
using System.Linq;
using RheinwerkAdventure.Components;
using RheinwerkAdventure.Controls;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Screens
{
    internal class ServerScreen : Screen
    {
        private ListItem startItem = new ListItem() { Text = "Starten" };

        private ListItem cancelItem = new ListItem() { Text = "Abbrechen" };

        private Label counter;

        public ServerScreen(ScreenComponent manager) 
            : base(manager, new Point(400, 300))
        {
            Controls.Add(new Panel(manager) { Position = new Rectangle(20, 20, 360, 40) });
            Controls.Add(new Label(manager) { Text = "Server", Position = new Rectangle(40, 30, 0, 0) });
            Controls.Add(new LoadingIcon(manager) { Position = new Rectangle(330, 30, 32, 24) });

            Controls.Add(counter = new Label(manager) { Position = new Rectangle(40, 80, 0, 0) });

            DialogButtons buttons = new DialogButtons(manager) { Position = new Rectangle(20, 300 - 70, 360, 50) };
            buttons.Items.Add(cancelItem);
            buttons.Items.Add(startItem);
            Controls.Add(buttons);

            buttons.SelectedItem = cancelItem;

            buttons.OnInteract += OnInteract;
        }

        private void OnInteract(ListItem item)
        {
            // Bei Start...
            if (item == startItem)
            {
                // Spiel starten und Fenster schließen
                Manager.Game.Server.StartGame();
                Manager.CloseScreen();
                Manager.CloseScreen();
                Manager.CloseScreen();
            }

            // Bei Abbruch
            if (item == cancelItem)
            {
                Manager.Game.Server.CloseServer();
                Manager.CloseScreen();
            }
        }

        public override void Update(GameTime gameTime)
        {
            var clients = Manager.Game.Server.Clients.Count();
            counter.Text = string.Format("Verbindungen: {0}", clients);

            if (!Manager.Game.Input.Handled)
            {
                if (Manager.Game.Input.Close)
                {
                    Manager.Game.Server.CloseServer();
                    Manager.CloseScreen();
                    Manager.Game.Input.Handled = true;
                }
            }
        }

        public override void OnShow()
        {
            Manager.Game.Server.OpenServer();
            base.OnShow();
        }
    }
}

