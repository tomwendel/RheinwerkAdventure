using System;
using RheinwerkAdventure.Model;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RheinwerkAdventure.Rendering
{
    /// <summary>
    /// Item Renderer für die komplexeren Character-SpriteSheets.
    /// </summary>
    internal class CharacterRenderer : ItemRenderer
    {
        private Character character;

        private Animation animation;

        private Direction direction;

        private int frameCount;

        private int animationRow;

        public CharacterRenderer(Character character, Camera camera, Texture2D texture, SpriteFont font)
            : base(character, camera, texture, font, new Point(64, 64), 50, new Point(32, 55), 2f)
        {
            this.character = character;
            animation = Animation.Idle;
            direction = Direction.South;
            frameCount = 1;
            animationRow = 8;
        }

        /// <summary>
        /// Render-Methode für dieses Item.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch Referenz</param>
        /// <param name="offset">Der Offset der View</param>
        /// <param name="gameTime">Aktuelle Game Time</param>
        /// <param name="highlight">Soll das Item hervorgehoben werden?</param> 
        public override void Draw(SpriteBatch spriteBatch, Point offset, GameTime gameTime, bool highlight)
        {
            // kommende Animation ermitteln
            Animation nextAnimation = Animation.Idle;
            if (character.Velocity.Length() > 0f)
            {
                nextAnimation = Animation.Walk;

                // Diagonale durch das Koordinatensystem
                // X>Y => rechte, obere Hälfte
                // -X>Y => linke, obere Hälfte

                // Spieler in Bewegung -> Ausrichtung des Spielers ermitteln
                if (character.Velocity.X > character.Velocity.Y)
                {
                    // Rechts oben
                    if (-character.Velocity.X > character.Velocity.Y)
                        // Links oben -> Oben
                        direction = Direction.North;
                    else
                        // Rechts unten -> Rechts
                        direction = Direction.East;
                }
                else
                {
                    // Links unten
                    if (-character.Velocity.X > character.Velocity.Y)
                        // Links oben -> links
                        direction = Direction.West;
                    else
                        // Rechts unten -> Unten
                        direction = Direction.South;
                }
            }

            // Schlag-Animation
            if (character is IAttacker && (character as IAttacker).Recovery > TimeSpan.Zero)
            {
                nextAnimation = Animation.Hit;
            }

            // Für den Fall, dass dieser Character Tod ist
            if (character is IAttackable && (character as IAttackable).Hitpoints <= 0)
            {
                nextAnimation = Animation.Die;
                direction = Direction.North;
            }

            // Animation bei Änderung resetten
            if (animation != nextAnimation)
            {
                animation = nextAnimation;
                AnimationTime = 0;
                switch (animation)
                {
                    case Animation.Walk:
                        frameCount = 9;
                        animationRow = 8;
                        break;
                    case Animation.Die:
                        frameCount = 6;
                        animationRow = 20;
                        break;
                    case Animation.Idle:
                        frameCount = 1;
                        animationRow = 8;
                        break;
                    case Animation.Hit:
                        frameCount = 6;
                        animationRow = 12;
                        break;
                }
            }

            // Animationszeile ermitteln
            int row = animationRow + (int)direction;
            if (animation == Animation.Die)
                row = animationRow;

            // Frame ermitteln
            int frame = 0; // Standard für Idle
            switch (animation)
            {
                case Animation.Walk:
                    // Animationsgeschwindigkeit an Laufgeschwindigkeit gekoppelt
                    float speed = character.Velocity.Length() / character.MaxSpeed;
                    AnimationTime += (int)(gameTime.ElapsedGameTime.TotalMilliseconds * speed);
                    frame = (AnimationTime / FrameTime) % frameCount;
                    break;
                case Animation.Hit:
                    // TODO: Animationsverlauf definieren
                    IAttacker attacker = Item as IAttacker;
                    double animationPosition = 1d - (attacker.Recovery.TotalMilliseconds / attacker.TotalRecovery.TotalMilliseconds);
                    frame = (int)(frameCount * animationPosition);
                    break;
                case Animation.Die:
                    // Animation stoppt mit dem letzten Frame
                    AnimationTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds / 2;
                    frame = Math.Min((AnimationTime / FrameTime), frameCount - 1);
                    break;
            }

            // Bestimmung der Position des Spieler-Mittelpunktes in View-Koordinaten
            int posX = (int)((Item.Position.X) * Camera.Scale) - offset.X;
            int posY = (int)((Item.Position.Y) * Camera.Scale) - offset.Y;
            int radius = (int)(Item.Radius * Camera.Scale);

            Vector2 scale = new Vector2(Camera.Scale / FrameSize.X, Camera.Scale / FrameSize.Y) * FrameScale;

            Rectangle sourceRectangle = new Rectangle(
                frame * FrameSize.X,
                row * FrameSize.Y, 
                FrameSize.X, 
                FrameSize.Y);

            Rectangle destinationRectangle = new Rectangle(
                posX - (int)(ItemOffset.X * scale.X), 
                posY - (int)(ItemOffset.Y * scale.Y),
                (int)(FrameSize.X * scale.X), 
                (int)(FrameSize.Y * scale.Y));

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);

            // Highlight
            if (highlight && !string.IsNullOrEmpty(Item.Name))
            {
                Vector2 textSize = Font.MeasureString(Item.Name);
                Vector2 location = new Vector2(
                    posX - (int)(textSize.X / 2), 
                    posY - (int)(ItemOffset.Y * scale.Y));
                spriteBatch.DrawString(Font, Item.Name, location, Color.White);
            }
        }
    }

    /// <summary>
    /// Auflistung möglicher Animationen
    /// </summary>
    internal enum Animation
    {
        Idle,
        Walk,
        Hit,
        Die
    }

    /// <summary>
    /// Auflistung von Blickrichtungen
    /// </summary>
    internal enum Direction
    {
        North = 0,
        West = 1,
        South = 2,
        East = 3
    }
}

