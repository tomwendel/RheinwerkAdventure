using System;
using System.Linq;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Controls;
using RheinwerkAdventure.Model;

namespace RheinwerkAdventure.Screens
{
    /// <summary>
    /// Dialog-Screen 
    /// </summary>
    internal class DialogScreen : Screen
    {
        private Item speaker;

        private Player player;

        private Dialog current;

        private Label message;

        private DialogList list;

        public DialogScreen(ScreenComponent manager, Item speaker, Player player, Dialog entry) 
            : base(manager)
        {
            this.speaker = speaker;
            this.player = player;
            current = entry;

            Controls.Add(new Icon(manager) { Position = new Rectangle(10, 10, 24, 24), Texture = speaker.Icon });
            Controls.Add(message = new Label(manager) { Position = new Rectangle(40, 10, manager.GraphicsDevice.Viewport.Width - 50, 30) });
            Controls.Add(list = new DialogList(manager) { });

            list.OnInteract += OnInteract;

            Refill();
        }

        /// <summary>
        /// Rekonfiguriert den Dialog auf Basis des aktuellen Dialog-Schritts.
        /// </summary>
        private void Refill()
        {
            // OnShow-Action ausführen
            if (current.OnShow != null)
                current.OnShow(Manager.Game, player);

            message.Text = current.Message;

            list.SelectedItem = null;
            list.Items.Clear();
            int height = 0;
            foreach (var entry in current.Options.Where(o => o.Visible))
            {
                list.Items.Add(new ListItem() { Text = entry.Option, Tag = entry });
                height += 30;
            }

            // Optionaler Back-Button
            if (current.Back != null)
            {
                list.Items.Add(new ListItem() { Text = "Zurueck", Tag = current.Back });
                height += 30;
            }

            // Optionaler Beenden-Button
            if (current.CanExit)
            {
                list.Items.Add(new ListItem() { Text = "Beenden" });
                height += 30;
            }

            // Ersten Eintrag selektieren
            if (list.Items.Count > 0)
                list.SelectedItem = list.Items[0];

            Position = new Rectangle(10, Manager.GraphicsDevice.Viewport.Height - height - 70, Manager.GraphicsDevice.Viewport.Width - 20, height + 60);
            list.Position = new Rectangle(15, 50, Position.Width - 30, height);
        }

        private void OnInteract(ListItem item)
        {
            Dialog dialog = item.Tag as Dialog;
            if (dialog != null)
            {
                // Auswahl einer Dialog-Option
                current = dialog;
                Refill();
            }
            else
            {
                // Beenden-Eitnrag ausgewählt
                Manager.CloseScreen();
            }
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}

