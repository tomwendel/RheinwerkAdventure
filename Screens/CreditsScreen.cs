using Microsoft.Xna.Framework;
using RheinwerkAdventure.Components;
using RheinwerkAdventure.Controls;

namespace RheinwerkAdventure.Screens
{
    internal class CreditsScreen : Screen
    {
        public CreditsScreen(ScreenComponent manager) 
            : base(manager, new Point(400, 360))
        {
            Controls.Add(new Panel(manager) { Position = new Rectangle(20, 20, 360, 40) });
            Controls.Add(new Label(manager) { Text = "Credits", Position = new Rectangle(40, 30, 0, 0) });

            Controls.Add(new Label(manager) { Text = "Vielen Dank an", Position = new Rectangle(40, 70, 0, 0) });
            Controls.Add(new Label(manager) { Text = "Hyptosis, Lost Garden, Zabin", Position = new Rectangle(50, 100, 0, 0) });
            Controls.Add(new Label(manager) { Text = "und Client Bellanger auf", Position = new Rectangle(50, 125, 0, 0) });
            Controls.Add(new Label(manager) { Text = "opengameart.org", Position = new Rectangle(50, 150, 0, 0) });

            Controls.Add(new Label(manager) { Text = "Mike Koenig, Mark DiAngelo und", Position = new Rectangle(50, 190, 0, 0) });
            Controls.Add(new Label(manager) { Text = "Luke.RUSTLTD fuer die Sounds", Position = new Rectangle(50, 215, 0, 0) });

            Controls.Add(new Label(manager) { Text = "Music Loop fuer die Musik", Position = new Rectangle(50, 255, 0, 0) });
            Controls.Add(new Label(manager) { Text = "Special Thx to kenney.nl", Position = new Rectangle(50, 295, 0, 0) });
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
