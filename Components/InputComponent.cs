using System;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Components
{
	internal class InputComponent : GameComponent
	{
        private readonly RheinwerkGame game;

		public InputComponent (RheinwerkGame game) : base(game)
		{
            this.game = game;
		}
	}
}

