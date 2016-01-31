using System;

namespace RheinwerkAdventure.Networking
{
    /// <summary>
    /// Auflistung aller möglichen Netzwerk-Nachrichten
    /// Alle Nachrichten werden mit Type als byte, gefolgt von der Nachrichtenlänge als short gesendet.
    /// Type[byte];Length[short];Payload[byte[]]
    /// </summary>
    internal enum MessageType
    {
        #region Verbindung & Flusskontrolle

        /// <summary>
        /// Undefinierte Nachricht
        /// </summary>
        None = 0,

        /// <summary>
        /// Server sagt hallo
        /// ClientId[int]
        /// </summary>
        ServerHello = 1,

        /// <summary>
        /// Server beendet die Verbindung
        /// 
        /// </summary>
        ServerClose = 3,

        /// <summary>
        /// Client beendet die Verbindung
        /// 
        /// </summary>
        ClientClose = 4,

        /// <summary>
        /// Server startet das Spiel
        /// PlayerId[int]
        /// </summary>
        ServerStartGame = 5,

        /// <summary>
        /// Server sendet den Player-Count
        /// Count[int]
        /// </summary>
        ServerPlayerCount = 6,

        #endregion

        #region Serverupdates

        /// <summary>
        /// Server signalisiert ein neues Item
        /// Area[string];Type[string];Id[int];Payload[byte[]]
        /// </summary>
        ServerInsertItemToArea = 10,

        /// <summary>
        /// Server signalisiert ein neues Item
        /// InventoryId[int];Type[string];Id[int];Payload[byte[]]
        /// </summary>
        ServerInsertItemToInventory = 11,

        /// <summary>
        /// Server signalisiert ein kleines Update eines Items
        /// Id[int];Length[int];Payload[byte[]]
        /// </summary>
        ServerUpdateItem = 21,

        /// <summary>
        /// Server signalisiert ein großes Update eines Items
        /// Id[int];Length[int];Payload[byte[]]
        /// </summary>
        ServerKeyUpdateItem = 22,

        /// <summary>
        /// Server signalisiert das Entfernen eines Items
        /// Id[int]
        /// </summary>
        ServerRemoveItem = 31,

        /// <summary>
        /// Server signalisiert einen Move von Area zu Area
        /// ItemId[int];OldArea[string];NewArea[string]
        /// </summary>
        ServerMoveAreaToArea = 41,

        /// <summary>
        /// Server signalisiert einen Move von Area zu Area
        /// ItemId[int];OldArea[string];InventoryId[int]
        /// </summary>
        ServerMoveAreaToInventory = 42,

        /// <summary>
        /// Server signalisiert einen Move von Area zu Area
        /// ItemId[int];InventoryId[int];NewArea[string]
        /// </summary>
        ServerMoveInventoryToArea = 43,

        /// <summary>
        /// Server signalisiert einen Move von Area zu Area
        /// ItemId[int];OldInventoryId[int];NewInventoryId[int]
        /// </summary>
        ServerMoveInventoryToInventory = 44,

        /// <summary>
        /// Server signalisiert einen veränderten Quest-Status.
        /// QuestId[string];Progress[string];State[byte]
        /// </summary>
        ServerQuestUpdate = 51,

        #endregion

        #region Clientupdates

        /// <summary>
        /// Client sendet ein Velocity-Update zum Server
        /// X[float];Y[float]
        /// </summary>
        ClientUpdateVelocity = 101,

        /// <summary>
        /// Client sendet einen Attack-Trigger zum Server
        /// 
        /// </summary>
        ClientTriggerAttack = 102,

        /// <summary>
        /// Client signalisiert dem Server einen Item-Transfer (Quest oder Handel)
        /// ItemId[int];Sender[int];Receiver[int](0 für zerstören)
        /// </summary>
        ClientTransferItem = 103,

        /// <summary>
        /// Client signalisiert ein Quest Update.
        /// QuestId[string];Progress[string];State[byte]
        /// </summary>
        ClientQuestUpdate = 104,

        #endregion
    }
}

