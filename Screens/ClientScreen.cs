using System;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Controls;

namespace RheinwerkAdventure.Screens
{
    internal class ClientScreen : Screen
    {
        private ListItem cancelItem = new ListItem() { Text = "Abbrechen" };

        public ClientScreen(ScreenComponent manager) : base(manager, new Point(400, 300))
        {
            Controls.Add(new Panel(manager) { Position = new Rectangle(20, 20, 360, 40) });
            Controls.Add(new Label(manager) { Text = "Verbinden", Position = new Rectangle(40, 30, 0, 0) });
            Controls.Add(new LoadingIcon(manager) { Position = new Rectangle(330, 30, 32, 24) });

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

