using System;

namespace RheinwerkAdventure.Networking
{
    /// <summary>
    /// Auflistung aller möglichen Netzwerk-Nachrichten
    /// </summary>
    internal enum MessageType
    {
        /// <summary>
        /// Server sagt hallo
        /// 1;ClientId[int]
        /// </summary>
        ServerHello = 1,

        /// <summary>
        /// Server beendet die Verbindung
        /// </summary>
        ServerClose = 3,

        /// <summary>
        /// Client beendet die Verbindung
        /// </summary>
        ClientClose = 4,

        /// <summary>
        /// Server startet das Spiel
        /// </summary>
        ServerStartGame = 5,

        /// <summary>
        /// Server sendet den Player-Count
        /// 6;Count[int]
        /// </summary>
        ServerPlayerCount = 6,
    }
}

