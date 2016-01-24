using System;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Ein Eisenschwert das sich der Spieler niemals leisten kann.
    /// </summary>
    internal class IronSword : Item
    {
        public IronSword()
        {
            Name = "Irony";
            Icon = "ironswordicon.png";
            Value = 10;
        }
    }
}

