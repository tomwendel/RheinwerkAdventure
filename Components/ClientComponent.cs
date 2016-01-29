using System;
using Microsoft.Xna.Framework;
using System.Net.Sockets;
using RheinwerkAdventure.Networking;
using System.IO;
using System.Configuration;
using RheinwerkAdventure.Model;
using RheinwerkAdventure.Screens;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Client für eine Netzwerk-Verbindung.
    /// </summary>
    internal class ClientComponent : GameComponent
    {
        private const int BUFFERSIZE = 16;

        private RheinwerkGame game;

        private TcpClient client;

        private NetworkStream stream;

        private BinaryWriter writer;

        private BinaryReader reader;

        private MemoryStream readerStream;

        private MemoryStream writerStream;

        private byte[] readerBuffer;

        private byte[] writerBuffer;

        /// <summary>
        /// Verbindungsstatus des Clients.
        /// </summary>
        public ClientState State { get; private set; }

        /// <summary>
        /// Client Id zur aktuellen Verbindung.
        /// </summary>
        public int ClientId { get; private set; }

        /// <summary>
        /// Gibt die Anzahl Player zurück.
        /// </summary>
        public int PlayerCount { get; private set; }

        public ClientComponent(RheinwerkGame game)
            : base(game)
        {
            this.game = game;

            readerBuffer = new byte[BUFFERSIZE];
            readerStream = new MemoryStream(readerBuffer);
            reader = new BinaryReader(readerStream);

            writerBuffer = new byte[BUFFERSIZE];
            writerStream = new MemoryStream(writerBuffer);
            writer = new BinaryWriter(writerStream);

            State = ClientState.Closed;
            ClientId = 0;
            PlayerCount = 0;
        }

        public override void Update(GameTime gameTime)
        {
            // Nur wenn Komponente aktiviert wurde.
            if (!Enabled)
                return;

            // TODO: Daten beobachten
        }

        /// <summary>
        /// Baut die Verbindung zum Server auf.
        /// </summary>
        public void Connect()
        {
            string server = ConfigurationManager.AppSettings["Server"];

            client = new TcpClient();
            client.BeginConnect(server ?? "localhost", 1201, ConnectCallback, null);
            State = ClientState.Connecting;
        }

        /// <summary>
        /// Callback des Verbindungsversuchs.
        /// </summary>
        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                client.EndConnect(result);
                stream = client.GetStream();
                writer = new BinaryWriter(stream);
                State = ClientState.Connected;

                stream.BeginRead(readerBuffer, 0, readerBuffer.Length, ReadCallback, null);
            }
            catch (Exception ex)
            {
                Close(ex, false);
            }
        }

        /// <summary>
        /// Read Callback
        /// </summary>
        private void ReadCallback(IAsyncResult result)
        {
            try
            {
                int count = stream.EndRead(result);

                // Daten auslesen
                readerStream.Seek(0, SeekOrigin.Begin);
                MessageType mType = (MessageType)reader.ReadByte();
                switch (mType)
                {
                    case MessageType.ServerHello:
                        ClientId = reader.ReadInt32();
                        break;
                    case MessageType.ServerStartGame:
                        State = ClientState.Running;
                        game.Simulation.NewGame();
                        game.Local.Player = new Player();
                        game.Simulation.InsertPlayer(game.Local.Player);
                        break;
                    case MessageType.ServerPlayerCount:
                        PlayerCount = reader.ReadInt32();
                        break;
                    case MessageType.ServerClose:
                        Close(null, true);
                        break;
                }

                stream.BeginRead(readerBuffer, 0, readerBuffer.Length, ReadCallback, null);
            }
            catch (Exception ex)
            {
                Close(ex, false);
            }
        }

        private void Flush()
        {
            int length = (int)writerStream.Position;
            stream.Write(writerBuffer, 0, length);
            stream.Flush();
            writerStream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Verbindung schließen.
        /// </summary>
        public void Close()
        {
            Close(null, false);
        }

        /// <summary>
        /// Interner Cleanup
        /// </summary>
        private void Close(Exception ex, bool silent)
        {
            State = ClientState.Closed;
            ClientId = 0;
            PlayerCount = 0;

            // TODO: Welt schließen, Player löschen
            if (game.Local.Player != null)
            {
                game.Simulation.RemovePlayer(game.Local.Player);
                game.Local.Player = null;
            }

            // Goodbye Message zum Client senden
            if (!silent && stream != null)
            {
                try
                {
                    writer.Write((byte)MessageType.ClientClose);
                    Flush();
                }
                catch
                {
                }
            }

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

            // Hauptmenü wieder öffnen
            game.Screen.ShowScreen(new MainMenuScreen(game.Screen));
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

