using RheinwerkAdventure.Model;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Interface für die Client-Componente
    /// </summary>
    internal interface IClientComponent
    {
        /// <summary>
        /// Gibt an ob der Client überhaupt gestartet werden kann.
        /// </summary>
        bool ClientFeatureAvailable { get; }

        /// <summary>
        /// Verbindungsstatus des Clients.
        /// </summary>
        ClientState State { get; }

        /// <summary>
        /// Client Id zur aktuellen Verbindung.
        /// </summary>
        int ClientId { get; }

        /// <summary>
        /// Gibt die Anzahl Player zurück.
        /// </summary>
        int PlayerCount { get; }

        /// <summary>
        /// Baut die Verbindung zum Server auf.
        /// </summary>
        void Connect();

        /// <summary>
        /// Verbindung schließen.
        /// </summary>
        void Close();

        /// <summary>
        /// Sendet einen Transfer-Request zum Server.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="sender">Sender.</param>
        /// <param name="receiver">Receiver.</param>
        void SendItemTransfer(Item item, IInventory sender, IInventory receiver);

        /// <summary>
        /// Sendet einen Quest-Update Request zum Server.
        /// </summary>
        /// <param name="quest">Quest.</param>
        /// <param name="progress">Progress.</param>
        /// <param name="state">State.</param>
        void SendQuestUpdate(string quest, string progress, QuestState state);
    }
}
