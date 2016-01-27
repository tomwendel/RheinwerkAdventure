using System;
using Microsoft.Xna.Framework;
using System.Net.Sockets;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Client für eine Netzwerk-Verbindung.
    /// </summary>
    internal class ClientComponent : GameComponent
    {
        private RheinwerkGame game;

        private TcpClient client;

        private NetworkStream stream;

        private byte[] buffer;

        public ClientState State { get; private set; }

        public ClientComponent(RheinwerkGame game)
            : base(game)
        {
            this.game = game;
            buffer = new byte[1024];
            State = ClientState.Closed;
        }

        public override void Update(GameTime gameTime)
        {
            // Nur wenn Komponente aktiviert wurde.
            if (!Enabled)
                return;
        }

        public void Connect()
        {
            client = new TcpClient();
            client.BeginConnect("localhost", 1201, ConnectCallback, null);
            State = ClientState.Connecting;
        }

        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                client.EndConnect(result);
                stream = client.GetStream();
                State = ClientState.Connected;

                stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, null);
            }
            catch (Exception ex)
            {
                Close(ex);
            }
        }

        private void ReadCallback(IAsyncResult result)
        {
            try
            {
                int count = stream.EndRead(result);

                // TODO: Handle Data

                stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, null);
            }
            catch (Exception ex)
            {
                Close(ex);
            }
        }

        public void Close()
        {
            Close(null);
        }

        private void Close(Exception ex)
        {
            // TODO: Close-Nachricht senden (falls möglich)
            // TODO: Welt schließen, Player löschen
            // TODO: Messagebox an den Client schicken (falls Exception)

            // Stream schließen (falls möglich)
            try
            {
                stream.Close();
            }
            catch
            {
            }

            // Stream disposen
            try
            {
                stream.Dispose();
            }
            catch
            {
            }

            stream = null;

            // Client schließen (falls möglich)
            try
            {
                client.Close();
            }
            catch
            {
            }
            client = null;

            State = ClientState.Closed;
        }
    }

    internal enum ClientState
    {
        Connecting,
        Connected,
        Running,
        Closed,
    }
}

