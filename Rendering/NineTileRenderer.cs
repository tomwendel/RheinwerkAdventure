using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Rendering
{
    internal class NineTileRenderer
    {
        private Texture2D[] textures;

        public NineTileRenderer(Texture2D texture, Rectangle source, Point cuts)
        {
            textures = new Texture2D[9];

            for (int y = 0; y < 3; y++)
            {
                // Offsets für Y berechnen
                int offsetY = source.Y;
                int height = cuts.Y;
                switch (y)
                {
                    case 1:
                        offsetY = source.Y + cuts.Y;
                        height = source.Height - cuts.Y - cuts.Y;
                        break;
                    case 2:
                        offsetY = source.Bottom - cuts.Y;
                        break;
                }

                for (int x = 0; x < 3; x++)
                {
                    // Offsets für X bereichnen
                    int offsetX = source.X;
                    int width = cuts.X;
                    switch (x)
                    {
                        case 1:
                            offsetX = source.X + cuts.X;
                            width = source.Width - cuts.X - cuts.X;
                            break;
                        case 2:
                            offsetX = source.Right - cuts.X;
                            break;
                    }

                    // Texturen erzeigen
                    Texture2D tex = new Texture2D(texture.GraphicsDevice, width, height);
                    Color[] buffer = new Color[width * height];
                    texture.GetData(0, new Rectangle(offsetX, offsetY, width, height), buffer, 0, buffer.Length);
                    tex.SetData(buffer);
                    textures[(y * 3) + x] = tex;
                }
            }
        }

        /// <summary>
        /// Zeichnet die Textur an die angegebene Stelle.
        /// </summary>
        /// <param name="batch">zu nutzender SpriteBatch</param>
        /// <param name="target">Zielrechteck</param>
        public void Draw(SpriteBatch batch, Rectangle target)
        {
            Draw(batch, target, 1f);
        }

        /// <summary>
        /// Zeichnet die Textur an die angegebene Stelle.
        /// </summary>
        /// <param name="batch">zu nutzender SpriteBatch</param>
        /// <param name="target">Zielrechteck</param>
        /// <param name="alpha"></param>
        public void Draw(SpriteBatch batch, Rectangle target, float alpha)
        {
            int cutY = textures[0].Height;
            int cutX = textures[0].Width;

            for (int y = 0; y < 3; y++)
            {
                // Offsets für Y berechnen
                int offsetY = target.Y;
                int height = cutY;
                switch (y)
                {
                    case 1:
                        offsetY = target.Y + cutY;
                        height = target.Height - cutY - cutY;
                        break;
                    case 2:
                        offsetY = target.Bottom - cutY;
                        break;
                }

                for (int x = 0; x < 3; x++)
                {
                    // Offsets für X bereichnen
                    int offsetX = target.X;
                    int width = cutX;
                    switch (x)
                    {
                        case 1:
                            offsetX = target.X + cutX;
                            width = target.Width - cutX - cutX;
                            break;
                        case 2:
                            offsetX = target.Right - cutX;
                            break;
                    }

                    // Zeichnen
                    batch.Draw(textures[(y * 3) + x],
                        new Rectangle(offsetX, offsetY, width, height),
                        new Rectangle(0, 0, width, height), Color.White * alpha);
                }
            }
        }
    }
}

