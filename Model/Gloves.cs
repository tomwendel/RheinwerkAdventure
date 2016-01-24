using System;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Handschuhe für den Spieler die er sich niemals leisten kann.
    /// </summary>
    internal class Gloves : Item
    {
        public Gloves()
        {
            Name = "Gloves";
            Icon = "glovesicon.png";
            Value = 20;
        }
    }
}

