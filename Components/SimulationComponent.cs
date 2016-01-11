using System;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Components
{
	internal class SimulationComponent : GameComponent
	{
        private readonly RheinwerkGame game;

		public SimulationComponent (RheinwerkGame game) : base(game)
		{
            this.game = game;
		}
	}
}

