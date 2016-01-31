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
        /// <summary>
        /// Anzahl Frames bis zum nächsten Keyframe
        /// </summary>
        private const int KEYFRAME = 10;

        /// <summary>
        /// Anzahl Frames bis zum nächsten kleinen Update
        /// </summary>
        private const int UPDATEFRAME = 2;

        /// <summary>
        /// Listener für ankommende Verbindungen.
        /// </summary>
        private TcpListener listener;

        private RheinwerkGame game;

        /// <summary>
        /// Zuletzt verwendete Client Id für ankommende Verbindungen.
        /// </summary>
        private int lastId = 0;

        /// <summary>
        /// Fortlaufender Frame-Counter seit dem Start des Servers.
        /// </summary>
        private int currentFrame = 0;

        /// <summary>
        /// Auflistung aller aktuell verbundenen Clients.
        /// </summary>
        private List<Client> clients;

        /// <summary>
        /// Nachschlagewerk für alle aktiven Items.
        /// </summary>
        private Dictionary<int, ItemCacheEntry> items;

        /// <summary>
        /// Nachschlagewerk für alle Quests.
        /// </summary>
        private Dictionary<string, QuestCacheEntry> quests;

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
            items = new Dictionary<int, ItemCacheEntry>();
            quests = new Dictionary<string, QuestCacheEntry>();

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

            // Items hinzufügen
            if (State == ServerState.Running)
            {
                // Ankommende Nachrichten verarbeiten
                foreach (var client in clients)
                    client.Update(game.Simulation, items);

                currentFrame++;
                foreach (var area in game.Simulation.World.Areas)
                {
                    foreach (var item in area.Items)
                    {
                        HandleItem(item, area, null);
                        if (item is IInventory)
                        {
                            IInventory inventory = item as IInventory;
                            foreach (var inventoryItem in inventory.Inventory)
                                HandleItem(inventoryItem, null, inventory);
                        }
                    }
                }

                var droppedItems = items.Values.Where(i => i.LastUpdate != currentFrame).ToArray();
                foreach (var item in droppedItems)
                {
                    // Remove item
                    items.Remove(item.Item.Id);
                    foreach (var client in clients.ToArray())
                        client.SendRemove(item.Item.Id);
                }

                // Quest Updates
                foreach (var quest in game.Simulation.World.Quests)
                {
                    QuestCacheEntry entry = quests[quest.Name];
                    string progress = quest.CurrentProgress == null ? string.Empty : quest.CurrentProgress.Id;
                    if (quest.State != entry.State || progress != entry.Progress)
                    {
                        entry.State = quest.State;
                        entry.Progress = progress;
                        foreach (var client in clients.ToArray())
                            client.SendQuestUpdate(quest.Name, progress, quest.State);
                    }
                }
            }
        }

        /// <summary>
        /// Verarbeitet den Zustandsabgleich eines Items.
        /// </summary>
        /// <param name="item">Item referenz</param>
        /// <param name="area">Area in der das Item liegt (oder null, falls Inventar)</param>
        /// <param name="inventory">Inventar in dem sich das Item befindet (oder null, falls Area)</param>
        private void HandleItem(Item item, Area area, IInventory inventory)
        {
            ItemCacheEntry entity;
            if (items.TryGetValue(item.Id, out entity))
            {
                // Frame Update
                entity.LastUpdate = currentFrame;

                // Item move
                if (entity.Area != area || entity.Inventory != inventory)
                {
                    foreach (var client in clients.ToArray())
                        client.SendMove(entity.Item, entity.Area, area, entity.Inventory, inventory);
                    entity.Area = area;
                    entity.Inventory = inventory;
                }

                // Updates
                if (currentFrame % KEYFRAME == 0)
                {
                    // Großes Update
                    foreach (var client in clients.ToArray())
                        client.SendKeyUpdate(item);
                }
                else if (currentFrame % UPDATEFRAME == 0)
                {
                    // Kleines Update
                    foreach (var client in clients.ToArray())
                        client.SendUpdate(item);    
                }
            }
            else
            {
                // Item fehlt -> Insert
                items.Add(item.Id, new ItemCacheEntry()
                    { 
                        Item = item, 
                        Area = area, 
                        Inventory = inventory,
                        LastUpdate = currentFrame
                    });
                            
                foreach (var client in clients.ToArray())
                    client.SendInsert(item, area, inventory);                    
            }
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
            game.Simulation.NewGame(SimulationMode.Server);

            // Items kartographieren
            currentFrame = 0;
            items.Clear();
            foreach (var area in game.Simulation.World.Areas)
            {
                foreach (var item in area.Items)
                {
                    items.Add(item.Id, new ItemCacheEntry() { Item = item, Area = area });

                    if (item is IInventory)
                    {
                        IInventory inventory = item as IInventory;
                        foreach (var inventoryItem in inventory.Inventory)
                            items.Add(inventoryItem.Id, new ItemCacheEntry() { Item = inventoryItem, Inventory = inventory });
                    }
                }
            }

            // Quests kartographieren
            quests.Clear();
            foreach (var quest in game.Simulation.World.Quests)
            {
                quests.Add(quest.Name, new QuestCacheEntry()
                    { 
                        Name = quest.Name, 
                        State = quest.State, 
                        Progress = quest.CurrentProgress != null ? quest.CurrentProgress.Id : string.Empty 
                    });
            }

            // Player einfügen
            game.Local.Player = new Player(game.Simulation.World.NextId++);
            game.Simulation.InsertPlayer(game.Local.Player);

            // Player für jeden Player
            foreach (var client in clients.ToArray())
            {
                client.Player = new Player(game.Simulation.World.NextId++);
                game.Simulation.InsertPlayer(client.Player);
                client.SendStart(client.Player.Id);
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
            items.Clear();
            quests.Clear();
        }
    }

    /// <summary>
    /// Auflistung der möglichen Server Zustände.
    /// </summary>
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

