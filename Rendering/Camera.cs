using System;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Rendering
{
    /// <summary>
    /// Repräsentiert die Kamera auf die Szene
    /// </summary>
    internal class Camera
    {
        private Vector2 viewSizeHalf;

        /// <summary>
        /// Position des Zentrums in World-Koordinaten.
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// Multiplikator für World -> View Koordinaten.
        /// </summary>
        public float Scale { get; private set; }

        /// <summary>
        /// Der Render-Versatz in World-Koordinaten.
        /// </summary>
        public Vector2 Offset { get { return Position - ViewSizeHalf; } }

        /// <summary>
        /// Halbe Bildschirmgröße in World-Koordinaten.
        /// </summary>
        public Vector2 ViewSizeHalf { get { return viewSizeHalf / Scale; } }

        /// <summary>
        /// Kamera-Konstruktor.
        /// </summary>
        /// <param name="viewSize">Größe des Viewports in Pixel</param>
        public Camera(Point viewSize)
        {
            viewSizeHalf = new Vector2(viewSize.X / 2f, viewSize.Y / 2f);
            Scale = 64f;
        }

        /// <summary>
        /// Setzt den Fokus auf den gegebenen Punkt in World-Koordinaten.
        /// </summary>
        /// <param name="position">Fokuspunkt</param>
        public void SetFocus(Vector2 position)
        {
            Position = position;
        }
    }
}

