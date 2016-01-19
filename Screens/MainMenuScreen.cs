using System;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Controls;

namespace RheinwerkAdventure.Screens
{
    internal class MainMenuScreen : Screen
    {
        public MainMenuScreen(ScreenComponent manager) : base(manager, new Point(400, 300))
        {
            Controls.Add(new Label(manager) { Text = "Hallo", Position = new Rectangle(10, 10, 0, 0) });
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}

