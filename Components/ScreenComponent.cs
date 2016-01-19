using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using RheinwerkAdventure.Screens;
using Microsoft.Xna.Framework.Graphics;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Komponente zur Verwaltung von Screen-Overlays.
    /// </summary>
    internal class ScreenComponent : DrawableGameComponent
    {
        private readonly Stack<Screen> screens;

        private readonly RheinwerkGame game;

        private SpriteBatch spriteBatch;

        #region Shared Resources

        /// <summary>
        /// Ein einzelner Pixel
        /// </summary>
        public Texture2D Pixel { get; private set; }

        /// <summary>
        /// Standard-Schriftart für Dialoge
        /// </summary>
        public SpriteFont Font { get; private set; }

        #endregion

        /// <summary>
        /// Liefert den aktuellen Screen oder null zurück.
        /// </summary>
        public Screen ActiveScreen
        {
            get { return screens.Count > 0 ? screens.Peek() : null; }
        }

        public ScreenComponent(RheinwerkGame game)
            : base(game)
        {
            this.game = game;
            screens = new Stack<Screen>();
        }

        /// <summary>
        /// Zeigt den übergebenen Screen an.
        /// </summary>
        public void ShowScreen(Screen screen)
        {
            screens.Push(screen);
        }

        /// <summary>
        /// Entfernt den obersten Screen.
        /// </summary>
        public void CloseScreen()
        {
            if (screens.Count > 0)
                screens.Pop();
        }

        protected override void LoadContent()
        {
            // Sprite Batch erstellen
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Standard Pixel erstellen
            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new [] { Color.White });

            // Schriftart laden
            Font = game.Content.Load<SpriteFont>("HudFont");

            ShowScreen(new MainMenuScreen(this));
        }

        public override void Update(GameTime gameTime)
        {
            Screen activeScreen = ActiveScreen;
            if (activeScreen != null)
            {
                foreach (var control in activeScreen.Controls)
                    control.Update(gameTime);
                activeScreen.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            foreach (var screen in screens)
                screen.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}

