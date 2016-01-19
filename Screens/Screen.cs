using System;
using RheinwerkAdventure.Components;
using System.Collections.Generic;
using RheinwerkAdventure.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RheinwerkAdventure.Screens
{
    /// <summary>
    /// Abstrakte Basisklasse für alle Screens.
    /// </summary>
    internal abstract class Screen
    {
        /// <summary>
        /// Auflistung aller enthaltener Controls.
        /// </summary>
        public List<Control> Controls { get; private set; }

        /// <summary>
        /// Position und Größe des Screens.
        /// </summary>
        public Rectangle Position { get; set; }

        /// <summary>
        /// Referenz auf den Screen Manager.
        /// </summary>
        protected ScreenComponent Manager { get; private set; }

        public Screen(ScreenComponent manager)
        {
            Manager = manager;
            Controls = new List<Control>();
        }

        public Screen(ScreenComponent manager, Point size) : this(manager)
        {
            Point pos = new Point(
                (manager.GraphicsDevice.Viewport.Width - size.X) / 2,
                (manager.GraphicsDevice.Viewport.Height - size.Y) / 2);

            Position = new Rectangle(pos, size);
        }

        /// <summary>
        /// Wird aufgerufen sobald der Screen in die Render-Auflistung kommt.
        /// </summary>
        public virtual void OnShow() {}

        /// <summary>
        /// Wird aufgerufen sobald der Screen aus der Render-Auflistung entfernt wird.
        /// </summary>
        public virtual void OnHide() {}

        public abstract void Update(GameTime gameTime);

        public void Draw(GameTime gameTime, SpriteBatch batch)
        {
            Manager.Panel.Draw(batch, Position);
            foreach (var control in Controls)
                control.Draw(batch, Position.Location);
        }
    }
}

