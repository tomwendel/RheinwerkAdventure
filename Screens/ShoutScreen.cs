using System;
using RheinwerkAdventure.Components;
using RheinwerkAdventure.Model;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Controls;

namespace RheinwerkAdventure.Screens
{
    internal class ShoutScreen : Screen
    {
        private Item speaker;

        private string message;

        public ShoutScreen(ScreenComponent manager, Item speaker, string message) 
            : base(manager)
        {
            this.speaker = speaker;
            this.message = message;

            Position = new Rectangle(10, manager.GraphicsDevice.Viewport.Height - 54, manager.GraphicsDevice.Viewport.Width - 20, 44);
            Controls.Add(new Icon(manager) { Position = new Rectangle(10, 10, 24, 24), Texture = speaker.Icon });
            Controls.Add(new Label(manager) { Text = message, Position = new Rectangle(45, 12, Position.Width - 20, Position.Height - 20) });
        }

        public override void Update(GameTime gameTime)
        {
            if (!Manager.Game.Input.Handled)
            {
                if (Manager.Game.Input.Close | Manager.Game.Input.Interact)
                {
                    Manager.CloseScreen();
                    Manager.Game.Input.Handled = true;
                }
            }
        }
    }
}

