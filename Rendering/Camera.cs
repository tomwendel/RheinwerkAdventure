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

        private Vector2 areaSize;

        /// <summary>
        /// Anzahl Pixel zum Rand bevor die Kamera mit fährt.
        /// </summary>
        public int Border { get; private set; }

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
            Border = 150;
        }

        /// <summary>
        /// Setzt den Fokus auf den gegebenen Punkt in World-Koordinaten.
        /// </summary>
        /// <param name="position">Fokuspunkt</param>
        public void SetFocusExplizit(Vector2 position, Vector2 areaSize)
        {
            this.areaSize = areaSize;
            Position = position;
            SetFocus(position);
        }

        /// <summary>
        /// Stellt sicher, dass der angegebene Punkt in World-Koordinaten sichtbar ist.
        /// </summary>
        /// <param name="position">Fokuspunkt</param>
        public void SetFocus(Vector2 position)
        {
            Vector2 viewSize = ViewSizeHalf * 2f;
            float worldBorder = Border / Scale;

            // Kamerakorrekturen auf X-Achse
            if (areaSize.X > viewSize.X)
            {
                // Position nach links verschieben, falls neue Position aus dem Bildschirmrand läuft.
                float left = position.X - Offset.X - worldBorder;
                if (left < 0f)
                    Position = new Vector2(Position.X + left, Position.Y);

                // Position nach rechts verschieben, falls neue Position aus dem Bildschirmrand läuft.
                float right = viewSize.X - position.X + Offset.X - worldBorder;
                if (right < 0f)
                    Position = new Vector2(Position.X - right, Position.Y);

                // Weiter nach rechts schieben, sollte die Position den Hintergrund frei legen.
                left = Offset.X;
                if (left < 0f)
                    Position = new Vector2(Position.X - left, Position.Y);

                // Weiter nach links schieben, sollte die Position den Hintergrund frei legen.
                right = areaSize.X - Offset.X - viewSize.X;
                if (right < 0f)
                    Position = new Vector2(Position.X + right, Position.Y);
            }
            else
            {
                // Spielfeld zu klein für korrekturen -> Zentrieren
                Position = new Vector2(areaSize.X / 2f, Position.Y);
            }
                
            if (areaSize.Y > viewSize.Y)
            {
                // Position nach oben verschieben, falls neue Position aus dem Bildschirmrand läuft.
                float top = position.Y - Offset.Y - worldBorder;
                if (top < 0f)
                    Position = new Vector2(Position.X, Position.Y + top);

                // Position nach unten verschieben, falls neue Position aus dem Bildschirmrand läuft.
                float bottom = viewSize.Y - position.Y + Offset.Y - worldBorder;
                if (bottom < 0f)
                    Position = new Vector2(Position.X, Position.Y - bottom);

                // Weiter nach unten schieben, sollte die Position den Hintergrund frei legen.
                top = Offset.Y;
                if (top < 0f)
                    Position = new Vector2(Position.X, Position.Y - top);

                // Weiter nach oben schieben, sollte die Position den Hintergrund frei legen.
                bottom = areaSize.Y - Offset.Y - viewSize.Y;
                if (bottom < 0f)
                    Position = new Vector2(Position.X, Position.Y + bottom);
            }
            else
            {
                // Spielfeld zu klein für korrekturen -> Zentrieren
                Position = new Vector2(Position.X, areaSize.Y / 2f);
            }
        }
    }
}

