using System;
using System.Net.Sockets;
using RheinwerkAdventure.Model;
using RheinwerkAdventure.Components;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace RheinwerkAdventure.Networking
{
    /// <summary>
    /// Die Repräsentanz eines Clients auf Server-Seite.
    /// </summary>
    internal class Client
    {
        private const int BUFFERSIZE = 16;

        private TcpClient client;

        private NetworkStream stream;

        private BinaryWriter writer;

        private BinaryReader reader;

        private MemoryStream readerStream;

        private MemoryStream writerStream;

        private byte[] readerBuffer;

        private byte[] writerBuffer;

        /// <summary>
        /// Gibt den Verbindungsstatus an.
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// Gibt die Exception an die bei einem Disconnect passiert ist.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gibt den referenzierten Player des Models an, 
        /// sofern das Spiel schon gestartet wurde.
        /// </summary>
        public Player Player { get; set; }

        public Client(TcpClient client, int clientId)
        {
            this.client = client;
            stream = client.GetStream();

            readerBuffer = new byte[BUFFERSIZE];
            readerStream = new MemoryStream(readerBuffer);
            reader = new BinaryReader(readerStream);

            writerBuffer = new byte[BUFFERSIZE];
            writerStream = new MemoryStream(writerBuffer);
            writer = new BinaryWriter(writerStream);

            // Hallo senden
            try
            {
                writer.Write((byte)MessageType.ServerHello);
                writer.Write(clientId);
                Flush();
                Connected = true;

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
                    case MessageType.ClientClose:
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

        /// <summary>
        /// Sendet die Daten aus dem WriteBuffer gesammelt weg.
        /// </summary>
        private void Flush()
        {
            int length = (int)writerStream.Position;
            stream.Write(writerBuffer, 0, length);
            stream.Flush();
            writerStream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Sendet die Anzahl vorhandener Player.
        /// </summary>
        /// <param name="count">Anzahl Spieler</param>
        public void SendPlayerCount(int count)
        {
            try
            {
                writer.Write((byte)MessageType.ServerPlayerCount);
                writer.Write(count);
                Flush();
            }
            catch (Exception ex)
            {
                Close(ex, false);
            }
        }

        /// <summary>
        /// Sendet das Signal zum Start des Spiels.
        /// </summary>
        public void SendStart()
        {
            try
            {
                writer.Write((byte)MessageType.ServerStartGame);
                Flush();
            }
            catch (Exception ex)
            {
                Close(ex, false);
            }
        }

        public void Close()
        {
            Close(null, false);
        }

        /// <summary>
        /// Schließt die Verbindung
        /// </summary>
        /// <param name="ex">Potentielle Exception für das Logging</param>
        /// <param name="silent">Gibt an ob der Server sich noch beim Client verabschieden sollte.</param>
        private void Close(Exception ex, bool silent)
        {
            Connected = false;
            Exception = ex;

            // Goodbye Message zum Client senden
            if (!silent && stream != null)
            {
                try
                {
                    writer.Write((byte)MessageType.ServerClose);
                    Flush();
                }
                catch
                {
                }
            }

            // Stream schließen
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
            
            // Referenz entfernen
            stream = null;
            writer = null;
            readerStream = null;
            reader = null;

            // Vorhandener Client
            try
            {
                client.Close();
            }
            catch
            {
            }

            // Referenz entfernen
            client = null;
        }
    }
}

