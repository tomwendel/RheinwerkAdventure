using System;
using RheinwerkAdventure.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RheinwerkAdventure.Controls
{
    /// <summary>
    /// Abstrakte Basisklasse für alle Steuerelemente.
    /// </summary>
    internal abstract class Control
    {
        /// <summary>
        /// Referenz auf den Screen Manager.
        /// </summary>
        protected ScreenComponent Manager { get; private set; }

        /// <summary>
        /// Position des Steuerelementes relativ zum Screen.
        /// </summary>
        public Rectangle Position { get; set; }

        public Control(ScreenComponent manager)
        {
            Manager = manager;
        }

        public virtual void Update(GameTime gameTime) { }

        public abstract void Draw(SpriteBatch spriteBatch, Point offset);
    }
}

