using System;

namespace RheinwerkAdventure.Networking
{
    /// <summary>
    /// Einzelnes Nachrichtenfragment
    /// </summary>
    internal class Message
    {
        /// <summary>
        /// Typ der Nachricht
        /// </summary>
        public MessageType MessageType { get; set; }

        /// <summary>
        /// Information
        /// </summary>
        public byte[] Payload { get; set; }
    }
}

