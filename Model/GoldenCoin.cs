using System;
using System.Linq;

namespace RheinwerkAdventure.Model
{
    internal class GoldenCoin : Coin
    {
        public GoldenCoin()
        {
            Mass = 0.5f;
            Texture = "coin_gold.png";
            Name = "Golden Coin";
            Icon = "coinicon.png";

            OnCollect = (game, item) => 
                {
                    var quest = game.Simulation.World.Quests.SingleOrDefault(q => q.Name == "Heidis Quest");
                    quest.Progress("return");
                };
        }
    }
}

