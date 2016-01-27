using System;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Controls;
using RheinwerkAdventure.Model;

namespace RheinwerkAdventure.Screens
{
    /// <summary>
    /// Das Hauptmenü
    /// </summary>
    internal class MainMenuScreen : Screen
    {
        private MenuList menu;

        private ListItem newGameItem = new ListItem() { Text = "Neues Spiel" };
        private ListItem networkItem = new ListItem() { Text = "Mehrspieler"};
        private ListItem optionsItem = new ListItem() { Text = "Optionen", Enabled = false };
        private ListItem exitItem = new ListItem() { Text = "Beenden" };

        public MainMenuScreen(ScreenComponent manager) : base(manager, new Point(400, 300))
        {
            Controls.Add(new Panel(manager) { Position = new Rectangle(20, 20, 360, 40) });
            Controls.Add(new Label(manager) { Text = "Hauptmenue", Position = new Rectangle(40, 30, 0, 0) });
            Controls.Add(menu = new MenuList(manager) { Position = new Rectangle(20, 70, 360, 200) });

            menu.Items.Add(newGameItem);
            menu.Items.Add(networkItem);
            menu.Items.Add(optionsItem);
            menu.Items.Add(exitItem);

            menu.SelectedItem = newGameItem;

            menu.OnInteract += OnInteract;
        }

        private void OnInteract(ListItem item)
        {
            if (item == newGameItem)
            {
                // Neues Spiel erstellen
                Manager.Game.Simulation.NewGame();
                Manager.Game.Local.Player = new Player();
                Manager.Game.Simulation.InsertPlayer(Manager.Game.Local.Player);
                Manager.CloseScreen();
            }

            if (item == networkItem)
            {
                // Netzwerk-Spiel
                Manager.ShowScreen(new NetworkScreen(Manager));
            }

            if (item == exitItem)
            {
                // Anwendung beenden
                Manager.Game.Exit();
                Manager.CloseScreen();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Manager.Game.Input.Handled)
            {
                if (Manager.Game.Input.Close && Manager.Game.Simulation.World != null)
                {
                    Manager.CloseScreen();
                    Manager.Game.Input.Handled = true;
                }
            }
        }

        public override void OnShow()
        {
            Manager.Game.Music.OpenMenu();
        }

        public override void OnHide()
        {
            Manager.Game.Music.CloseMenu();
        }
    }
}

