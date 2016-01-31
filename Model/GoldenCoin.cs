using System;
using System.Linq;

namespace RheinwerkAdventure.Model
{
    internal class GoldenCoin : Coin
    {
        public GoldenCoin(int id) : base(id)
        {
            Mass = 0.5f;
            Texture = "coin_gold.png";
            Name = "Golden Coin";
            Icon = "coinicon.png";

            OnCollect = (simulation, item) => 
                {
                    simulation.SetQuestProgress("Heidis Quest", "return");
                };
        }
    }
}

