using System;

namespace RheinwerkAdventure.Model
{
    /// <summary>
    /// Interface für alle einsammelbaren Items.
    /// </summary>
    internal interface ICollectable
    {
        /// <summary>
        /// Action die aufgerufen wird, wenn das Item eingesammelt wird.
        /// </summary>
        Action<RheinwerkGame, Item> OnCollect { get; }
    }
}

