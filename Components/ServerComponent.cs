using System;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Net.Sockets;
using RheinwerkAdventure.Networking;
using System.Collections.Generic;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Server Komponente für das Netzwerspiel
    /// </summary>
    internal class ServerComponent : GameComponent
    {
        private TcpListener listener;

        private RheinwerkGame game;

        private List<Client> clients;

        public ServerState State { get; private set; }

        public IEnumerable<Client> Clients { get { return clients; } }

        public ServerComponent(RheinwerkGame game)
            : base(game)
        {
            this.game = game;

            listener = TcpListener.Create(1201);
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
        }

        public void OpenServer()
        {
            listener.Start();
            listener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
            State = ServerState.Listening;
        }

        private void AcceptTcpClientCallback(IAsyncResult result)
        {
            try
            {
                TcpClient tcpClient = listener.EndAcceptTcpClient(result);
                Client client = new Client(tcpClient);
                clients.Add(client);
            }
            catch (Exception)
            {
            }

            if (State == ServerState.Listening)
                listener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
        }

        public void StartGame()
        {
            // TODO: Start World
            // TODO: Add local Player
            // TODO: Add Player für jeden Client
            State = ServerState.Running;
        }

        public void CloseServer()
        {
            listener.Stop();

            // Close Client Connections
            foreach (var client in clients)
                client.Close();

            // Welt schließen
            if (game.Local.Player != null)
                game.Simulation.RemovePlayer(game.Local.Player);
            game.Simulation.CloseGame();

            State = ServerState.Closed;
        }
    }

    internal enum ServerState
    {
        Closed,
        Listening,
        Running,
    }
}

