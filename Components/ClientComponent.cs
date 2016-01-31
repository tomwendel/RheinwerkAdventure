using System;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Net.Sockets;
using RheinwerkAdventure.Networking;
using System.IO;
using System.Configuration;
using RheinwerkAdventure.Model;
using RheinwerkAdventure.Screens;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Client für eine Netzwerk-Verbindung.
    /// </summary>
    internal class ClientComponent : GameComponent
    {
        /// <summary>
        /// Puffergröße
        /// </summary>
        private const int BUFFERSIZE = 512;

        private RheinwerkGame game;

        /// <summary>
        /// Tcp Connection zum Server
        /// </summary>
        private TcpClient client;

        /// <summary>
        /// Network Stream zum Server
        /// </summary>
        private NetworkStream stream;

        /// <summary>
        /// Lesepuffer für einkommende Nachrichten
        /// </summary>
        private byte[] readerBuffer;

        /// <summary>
        /// Schreibpuffer für ausgehende nachrichten
        /// </summary>
        private byte[] writerBuffer;

        /// <summary>
        /// Stream-Wrapper für den Schreibpuffer
        /// </summary>
        private MemoryStream writerStream;

        /// <summary>
        /// Binary Writer für den Schreibpuffer
        /// </summary>
        private BinaryWriter writer;

        /// <summary>
        /// Zwischenspeicher für den asynchronen Lese-Callback
        /// </summary>
        private MessageType nextMessageType = MessageType.None;

        /// <summary>
        /// Gibt die anzahl Bytes an die für die nächste Nachricht bereits empfangen wurden.
        /// </summary>
        private int nextMessageCollectedBytes = 0;

        /// <summary>
        /// Gibt die Anzanl Bytes an, die für die nächste Nachricht noch gebraucht werden.
        /// </summary>
        private int nextMessageRequiredBytes = 3;

        /// <summary>
        /// Alte Velocity um herauszufinden, ob ein Update erforderlich ist.
        /// </summary>
        private Vector2 oldVelocity;

        /// <summary>
        /// Nachrichten-Schlange für die Synchronisation.
        /// </summary>
        private ConcurrentQueue<Message> messages;

        /// <summary>
        /// Dictionary für Items zum schnelleren Auffinden von Items.
        /// </summary>
        private Dictionary<int, ItemCacheEntry> items;

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
            items = new Dictionary<int, ItemCacheEntry>();

            // Buffer und Writer initialisieren
            readerBuffer = new byte[BUFFERSIZE];
            writerBuffer = new byte[BUFFERSIZE];
            writerStream = new MemoryStream(writerBuffer);
            writer = new BinaryWriter(writerStream);

            State = ClientState.Closed;
            ClientId = 0;
            PlayerCount = 0;
            messages = new ConcurrentQueue<Message>();
        }

        public override void Update(GameTime gameTime)
        {
            // Nur wenn Komponente aktiviert wurde.
            if (!Enabled)
                return;

            // Steuerdaten zum Server schicken
            if (State == ClientState.Running && game.Local.Player != null)
            {
                // Velocity
                if (oldVelocity != game.Local.Player.Velocity)
                    SendVelocityUpdate(game.Local.Player.Velocity);
                oldVelocity = game.Local.Player.Velocity;

                // Attack-Trigger
                if (game.Local.Player.AttackSignal)
                    SendAttackTrigger();
            }

            // Ankommende Nachrichten verarbeiten
            Message m;
            while (messages.TryDequeue(out m))
            {
                switch (m.MessageType)
                {
                    case MessageType.ServerHello:
                        ClientId = BitConverter.ToInt32(m.Payload, 0);
                        break;
                    case MessageType.ServerStartGame:
                        StartGame(BitConverter.ToInt32(m.Payload, 0));
                        break;
                    case MessageType.ServerPlayerCount:
                        PlayerCount = BitConverter.ToInt32(m.Payload, 0);
                        break;
                    case MessageType.ServerClose:
                        Close(null, true);
                        break;
                    case MessageType.ServerInsertItemToArea:
                        // Area[string];Type[string];Id[int];Length[int];Payload[byte[]]
                        using (MemoryStream stream = new MemoryStream(m.Payload))
                        {
                            using (BinaryReader br = new BinaryReader(stream))
                            {
                                string area = br.ReadString();
                                ItemInsert(br.ReadString(), br.ReadInt32(), area, null, br);
                            }
                        }
                        break;
                    case MessageType.ServerInsertItemToInventory:
                        // InventoryId[int];Type[string];Id[int];Length[int];Payload[byte[]]
                        using (MemoryStream stream = new MemoryStream(m.Payload))
                        {
                            using (BinaryReader br = new BinaryReader(stream))
                            {
                                int inventory = br.ReadInt32();
                                ItemInsert(br.ReadString(), br.ReadInt32(), null, inventory, br);
                            }
                        }
                        break;
                    case MessageType.ServerUpdateItem:
                        // Id[int];Length[int];Payload[byte[]]
                        using (MemoryStream stream = new MemoryStream(m.Payload))
                        {
                            using (BinaryReader br = new BinaryReader(stream))
                            {
                                ItemUpdate(br.ReadInt32(), br);
                            }
                        }
                        break;
                    case MessageType.ServerKeyUpdateItem:
                        // Id[int];Length[int];Payload[byte[]]
                        using (MemoryStream stream = new MemoryStream(m.Payload))
                        {
                            using (BinaryReader br = new BinaryReader(stream))
                            {
                                ItemKeyUpdate(br.ReadInt32(), br);
                            }
                        }
                        break;
                    case MessageType.ServerRemoveItem:
                        // Id[int]
                        ItemRemove(BitConverter.ToInt32(m.Payload, 0));
                        break;
                    case MessageType.ServerMoveAreaToArea:
                        // 41;ItemId[int];OldArea[string];NewArea[string]
                        using (MemoryStream stream = new MemoryStream(m.Payload))
                        {
                            using (BinaryReader br = new BinaryReader(stream))
                            {
                                ItemMove(br.ReadInt32(), br.ReadString(), br.ReadString(), null, null);
                            }
                        }
                        break;
                    case MessageType.ServerMoveAreaToInventory:
                        // 42;ItemId[int];OldArea[string];InventoryId[int]
                        using (MemoryStream stream = new MemoryStream(m.Payload))
                        {
                            using (BinaryReader br = new BinaryReader(stream))
                            {
                                ItemMove(br.ReadInt32(), br.ReadString(), null, null, br.ReadInt32());
                            }
                        }
                        break;
                    case MessageType.ServerMoveInventoryToArea:
                        // 43;ItemId[int];InventoryId[int];NewArea[string]
                        using (MemoryStream stream = new MemoryStream(m.Payload))
                        {
                            using (BinaryReader br = new BinaryReader(stream))
                            {
                                int iBuffer = br.ReadInt32();
                                ItemMove(br.ReadInt32(), null, br.ReadString(), iBuffer, null);
                            }
                        }
                        break;
                    case MessageType.ServerMoveInventoryToInventory:
                        // 44;ItemId[int];OldInventoryId[int];NewInventoryId[int]
                        using (MemoryStream stream = new MemoryStream(m.Payload))
                        {
                            using (BinaryReader br = new BinaryReader(stream))
                            {
                                ItemMove(br.ReadInt32(), null, null, br.ReadInt32(), br.ReadInt32());
                            }
                        }
                        break;
                    case MessageType.ServerQuestUpdate:
                        using (MemoryStream stream = new MemoryStream(m.Payload))
                        {
                            using (BinaryReader br = new BinaryReader(stream))
                            {
                                string quest = br.ReadString();
                                string progress = br.ReadString();
                                QuestState state = (QuestState)br.ReadByte();

                                var qu = game.Simulation.World.Quests.SingleOrDefault(q => q.Name == quest);
                                qu.CurrentProgress = qu.QuestProgresses.FirstOrDefault(q => q.Id.Equals(progress));
                                qu.State = state;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Sendet das Angriffssignal an den Server.
        /// </summary>
        private void SendAttackTrigger()
        {
            try
            {
                writer.Write((byte)MessageType.ClientTriggerAttack);
                writer.Write((short)0);
                Flush();
            }
            catch (Exception ex)
            {
                Close(ex, false);
            }
        }

        /// <summary>
        /// Sendet eine aktualisierte Velocity.
        /// </summary>
        /// <param name="velocity">Velocity.</param>
        private void SendVelocityUpdate(Vector2 velocity)
        {
            try
            {
                writer.Write((byte)MessageType.ClientUpdateVelocity);
                writer.Write((short)8);
                writer.Write(velocity.X);
                writer.Write(velocity.Y);
                Flush();
            }
            catch (Exception ex)
            {
                Close(ex, false);
            }
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
                State = ClientState.Connected;

                nextMessageType = MessageType.None;
                nextMessageCollectedBytes = 0;
                nextMessageRequiredBytes = 3;
                stream.BeginRead(readerBuffer, nextMessageCollectedBytes, nextMessageRequiredBytes, ReadCallback, null);
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
                nextMessageCollectedBytes += count;
                nextMessageRequiredBytes -= count;
                if (count > 0 && nextMessageRequiredBytes == 0) 
                {
                    if (nextMessageType == MessageType.None) 
                    {
                        // Messagetype identifizieren
                        nextMessageType = (MessageType)readerBuffer[0];
                        nextMessageRequiredBytes = BitConverter.ToInt16(readerBuffer, 1);
                    }

                    if (nextMessageType != MessageType.None && nextMessageRequiredBytes == 0)
                    {
                        // Nachricht bauen
                        Message m = new Message()
                            {
                                MessageType = nextMessageType,
                                Payload = new byte[nextMessageCollectedBytes - 3],
                            };

                        // Payload kopieren
                        Array.Copy(readerBuffer, 3, m.Payload, 0, m.Payload.Length);
                        messages.Enqueue(m);

                        // Reset für die nächste Nachricht
                        nextMessageType = MessageType.None;
                        nextMessageCollectedBytes = 0;
                        nextMessageRequiredBytes = 3;
                    }

                    stream.BeginRead(readerBuffer, nextMessageCollectedBytes, nextMessageRequiredBytes, ReadCallback, null);
                }
            }
            catch (Exception ex)
            {
                Close(ex, false);
            }
        }

        /// <summary>
        /// Startet aufgrund einer eingehenden Start-Message vom Server das Spiel.
        /// </summary>
        /// <param name="id">Item ID des lokalen Spielers</param>
        private void StartGame(int id)
        {
            State = ClientState.Running;
            game.Simulation.NewGame(SimulationMode.Client);
            game.Local.Player = new Player(id);
            game.Simulation.InsertPlayer(game.Local.Player);

            // Items kartographieren
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
        }

        /// <summary>
        /// Fügt aufgrund einer Server-Nachricht ein neues item ein.
        /// </summary>
        /// <param name="type">Datentyp</param>
        /// <param name="id">Id</param>
        /// <param name="area">Ziel Area (oder null, falls Inventar)</param>
        /// <param name="inventory">Id des Inventory-Items (oder null, falls Area)</param>
        /// <param name="reader">Binary Reader für den Deserializer</param>
        private void ItemInsert(string type, int id, string area, int? inventory, BinaryReader reader)
        {
            Item item = (Item)Activator.CreateInstance(Type.GetType(type), id);
            item.DeserializeInsert(reader);

            if (items.ContainsKey(id))
                return;

            // in die Area einfügen
            if (!string.IsNullOrEmpty(area))
            {
                Area ar = game.Simulation.World.Areas.Single(a => a.Name.Equals(area));
                ar.Items.Add(item);

                items.Add(id, new ItemCacheEntry() { Item = item, Area = ar });
            }

            // In das Inventar einfügen
            if (inventory.HasValue)
            {
                IInventory i = items[inventory.Value].Item  as IInventory;
                i.Inventory.Add(item);

                items.Add(id, new ItemCacheEntry() { Item = item, Inventory = i });
            }
        }

        /// <summary>
        /// Entfernt aufgrund einer Server-Nachricht ein Item.
        /// </summary>
        /// <param name="id">Id</param>
        private void ItemRemove(int id)
        {
            ItemCacheEntry entry = items[id];
            Item item = entry.Item;

            if (entry.Area != null)
                entry.Area.Items.Remove(item);

            if (entry.Inventory != null)
                entry.Inventory.Inventory.Remove(item);

            items.Remove(id);
        }

        /// <summary>
        /// Aktualisiert ein vorhandenes Item (kleines Update)
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="reader">Binary Reader für den Serializer</param>
        private void ItemUpdate(int id, BinaryReader reader)
        {
            ItemCacheEntry entry = items[id];
            Item item = entry.Item;

            item.DeserializeUpdate(reader);
        }

        /// <summary>
        /// Aktualisiert ein vorhandenes Item (großes Update)
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="reader">Binary Reader für den Serializer</param>
        private void ItemKeyUpdate(int id, BinaryReader reader)
        {
            ItemCacheEntry entry = items[id];
            Item item = entry.Item;

            item.DeserializeKeyUpdate(reader);
        }

        /// <summary>
        /// Verschiebt ein Item von einem Container zu einem anderen (Area oder Inventar)
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="oldArea">Quellarea (oder empty falls Inventar)</param>
        /// <param name="newArea">Zielarea (oder empty falls inventar)</param>
        /// <param name="oldInventory">QuellInventar (oder null, falls Area)</param>
        /// <param name="newInventory">ZielInventar (oder null, falls Inventar)</param>
        private void ItemMove(int id, string oldArea, string newArea, int? oldInventory, int? newInventory)
        {
            ItemCacheEntry entry = items[id];
            Item item = entry.Item;

            // Remove (Area)
            if (!string.IsNullOrEmpty(oldArea))
            {
                Area area = game.Simulation.World.Areas.Single(a => a.Name.Equals(oldArea));
                if (area.Items.Contains(item))
                    area.Items.Remove(item);
            }

            // Remove (Inventar)
            if (oldInventory.HasValue)
            {
                IInventory inventory = items[oldInventory.Value].Item as IInventory;
                if (inventory.Inventory.Contains(item))
                    inventory.Inventory.Remove(item);
            }

            // Insert (Area)
            if (!string.IsNullOrEmpty(newArea))
            {
                Area area = game.Simulation.World.Areas.Single(a => a.Name.Equals(newArea));
                if (!area.Items.Contains(item))
                    area.Items.Add(item);

                entry.Area = area;
                entry.Inventory = null;
            }

            // Insert (Inventar)
            if (newInventory.HasValue)
            {
                IInventory inventory = items[newInventory.Value].Item  as IInventory;
                if (!inventory.Inventory.Contains(item))
                    inventory.Inventory.Add(item);

                entry.Area = null;
                entry.Inventory = inventory;
            }
        }

        /// <summary>
        /// Sendet einen Transfer-Request zum Server.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="sender">Sender.</param>
        /// <param name="receiver">Receiver.</param>
        public void SendItemTransfer(Item item, IInventory sender, IInventory receiver)
        {
            try
            {
                writer.Write((byte)MessageType.ClientTransferItem);
                writer.Write((short)12);
                writer.Write(item.Id);
                writer.Write((sender as Item).Id);
                if (receiver != null)
                    writer.Write((receiver as Item).Id);
                else
                    writer.Write(0);
                Flush();
            }
            catch (Exception ex)
            {
                Close(ex, false);
            }
        }

        /// <summary>
        /// Sendet einen Quest-Update Request zum Server.
        /// </summary>
        /// <param name="quest">Quest.</param>
        /// <param name="progress">Progress.</param>
        /// <param name="state">State.</param>
        public void SendQuestUpdate(string quest, string progress, QuestState state)
        {
            try
            {
                writer.Write((byte)MessageType.ClientQuestUpdate);
                writer.Write((short)0);
                writer.Write(quest);
                writer.Write(progress);
                writer.Write((byte)state);

                // Content Länge eintragen
                int contentend = (int)writerStream.Position;
                writerStream.Seek(1, SeekOrigin.Begin);
                writer.Write((short)(contentend - 3));
                writerStream.Seek(contentend, SeekOrigin.Begin);
                Flush();
            }
            catch (Exception ex)
            {
                Close(ex, false);
            }
        }

        /// <summary>
        /// Sendet den aktuellen Inhalt des WriteBuffers an den Server.
        /// </summary>
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

