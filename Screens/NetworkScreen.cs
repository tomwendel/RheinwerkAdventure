using System;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Controls;

namespace RheinwerkAdventure.Screens
{
    /// <summary>
    /// Netzwerk-Screen
    /// </summary>
    internal class NetworkScreen : Screen
    {
        private ListItem serverItem = new ListItem() { Text = "Server starten" };

        private ListItem clientItem = new ListItem() { Text = "Verbinden" };

        private ListItem cancelItem = new ListItem() { Text = "Abbrechen" };

        private MenuList menu;

        public NetworkScreen(ScreenComponent manager) : base(manager, new Point(400, 300))
        {
            Controls.Add(new Panel(manager) { Position = new Rectangle(20, 20, 360, 40) });
            Controls.Add(new Label(manager) { Text = "Netzwerk", Position = new Rectangle(40, 30, 0, 0) });
            Controls.Add(menu = new MenuList(manager) { Position = new Rectangle(20, 70, 360, 200) });

            menu.Items.Add(serverItem);
            menu.Items.Add(clientItem);
            menu.Items.Add(cancelItem);

            menu.SelectedItem = serverItem;

            menu.OnInteract += OnInteract;
        }

        private void OnInteract(ListItem item)
        {
            // Bei Server-Auswahl
            if (item == serverItem)
                Manager.ShowScreen(new ServerScreen(Manager));

            // Client Auswahl
            else if (item == clientItem)
                Manager.ShowScreen(new ClientScreen(Manager));

            // Cancel-Knopf gedrückt
            else if (item == cancelItem)
                Manager.CloseScreen();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Manager.Game.Input.Handled)
            {
                if (Manager.Game.Input.Close)
                {
                    Manager.CloseScreen();
                    Manager.Game.Input.Handled = true;
                }
            }
        }
    }
}

