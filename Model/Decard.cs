using System;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Repräsentiert den Helfer am Brunnen.
    /// </summary>
    internal class Decard : Character, IInteractable
    {
        public Action<Player> OnInteract { get; set; }

        public Decard()
        {
            Texture = "decard.png";
            Name = "Decard";
            Icon = "decardicon.png";
        }
    }
}

