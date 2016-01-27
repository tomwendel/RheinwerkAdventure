using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RheinwerkAdventure.Model;
using System.IO;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Game Komponente für die Anzeige des Headup Displays (Lebensanzeige,...)
    /// </summary>
    internal class HudComponent : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;

        private RheinwerkGame game;

        private SpriteFont hudFont;

        private Texture2D hearts;

        private Texture2D coin;

        public HudComponent(RheinwerkGame game) : base(game)
        {
            this.game = game;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            hudFont = Game.Content.Load<SpriteFont>("HudFont");
            hearts = Game.Content.Load<Texture2D>("hearts");
            coin = Game.Content.Load<Texture2D>("coinicon");
        }

        public override void Update(GameTime gameTime)
        {
            // Nur wenn Komponente aktiviert wurde.
            if (!Enabled)
                return;

            // Nur arbeiten, wenn es eine Welt, einen Player und eine aktive Area gibt.
            Area area = game.Local.GetCurrentArea();
            if (game.Simulation.World == null || game.Local.Player == null || area == null)
                return;
        }

        public override void Draw(GameTime gameTime)
        {
            // Nur wenn Komponente sichtbar ist.
            if (!Visible)
                return;

            // Nur arbeiten, wenn es eine Welt, einen Player und eine aktive Area gibt.
            Area area = game.Local.GetCurrentArea();
            if (game.Simulation.World == null || game.Local.Player == null || area == null)
                return;

            spriteBatch.Begin();

            Vector2 position = game.Local.Player.Position;
            string debugText = string.Format("{0} ({1:0}/{2:0})", area.Name, position.X, position.Y);

            // Ausgabe der ersten Debug-Info
            spriteBatch.DrawString(hudFont, debugText, new Vector2(10, 10), Color.White);

            // Herzen ausgeben
            int totalHearts = game.Local.Player.MaxHitpoints;
            int filledHearts = game.Local.Player.Hitpoints;
            int offset = GraphicsDevice.Viewport.Width - (totalHearts * 34) - 10;
            for (int i = 0; i < totalHearts; i++)
            {
                Rectangle source = new Rectangle(0, (filledHearts > i ? 0 : 67), 32, 32);
                Rectangle destination = new Rectangle(offset + (i * 34), 10, 32, 32);

                spriteBatch.Draw(hearts, destination, source, Color.White);
            }

            // Coins ausgeben
            string coins = game.Local.Player.Inventory.Count((i) => i is Coin).ToString();
            spriteBatch.Draw(coin, new Rectangle(GraphicsDevice.Viewport.Width - 34, 49, 24, 24), Color.White);
            int coinSize = (int)hudFont.MeasureString(coins).X;
            spriteBatch.DrawString(hudFont, coins, new Vector2(GraphicsDevice.Viewport.Width - coinSize - 35, 50), Color.White);

            // Quest anzeigen
            Quest quest = game.Simulation.World.Quests.FirstOrDefault(q => q.State != QuestState.Inactive);
            if (quest != null)
            {
                spriteBatch.DrawString(hudFont, quest.Name, new Vector2(10, 40), Color.White);
                spriteBatch.DrawString(hudFont, quest.CurrentProgress.Description, new Vector2(10, 60), Color.White);
            }

            spriteBatch.End();
        }
    }
}

