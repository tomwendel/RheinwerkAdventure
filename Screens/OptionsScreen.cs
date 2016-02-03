using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Components;
using RheinwerkAdventure.Controls;

namespace RheinwerkAdventure.Screens
{
    internal class OptionsScreen : Screen
    {
        private MenuList menu;

        private ListItem musicVolumeItem = new ListItem() { Text = "Music" };

        private ListItem soundVolumeItem = new ListItem() { Text = "Sounds" };

        public OptionsScreen(ScreenComponent manager) :
            base(manager, new Point(400, 360))
        {
            Controls.Add(new Panel(manager) { Position = new Rectangle(20, 20, 360, 40) });
            Controls.Add(new Label(manager) { Text = "Optionen", Position = new Rectangle(40, 30, 0, 0) });
            Controls.Add(menu = new MenuList(manager) { Position = new Rectangle(20, 70, 360, 200) });

            menu.Items.Add(musicVolumeItem);
            menu.Items.Add(soundVolumeItem);
            menu.SelectedItem = menu.Items.First();
            RefreshText();
        }

        private void RefreshText()
        {
            musicVolumeItem.Text = string.Format("Music ({0}/{1})", Manager.Game.Settings.MusicVolume, 10);
            soundVolumeItem.Text = string.Format("Sound ({0}/{1})", Manager.Game.Settings.SoundVolume, 10);

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

                // Links
                if (Manager.Game.Input.Left)
                {
                    if (menu.SelectedItem == musicVolumeItem)
                    {
                        Manager.Game.Input.Handled = true;
                        Manager.Game.Settings.MusicVolume--;
                        RefreshText();
                    }

                    if (menu.SelectedItem == soundVolumeItem)
                    {
                        Manager.Game.Input.Handled = true;
                        Manager.Game.Settings.SoundVolume--;
                        RefreshText();
                    }
                }

                // Rechts
                if (Manager.Game.Input.Right)
                {
                    if (menu.SelectedItem == musicVolumeItem)
                    {
                        Manager.Game.Input.Handled = true;
                        Manager.Game.Settings.MusicVolume++;
                        RefreshText();
                    }

                    if (menu.SelectedItem == soundVolumeItem)
                    {
                        Manager.Game.Input.Handled = true;
                        Manager.Game.Settings.SoundVolume++;
                        RefreshText();
                    }
                }
            }
        }
    }
}
