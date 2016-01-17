using System;
using RheinwerkAdventure.Model;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Rendering
{
    /// <summary>
    /// Render-Container für einzelne Spiele-Items.
    /// </summary>
    internal class ItemRenderer
    {
        /// <summary>
        /// Referenz auf das Item
        /// </summary>
        private Item item;

        /// <summary>
        /// Referenz auf die Kamera
        /// </summary>
        private Camera camera;

        /// <summary>
        /// Referenz auf die zu verwendende Textur
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// Größenangabe eines Frames in Pixel
        /// </summary>
        private Point frameSize;

        /// <summary>
        /// Anzahl Millisekunden pro Frame
        /// </summary>
        private int frameTime;

        /// <summary>
        /// Item-Mittelpunkt in Pixel
        /// </summary>
        private Point itemOffset;

        /// <summary>
        /// Skalierungsfaktor beim rendern
        /// </summary>
        private float frameScale;

        /// <summary>
        /// Anzahl Frames in der Animation
        /// </summary>
        private int frameCount;

        /// <summary>
        /// Vergangene Animationszeit in Millisekunden
        /// </summary>
        private int animationTime;

        /// <summary>
        /// Initialisierung des Item Renderers
        /// </summary>
        /// <param name="item">Item Referenz</param>
        /// <param name="camera">Kamera Referenz</param>
        /// <param name="texture">Textur Referenz</param>
        /// <param name="frameSize">Größe eines Frames in Pixel</param>
        /// <param name="frameTime">Anzahl Millisekunden pro Frame</param>
        /// <param name="frameCount">Anzahl Frames</param>
        /// <param name="itemOffset">Mittelpunkt des Items innerhalb des Frames</param>
        /// <param name="frameScale">Skalierung</param>
        public ItemRenderer(Item item, Camera camera, Texture2D texture, Point frameSize, int frameTime, int frameCount, Point itemOffset, float frameScale)
        {
            this.item = item;
            this.camera = camera;
            this.texture = texture;
            this.frameSize = frameSize;
            this.frameTime = frameTime;
            this.frameCount = frameCount;
            this.itemOffset = itemOffset;
            this.frameScale = frameScale;
        }

        public void Draw(SpriteBatch spriteBatch, Point offset, GameTime gameTime)
        {
            // Animationszeit neu berechnen (vergangene Millisekunden zum letzten Frame addieren)
            animationTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Ermittlung des aktuellen Frames
            int frame = (animationTime / frameTime) % frameCount;

            // Bestimmung der Position des Spieler-Mittelpunktes in View-Koordinaten
            int posX = (int)((item.Position.X) * camera.Scale) - offset.X;
            int posY = (int)((item.Position.Y) * camera.Scale) - offset.Y;

            // Bestimmung des Skalierungsfaktors
            Vector2 scale = new Vector2(
                (camera.Scale / frameSize.X) * frameScale,
                (camera.Scale / frameSize.Y) * frameScale);

            Rectangle sourceRectangle = new Rectangle(frame * frameSize.X, 0, frameSize.X, frameSize.Y);
            spriteBatch.Draw(texture, 
                new Rectangle(
                    (int)(posX - (itemOffset.X * frameScale)), 
                    (int)(posY - (itemOffset.Y * frameScale)), 
                    (int)(frameSize.X * scale.X), 
                    (int)(frameSize.Y * scale.Y)), 
                sourceRectangle, Color.White);
        }
    }
}

