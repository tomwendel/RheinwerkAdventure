using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RheinwerkAdventure.Components
{
	internal class InputComponent : GameComponent
	{
        private readonly RheinwerkGame game;

        public Vector2 Movement { get; private set; }

		public InputComponent (RheinwerkGame game) : base(game)
		{
            this.game = game;
		}

        public override void Update(GameTime gameTime)
        {
            Vector2 movement = Vector2.Zero;

            // Gamepad Steuerung
            GamePadState gamePad = GamePad.GetState();
            movement += gamePad.ThumbSticks.Left * new Vector2(1f, -1f);

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

            if (movement.Length() > 1f)
                movement.Normalize();

            Movement = movement;

            base.Update(gameTime);
        }
	}
}

