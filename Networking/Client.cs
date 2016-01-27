using System;
using System.Net.Sockets;
using RheinwerkAdventure.Model;
using RheinwerkAdventure.Components;

namespace RheinwerkAdventure.Networking
{
    /// <summary>
    /// Die Repräsentanz eines Clients auf Server-Seite.
    /// </summary>
    internal class Client
    {
        private TcpClient client;

        private NetworkStream stream;

        /// <summary>
        /// Gibt den Verbindungsstatus an.
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// Gibt den referenzierten Player des Models an, 
        /// sofern das Spiel schon gestartet wurde.
        /// </summary>
        public Player Player { get; set; }

        public Client(TcpClient client)
        {
            this.client = client;
            stream = client.GetStream();

            Connected = true;
        }

        public void Close()
        {
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

