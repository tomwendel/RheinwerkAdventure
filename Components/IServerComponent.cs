using RheinwerkAdventure.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RheinwerkAdventure.Components
{
    internal interface IServerComponent
    {
        /// <summary>
        /// Gibt an ob der Server überhaupt gestartet werden kann.
        /// </summary>
        bool ServerFeatureAvailable { get; }

        /// <summary>
        /// Aktueller Status des Servers
        /// </summary>
        ServerState State { get; }
        
        /// <summary>
        /// Auflistung aktiver Clients
        /// </summary>
        IEnumerable<Client> Clients { get; }

        /// <summary>
        /// Öffnet den Server für ankommende Verbindungen
        /// </summary>
        void OpenServer();

        /// <summary>
        /// Startet das Game mit den aktuell verbundenen Clients.
        /// Der Server wird für eingehende Verbindungen geschlossen.
        /// </summary>
        void StartGame();

        /// <summary>
        /// Stoppt den Server.
        /// </summary>
        void CloseServer();
    }
}
