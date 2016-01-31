using System;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Das kleine Holzschwert das beim Kampf hilft.
    /// </summary>
    internal class WoodSword : Item
    {
        public WoodSword(int id) : base(id)
        {
            Name = "Woody";
            Icon = "woodswordicon.png";
            Value = 1;
        }
    }
}

