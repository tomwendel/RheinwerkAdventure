using System;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Net.Sockets;
using RheinwerkAdventure.Networking;
using System.Collections.Generic;
using RheinwerkAdventure.Model;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Server Komponente für das Netzwerspiel
    /// </summary>
    internal class ServerComponent : GameComponent
    {
        private TcpListener listener;

        private RheinwerkGame game;

        private int lastId = 0;

        private List<Client> clients;

        /// <summary>
        /// Aktueller Status des Servers
        /// </summary>
        public ServerState State { get; private set; }

        /// <summary>
        /// Auflistung aktiver Clients
        /// </summary>
        public IEnumerable<Client> Clients { get { return clients; } }

        public ServerComponent(RheinwerkGame game)
            : base(game)
        {
            this.game = game;

            clients = new List<Client>();

            State = ServerState.Closed;
        }

        public override void Update(GameTime gameTime)
        {
            // Nur wenn Komponente aktiviert wurde.
            if (!Enabled)
                return;

            // geschlossene Clients entfernen
            var closedClients = clients.Where(c => !c.Connected).ToArray();
            foreach (var client in closedClients)
            {
                game.Simulation.RemovePlayer(client.Player);
                client.Close();
                clients.Remove(client);
            }

            // Update schicken
            if (closedClients.Length > 0)
                BroadcastPlayerCount();
        }

        /// <summary>
        /// Öffnet den Server für ankommende Verbindungen
        /// </summary>
        public void OpenServer()
        {
            listener = TcpListener.Create(1201);
            listener.Start();
            listener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
            State = ServerState.Listening;
        }

        /// <summary>
        /// Interner Callback für ankommende Verbindungen
        /// </summary>
        private void AcceptTcpClientCallback(IAsyncResult result)
        {
            try
            {
                // Erstellt eine neue Client-Verbindung
                TcpClient tcpClient = listener.EndAcceptTcpClient(result);
                lastId += 1;
                Client client = new Client(tcpClient, lastId);
                clients.Add(client);
            }
            catch (Exception)
            {
            }

            // Broadcast neuen Count
            BroadcastPlayerCount();

            // Weiter zuhören
            if (State == ServerState.Listening)
                listener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
        }

        /// <summary>
        /// Sendet die aktualisierte Anzahl Player an alle verbundenen Clients.
        /// </summary>
        private void BroadcastPlayerCount()
        {
            int count = clients.Count() + 1;
            foreach (var client in clients.ToArray())
                client.SendPlayerCount(count);
        }

        /// <summary>
        /// Startet das Game mit den aktuell verbundenen Clients.
        /// Der Server wird für eingehende Verbindungen geschlossen.
        /// </summary>
        public void StartGame()
        {
            State = ServerState.Running;
            listener.Stop();

            // Start World
            game.Simulation.NewGame();
            game.Local.Player = new Player();
            game.Simulation.InsertPlayer(game.Local.Player);

            // Player für jeden Player
            foreach (var client in clients.ToArray())
            {
                client.Player = new Player();
                game.Simulation.InsertPlayer(client.Player);
                client.SendStart();
            }
        }

        /// <summary>
        /// Stoppt den Server.
        /// </summary>
        public void CloseServer()
        {
            // Listener stoppen sofern aktiv
            if (State == ServerState.Listening)
            {
                State = ServerState.Closed;
                listener.Stop();
                listener = null;
            }

            State = ServerState.Closed;

            // Close Client Connections
            foreach (var client in clients.ToArray())
            {
                if (game.Simulation.World != null)
                {
                    game.Simulation.RemovePlayer(client.Player);
                    client.Player = null;
                }
                client.Close();
            }
            clients.Clear();

            // Welt schließen
            if (game.Local.Player != null)
            {
                game.Simulation.RemovePlayer(game.Local.Player);
                game.Local.Player = null;
            }
            game.Simulation.CloseGame();
        }
    }

    internal enum ServerState
    {
        Closed,
        Listening,
        Running,
    }
}

