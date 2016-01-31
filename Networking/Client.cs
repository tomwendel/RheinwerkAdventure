using System;
using System.Net.Sockets;
using RheinwerkAdventure.Model;
using RheinwerkAdventure.Components;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace RheinwerkAdventure.Networking
{
    /// <summary>
    /// Die Repräsentanz eines Clients auf Server-Seite.
    /// </summary>
    internal class Client
    {
        /// <summary>
        /// Puffergröße
        /// </summary>
        private const int BUFFERSIZE = 512;

        /// <summary>
        /// TCP Connection zum Client.
        /// </summary>
        private TcpClient client;

        /// <summary>
        /// Network Stream zum Client.
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
        /// Binary Writer für den Schreibpuffer
        /// </summary>
        private BinaryWriter writer;

        /// <summary>
        /// Stream-Wrapper für den Schreibpuffer
        /// </summary>
        private MemoryStream writerStream;

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
        /// Nachrichten-Schlange für die Synchronisation.
        /// </summary>
        private ConcurrentQueue<Message> messages;

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

            writerBuffer = new byte[BUFFERSIZE];
            writerStream = new MemoryStream(writerBuffer);
            writer = new BinaryWriter(writerStream);

            messages = new ConcurrentQueue<Message>();

            // Hallo senden
            try
            {
                writer.Write((byte)MessageType.ServerHello);
                writer.Write((short)4);
                writer.Write(clientId);
                Flush();
                Connected = true;

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
        /// Update 
        /// </summary>
        /// <param name="simulation">Simulation.</param>
        /// <param name="items">Items.</param>
        public void Update(SimulationComponent simulation, Dictionary<int, ItemCacheEntry> items)
        {
            Message m;
            while (messages.TryDequeue(out m))
            {
                switch (m.MessageType)
                {
                    case MessageType.ClientClose:
                        Close(null, true);
                        break;
                    case MessageType.ClientUpdateVelocity:
                        Player.Velocity = new Vector2(
                            BitConverter.ToSingle(m.Payload, 0), 
                            BitConverter.ToSingle(m.Payload, 4));
                        break;
                    case MessageType.ClientTriggerAttack:
                        Player.AttackSignal = true;
                        break;
                    case MessageType.ClientQuestUpdate:
                        using (MemoryStream stream = new MemoryStream(m.Payload))
                        {
                            using (BinaryReader br = new BinaryReader(stream))
                            {
                                string quest = br.ReadString();
                                string progress = br.ReadString();
                                QuestState state = (QuestState)br.ReadByte();
                                switch (state)
                                {
                                    case QuestState.Active:
                                        simulation.SetQuestProgress(quest, progress);
                                        break;
                                    case QuestState.Failed:
                                        simulation.SetQuestFail(quest, progress);
                                        break;
                                    case QuestState.Succeeded:
                                        simulation.SetQuestSuccess(quest, progress);
                                        break;
                                }
                            }
                        }
                        break;
                    case MessageType.ClientTransferItem:
                        int itemId = BitConverter.ToInt32(m.Payload, 0);
                        int senderId = BitConverter.ToInt32(m.Payload, 4);
                        int receiverId = BitConverter.ToInt32(m.Payload, 8);

                        Item item = null;
                        if (items.ContainsKey(itemId))
                            item = items[itemId].Item;

                        Item sender = null;
                        if (items.ContainsKey(senderId))
                            sender = items[senderId].Item;

                        Item receiver = null;
                        if (items.ContainsKey(receiverId))
                            receiver = items[receiverId].Item;

                        if (item != null && sender != null)
                            simulation.Transfer(item, sender as IInventory, receiver as IInventory);

                        break;
                }
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
                    // Payload auslesen   
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
                }

                stream.BeginRead(readerBuffer, nextMessageCollectedBytes, nextMessageRequiredBytes, ReadCallback, null);
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
                writer.Write((short)4);
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
        public void SendStart(int id)
        {
            try
            {
                writer.Write((byte)MessageType.ServerStartGame);
                writer.Write((short)4);
                writer.Write(id);
                Flush();
            }
            catch (Exception ex)
            {
                Close(ex, false);
            }
        }

        /// <summary>
        /// Sendet einen Insert-Auftrag an den Client.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="area">Area.</param>
        /// <param name="inventory">Inventory.</param>
        public void SendInsert(Item item, Area area, IInventory inventory)
        {
            try
            {
                if (area != null)
                {
                    // Area[string];Type[string];Id[int];Payload[byte[]]
                    writer.Write((byte)MessageType.ServerInsertItemToArea);
                    writer.Write((short)0);
                    writer.Write(area.Name);
                }
                else
                {
                    // InventoryId[int];Type[string];Id[int];Payload[byte[]]
                    writer.Write((byte)MessageType.ServerInsertItemToArea);
                    writer.Write((short)0);
                    writer.Write((inventory as Item).Id);
                }

                // Type ermitteln
                writer.Write(item.GetType().FullName);
                writer.Write(item.Id);

                // Payload ermitteln und erstellen
                item.SerializeInsert(writer);
                int contentend = (int)writerStream.Position;

                // Content Länge eintragen
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
        /// Sendet ein KeyUpdate zu diesem Item an diesen Client.
        /// </summary>
        /// <param name="item">Item.</param>
        public void SendKeyUpdate(Item item)
        {
            try
            {
                // Id[int];Payload[byte[]]
                writer.Write((byte)MessageType.ServerKeyUpdateItem);
                writer.Write((short)0);
                writer.Write(item.Id);

                // Payload ermitteln und erstellen
                item.SerializeKeyUpdate(writer);
                int contentend = (int)writerStream.Position;

                // Content Länge eintragen
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
        /// Sendet ein normales Update zum angegebenen Item an diesen Client.
        /// </summary>
        /// <param name="item">Item.</param>
        public void SendUpdate(Item item)
        {
            try
            {
                // Id[int];Payload[byte[]]
                writer.Write((byte)MessageType.ServerUpdateItem);
                writer.Write((short)0);
                writer.Write(item.Id);

                // Payload ermitteln und erstellen
                item.SerializeUpdate(writer);
                int contentend = (int)writerStream.Position;

                // Content Länge eintragen
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
        /// Sendet ein Remove-Kommando für das Item mit der angegebenen Id an diesen Client.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void SendRemove(int id)
        {
            try
            {
                // Id[int]
                writer.Write((byte)MessageType.ServerRemoveItem);
                writer.Write((short)4);
                writer.Write(id);
                Flush();
            }
            catch (Exception ex)
            {
                Close(ex, false);
            }
        }

        /// <summary>
        /// Sendet einen Area/Inventar-Transfer des angegebenen Items an diesen Client.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="oldArea">Old area.</param>
        /// <param name="newArea">New area.</param>
        /// <param name="oldInventory">Old inventory.</param>
        /// <param name="newInventory">New inventory.</param>
        public void SendMove(Item item, Area oldArea, Area newArea, IInventory oldInventory, IInventory newInventory)
        {
            if (oldArea != null)
            {
                if (newArea != null)
                {
                    // Area to Area
                    // ItemId[int];OldArea[string];NewArea[string]
                    writer.Write((byte)MessageType.ServerMoveAreaToArea);
                    writer.Write((short)0);
                    writer.Write(item.Id);
                    writer.Write(oldArea.Name);
                    writer.Write(newArea.Name);
                }
                else
                {
                    // Area to Inventory
                    // ItemId[int];OldArea[string];InventoryId[int]
                    writer.Write((byte)MessageType.ServerMoveAreaToInventory);
                    writer.Write((short)0);
                    writer.Write(item.Id);
                    writer.Write(oldArea.Name);
                    writer.Write((newInventory as Item).Id);
                }
            }
            else
            {
                if (newArea != null)
                {
                    // Inventory To Area
                    // ItemId[int];InventoryId[int];NewArea[string]
                    writer.Write((byte)MessageType.ServerMoveInventoryToArea);
                    writer.Write((short)0);
                    writer.Write(item.Id);
                    writer.Write((oldInventory as Item).Id);
                    writer.Write(newArea.Name);
                }
                else
                {
                    // Inventory To Inventory
                    // ItemId[int];OldInventoryId[int];NewInventoryId[int]
                    writer.Write((byte)MessageType.ServerMoveInventoryToInventory);
                    writer.Write((short)0);
                    writer.Write(item.Id);
                    writer.Write((oldInventory as Item).Id);
                    writer.Write((newInventory as Item).Id);
                }
            }

            int contentend = (int)writerStream.Position;
            writerStream.Seek(1, SeekOrigin.Begin);
            writer.Write((short)(contentend - 3));
            writerStream.Seek(contentend, SeekOrigin.Begin);

            Flush();
        }

        /// <summary>
        /// Sendet ein Quest Update zum angegebenen Quest an diesen Client.
        /// </summary>
        /// <param name="quest">Quest.</param>
        /// <param name="progress">Progress.</param>
        /// <param name="state">State.</param>
        public void SendQuestUpdate(string quest, string progress, QuestState state)
        {
            try
            {
                // QuestId[string];Progress[string];State[byte]
                writer.Write((byte)MessageType.ServerQuestUpdate);
                writer.Write((short)0);
                writer.Write(quest);
                writer.Write(progress);
                writer.Write((byte)state);

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
        /// Schließt die Verbindung zu diesem Client.
        /// </summary>
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
                    writer.Write((short)0);
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

