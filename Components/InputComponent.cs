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

        /// <summary>
        /// Gibt an ob die Eingaben innerhalb dieses Update-Zykluses bereits abgearbeitet wurden.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gibt die vom Spieler gewünschte Bewegungsrichtung (normalisiert) an.
        /// </summary>
        public Vector2 Movement { get; private set; }

        /// <summary>
        /// Gibt an ob der User den Close-Button drückt.
        /// </summary>
        public bool Close { get; private set; }

        /// <summary>
        /// Gibt an ob der User den Inventar-Button drückt.
        /// </summary>
        public bool Inventory { get; private set; }

        /// <summary>
        /// Gibt an ob der User den Attack-Knopf drückt.
        /// </summary>
        public bool Attack { get; private set; }

        /// <summary>
        /// Gibt an ob der User den Interact-Knopf drückt.
        /// </summary>
        public bool Interact { get; private set; }

        public InputComponent(RheinwerkGame game)
            : base(game)
        {
            this.game = game;
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 movement = Vector2.Zero;
            bool close = false;
            bool inventory = false;
            bool attack = false;
            bool interact = false;

            // Gamepad Steuerung
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
            movement += gamePad.ThumbSticks.Left * new Vector2(1f, -1f);
            close |= gamePad.Buttons.B == ButtonState.Pressed;
            inventory |= gamePad.Buttons.Y == ButtonState.Pressed;
            attack |= gamePad.Buttons.X == ButtonState.Pressed;
            interact |= gamePad.Buttons.A == ButtonState.Pressed;

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
            close |= keyboard.IsKeyDown(Keys.Escape);
            inventory |= keyboard.IsKeyDown(Keys.I);
            attack |= keyboard.IsKeyDown(Keys.LeftControl);
            interact |= keyboard.IsKeyDown(Keys.Space);

            // Normalisierung der Bewegungsrichtung
            if (movement.Length() > 1f)
                movement.Normalize();

            // Properties setzen
            Movement = movement;
            Close = close;
            Inventory = inventory;
            Interact = interact;
            Attack = attack;

            // Handle-Flag zurück setzen.
            Handled = false;
        }
    }
}

