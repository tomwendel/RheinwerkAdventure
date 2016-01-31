using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RheinwerkAdventure.Model;

namespace RheinwerkAdventure.Components
{
    internal class ClientComponent : GameComponent, IClientComponent
    {
        public ClientComponent(Game game) : base(game)
        {
        }

        public bool ClientFeatureAvailable
        {
            get
            {
                return false;
            }
        }

        public int ClientId
        {
            get
            {
                return 0;
            }
        }

        public int PlayerCount
        {
            get
            {
                return 0;
            }
        }

        public ClientState State
        {
            get
            {
                return ClientState.Closed;
            }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void SendItemTransfer(Item item, IInventory sender, IInventory receiver)
        {
            throw new NotImplementedException();
        }

        public void SendQuestUpdate(string quest, string progress, QuestState state)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Auflistung der möglichen Client-Zustände.
    /// </summary>
    internal enum ClientState
    {
        /// <summary>
        /// Verbindung wird gerade aufgebaut.
        /// </summary>
        Connecting,

        /// <summary>
        /// Verbindung aufgebaut.
        /// </summary>
        Connected,

        /// <summary>
        /// Spiel läuft.
        /// </summary>
        Running,

        /// <summary>
        /// Verbindung ist geschlossen.
        /// </summary>
        Closed,
    }
}
