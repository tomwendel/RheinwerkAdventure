using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RheinwerkAdventure.Networking;

namespace RheinwerkAdventure.Components
{
    internal class ServerComponent : GameComponent, IServerComponent
    {
        public ServerComponent(Game game) : base(game)
        {
        }

        public IEnumerable<Client> Clients
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool ServerFeatureAvailable
        {
            get
            {
                return false;
            }
        }

        public ServerState State
        {
            get
            {
                return ServerState.Closed;
            }
        }

        public void CloseServer()
        {
            throw new NotImplementedException();
        }

        public void OpenServer()
        {
            throw new NotImplementedException();
        }

        public void StartGame()
        {
            throw new NotImplementedException();
        }
    }

    internal enum ServerState
    {
        /// <summary>
        /// Server geschlossen.
        /// </summary>
        Closed,

        /// <summary>
        /// Server wartet auf ankommende Verbindungen.
        /// </summary>
        Listening,

        /// <summary>
        /// Spiel läuft gerade.
        /// </summary>
        Running,
    }
}
