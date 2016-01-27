using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Game Komponente zur Verarbeitung von Spieler-Eingaben
    /// </summary>
    internal class InputComponent : GameComponent
    {
        private readonly RheinwerkGame game;
        private readonly Trigger upTrigger;
        private readonly Trigger downTrigger;
        private readonly Trigger leftTrigger;
        private readonly Trigger rightTrigger;
        private readonly Trigger closeTrigger;
        private readonly Trigger interactTrigger;
        private readonly Trigger attackTrigger;
        private readonly Trigger inventoryTrigger;

        /// <summary>
        /// Gibt an ob die Eingaben innerhalb dieses Update-Zykluses bereits abgearbeitet wurden.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gibt an ob der User nach oben gedrückt hat.
        /// </summary>
        public bool Up { get { return upTrigger.Value; } }

        /// <summary>
        /// Gibt an ob der User nach unten gedrückt hat.
        /// </summary>
        public bool Down { get { return downTrigger.Value; } }

        /// <summary>
        /// Gibt an ob der User nach links gedrückt hat.
        /// </summary>
        public bool Left { get { return leftTrigger.Value; } }

        /// <summary>
        /// Gibt an ob der User nach rechts gedrückt hat.
        /// </summary>
        public bool Right { get { return rightTrigger.Value; } }

        /// <summary>
        /// Gibt die vom Spieler gewünschte Bewegungsrichtung (normalisiert) an.
        /// </summary>
        public Vector2 Movement { get; private set; }

        /// <summary>
        /// Gibt an ob der User den Close-Button drückt.
        /// </summary>
        public bool Close { get { return closeTrigger.Value; } }

        /// <summary>
        /// Gibt an ob der User den Inventar-Button drückt.
        /// </summary>
        public bool Inventory { get { return inventoryTrigger.Value; } }

        /// <summary>
        /// Gibt an ob der User den Attack-Knopf drückt.
        /// </summary>
        public bool Attack { get { return attackTrigger.Value; } }

        /// <summary>
        /// Gibt an ob der User den Interact-Knopf drückt.
        /// </summary>
        public bool Interact { get { return interactTrigger.Value; } }

        public InputComponent(RheinwerkGame game)
            : base(game)
        {
            this.game = game;

            upTrigger = new Trigger();
            downTrigger = new Trigger();
            leftTrigger = new Trigger();
            rightTrigger = new Trigger();
            closeTrigger = new Trigger();
            inventoryTrigger = new Trigger();
            attackTrigger = new Trigger();
            interactTrigger = new Trigger();
        }

        public override void Update(GameTime gameTime)
        {
            // Nur wenn Komponente aktiviert wurde.
            if (!Enabled)
                return;
            
            Vector2 movement = Vector2.Zero;
            bool up = false;
            bool down = false;
            bool left = false;
            bool right = false;
            bool close = false;
            bool inventory = false;
            bool attack = false;
            bool interact = false;

            // Gamepad Steuerung
            try
            {
                GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
                movement += gamePad.ThumbSticks.Left * new Vector2(1f, -1f);
                left |= gamePad.ThumbSticks.Left.X < -0.5f;
                right |= gamePad.ThumbSticks.Left.X > 0.5f;
                up |= gamePad.ThumbSticks.Left.Y > 0.5f;
                down |= gamePad.ThumbSticks.Left.Y < -0.5f;
                close |= gamePad.Buttons.B == ButtonState.Pressed;
                inventory |= gamePad.Buttons.Y == ButtonState.Pressed;
                attack |= gamePad.Buttons.X == ButtonState.Pressed;
                interact |= gamePad.Buttons.A == ButtonState.Pressed;
            }
            catch
            {
            }

            // Keyboard Steuerung
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Left))
                movement += new Vector2(-1f, 0f);
            if (keyboard.IsKeyDown(Keys.Right))
                movement += new Vector2(1f, 0f);
            if (keyboard.IsKeyDown(Keys.Up))
                movement += new Vector2(0f, -1f);
            if (keyboard.IsKeyDown(Keys.Down))
                movement += new Vector2(0f, 1f);
            left |= keyboard.IsKeyDown(Keys.Left);
            right |= keyboard.IsKeyDown(Keys.Right);
            up |= keyboard.IsKeyDown(Keys.Up);
            down |= keyboard.IsKeyDown(Keys.Down);
            close |= keyboard.IsKeyDown(Keys.Escape);
            inventory |= keyboard.IsKeyDown(Keys.I);
            attack |= keyboard.IsKeyDown(Keys.LeftControl);
            interact |= keyboard.IsKeyDown(Keys.Space) | keyboard.IsKeyDown(Keys.Enter);

            // Normalisierung der Bewegungsrichtung
            if (movement.Length() > 1f)
                movement.Normalize();

            // Properties setzen
            Movement = movement;
            upTrigger.Value = up;
            downTrigger.Value = down;
            leftTrigger.Value = left;
            rightTrigger.Value = right;
            closeTrigger.Value = close;
            inventoryTrigger.Value = inventory;
            interactTrigger.Value = interact;
            attackTrigger.Value = attack;

            // Handle-Flag zurück setzen.
            Handled = false;
        }

        /// <summary>
        /// Kapselt den Druck auf einen Button und sorgt dafür, 
        /// dass ein true nur ein einziges mal abgerufen werden kann.
        /// </summary>
        private class Trigger
        {
            // Speichert den letzten Wert aus set
            private bool lastValue = false;

            // Speichert, ob der Trigger ausgelöst wurde.
            private bool triggered = false;

            /// <summary>
            /// Liefert den Wert eines ausgelösten bool zurück.
            /// </summary>
            public bool Value
            {
                get
                {
                    // Gibt den Wert des Triggers aus und setzt den Auslöser auf false zurück.
                    bool result = triggered;
                    triggered = false;
                    return result;
                }
                set
                {
                    // Schaut, ob der Trigger neu gesetzt werden darf.
                    if (lastValue != value)
                        lastValue = triggered = value;
                }
            }
        }
    }


}

