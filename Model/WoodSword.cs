using System;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Das kleine Holzschwert das beim Kampf hilft.
    /// </summary>
    internal class WoodSword : Item
    {
        public WoodSword()
        {
            Name = "Woody";
            Icon = "woodswordicon.png";
            Value = 1;
        }
    }
}

