using System;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Model;

namespace RheinwerkAdventure.Components
{
	internal class SimulationComponent : GameComponent
	{
        private readonly RheinwerkGame game;

        public World World
        {
            get;
            private set;
        }

        public Player Player
        {
            get;
            private set;
        }

		public SimulationComponent (RheinwerkGame game) : base(game)
		{
            this.game = game;
            NewGame();
		}

        public void NewGame()
        {
            World = new World();

            Area area = new Area(30, 20);
            for (int x = 0; x < area.Width; x++)
            {
                for (int y = 0; y < area.Height; y++)
                {
                    area.Tiles[x, y] = new Tile();
                }
            }

            Player = new Player() { Position = new Vector2(15, 10), Radius = 0.25f };
            Diamant diamant = new Diamant() { Position = new Vector2(10, 10), Radius = 0.25f };

            area.Items.Add(Player);
            area.Items.Add(diamant);

            World.Areas.Add(area);
        }
	}
}

