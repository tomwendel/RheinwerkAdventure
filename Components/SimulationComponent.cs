using System;
using System.Linq;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Model;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using RheinwerkAdventure.Screens;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Game Komponente zur ständigen Berechnung des Spielverlaufs im Model.
    /// </summary>
    internal class SimulationComponent : GameComponent
    {
        // Sicherheitslücke gegen Rundungsfehler
        private float gap = 0.00001f;

        private readonly RheinwerkGame game;

        private readonly Random rand = new Random();

        /// <summary>
        /// Referenz auf das zentrale Spielmodell.
        /// </summary>
        public World World { get; private set; }

        /// <summary>
        /// Modus in dem die Simulation läuft
        /// </summary>
        /// <value>The mode.</value>
        public SimulationMode Mode { get; private set; }

        public SimulationComponent(RheinwerkGame game)
            : base(game)
        {
            this.game = game;
        }

        /// <summary>
        /// Startet ein neues Spiel
        /// </summary>
        /// <param name="mode">Simulationsmodus</param>
        public void NewGame(SimulationMode mode)
        {
            World = new World();
            Mode = mode;
            int nextId = World.NextId;
            World.Areas.AddRange(MapLoader.LoadAll(ref nextId));
            World.NextId = nextId;

            // Quests erstellen
            Quest quest = new Quest()
            {
                Name = "Heidis Quest",
            };
            World.Quests.Add(quest);

            quest.QuestProgresses.Add(new QuestProgress() { Id = "search", Description = "Gehe auf die Suche nach der goldenen Muenze" });
            quest.QuestProgresses.Add(new QuestProgress() { Id = "return", Description = "Bring die Muenze zurueck" });
            quest.QuestProgresses.Add(new QuestProgress() { Id = "success", Description = "Das Dorf wird dir ewig dankbar sein" });
            quest.QuestProgresses.Add(new QuestProgress() { Id = "fail", Description = "Die Muenze ist fuer immer verloren" });
        }

        /// <summary>
        /// Beendet das aktuelle Spiel.
        /// </summary>
        public void CloseGame()
        {
            World = null;
            Mode = SimulationMode.None;
        }

        /// <summary>
        /// Fügt den Player an eine der Startstellen ein
        /// </summary>
        /// <returns>Area in die der Spieler eingefügt wurde</returns>
        /// <param name="player">Player-Instanz</param>
        public void InsertPlayer(Player player)
        {
            Vector2 x = new Vector2((float)rand.NextDouble() - 0.5f, (float)rand.NextDouble() - 0.5f);

            // Den ersten verfügbaren Startplatz finden und nutzen
            Area target = World.Areas.Where(a => a.Startpoints.Count > 0).First();
            player.Position = target.Startpoints[0] + x;
            target.Items.Add(player);
        }

        /// <summary>
        /// Entfernt den angegebenen Spieler wieder aus dem vorhandenen Spiel.
        /// </summary>
        /// <param name="player">Zu entfernenden Spieler</param>
        public void RemovePlayer(Player player)
        {
            if (World == null)
                return;
            
            foreach (var area in World.Areas)
            {
                if (area.Items.Contains(player))
                {
                    area.Items.Remove(player);
                    break;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Nur wenn Komponente aktiviert wurde.
            if (!Enabled)
                return;

            // Nur berechnen, falls eine Welt aktiv ist.
            if (World == null)
                return;

            List<Action> transfers = new List<Action>();
            foreach (var area in World.Areas)
            {
                // Schleife über alle sich aktiv bewegenden Spiel-Elemente
                foreach (var character in area.Items.OfType<Character>().ToArray())
                {
                    // Tote Charactere ignorieren
                    if (character is IAttackable && (character as IAttackable).Hitpoints <= 0)
                        continue;

                    // KI Update
                    if (character.Ai != null && Mode != SimulationMode.Client)
                        character.Ai.Update(area, gameTime);
                    
                    // Neuberechnung der Character-Position.
                    character.move += character.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Attacker identifizieren
                    IAttacker attacker = null;
                    if (character is IAttacker)
                    {
                        attacker = (IAttacker)character;
                        attacker.AttackableItems.Clear();

                        // Recovery-Time aktualisieren
                        attacker.Recovery -= gameTime.ElapsedGameTime;
                        if (attacker.Recovery < TimeSpan.Zero)
                            attacker.Recovery = TimeSpan.Zero;
                    }

                    // Interactor identifizieren
                    IInteractor interactor = null;
                    if (character is IInteractor)
                    {
                        interactor = (IInteractor)character;
                        interactor.InteractableItems.Clear();
                    }

                    // Kollisionsprüfung mit allen restlichen Items.
                    foreach (var item in area.Items)
                    {
                        // Kollision mit sich selbst ausschließen
                        if (item == character)
                            continue;

                        // Distanz berechnen
                        Vector2 distance = (item.Position + item.move) - (character.Position + character.move);

                        // Ermittlung der angreifbaren Items.
                        if (attacker != null &&
                            item is IAttackable &&
                            distance.Length() - attacker.AttackRange - item.Radius < 0f)
                        {
                            attacker.AttackableItems.Add(item as IAttackable);
                        }

                        // Ermittlung der interagierbaren Items.
                        if (interactor != null &&
                            item is IInteractable &&
                            distance.Length() - interactor.InteractionRange - item.Radius < 0f)
                        {
                            interactor.InteractableItems.Add(item as IInteractable);
                        }

                        // Überschneidung berechnen & darauf reagieren
                        float overlap = item.Radius + character.Radius - distance.Length();
                        if (overlap > 0f)
                        {
                            Vector2 resolution = distance * (overlap / distance.Length());
                            if (item.Fixed && !character.Fixed)
                            {
                                // Item fixiert
                                character.move -= resolution;
                            }
                            else if (!item.Fixed && character.Fixed)
                            {
                                // Character fixiert
                                item.move += resolution;
                            }
                            else if (!item.Fixed && !character.Fixed)
                            {
                                // keiner fixiert
                                float totalMass = item.Mass + character.Mass;
                                character.move -= resolution * (item.Mass / totalMass);
                                item.move += resolution * (character.Mass / totalMass);
                            }

                            // Kombination aus Collectable und Iventory
                            if (item is ICollectable && character is IInventory)
                            {
                                ICollectable collectable = item as ICollectable;

                                //  -> Character sammelt Item ein
                                if (Mode != SimulationMode.Client)
                                {
                                    transfers.Add(() =>
                                        {
                                            if (area.Items.Contains(item))
                                                area.Items.Remove(item);
                                        
                                            IInventory inventory = character as IInventory;
                                            if (!inventory.Inventory.Contains(item))
                                            {
                                                inventory.Inventory.Add(item);
                                                item.Position = Vector2.Zero;
                                            }
                                        });
                                }

                                // Event aufrufen
                                if (collectable.OnCollect != null)
                                    collectable.OnCollect(this, item);
                            }
                        }
                    }
                }

                // Kollision mit blockierten Zellen
                foreach (var item in area.Items.ToArray())
                {
                    bool collision = false;
                    int loops = 0;

                    // Standard-Update für das Item
                    if (item.Update != null)
                        item.Update(game, area, item, gameTime);

                    do
                    {
                        // Grenzbereiche für die zu überprüfenden Zellen ermitteln
                        Vector2 position = item.Position + item.move;
                        int minCellX = (int)(position.X - item.Radius);
                        int maxCellX = (int)(position.X + item.Radius);
                        int minCellY = (int)(position.Y - item.Radius);
                        int maxCellY = (int)(position.Y + item.Radius);

                        collision = false;
                        float minImpact = 2f;
                        int minAxis = 0;

                        // Schleife über alle betroffenen Zellen zur Ermittlung der ersten Kollision
                        for (int x = minCellX; x <= maxCellX; x++)
                        {
                            for (int y = minCellY; y <= maxCellY; y++)
                            {
                                // Zellen ignorieren die den Spieler nicht blockieren
                                if (!area.IsCellBlocked(x, y))
                                    continue;

                                // Zellen ignorieren die vom Spieler nicht berührt werden
                                if (position.X - item.Radius > x + 1 ||
                                    position.X + item.Radius < x ||
                                    position.Y - item.Radius > y + 1 ||
                                    position.Y + item.Radius < y)
                                    continue;

                                collision = true;

                                // Kollisionszeitpunkt auf X-Achse ermitteln
                                float diffX = float.MaxValue;
                                if (item.move.X > 0)
                                    diffX = position.X + item.Radius - x + gap;
                                if (item.move.X < 0)
                                    diffX = position.X - item.Radius - (x + 1) - gap;
                                float impactX = 1f - (diffX / item.move.X);

                                // Kollisionszeitpunkt auf Y-Achse ermitteln
                                float diffY = float.MaxValue;
                                if (item.move.Y > 0)
                                    diffY = position.Y + item.Radius - y + gap;
                                if (item.move.Y < 0)
                                    diffY = position.Y - item.Radius - (y + 1) - gap;
                                float impactY = 1f - (diffY / item.move.Y);

                                // Relevante Achse ermitteln
                                // Ergibt sich aus dem spätesten Kollisionszeitpunkt
                                int axis = 0;
                                float impact = 0;
                                if (impactX > impactY)
                                {
                                    axis = 1;
                                    impact = impactX;
                                }
                                else
                                {
                                    axis = 2;
                                    impact = impactY;
                                }

                                // Ist diese Kollision eher als die bisher erkannten
                                if (impact < minImpact)
                                {
                                    minImpact = impact;
                                    minAxis = axis;
                                }
                            }
                        }

                        // Im Falle einer Kollision in diesem Schleifendurchlauf...
                        if (collision)
                        {
                            // X-Anteil ab dem Kollisionszeitpunkt kürzen
                            if (minAxis == 1)
                                item.move *= new Vector2(minImpact, 1f);

                            // Y-Anteil ab dem Kollisionszeitpunkt kürzen
                            if (minAxis == 2)
                                item.move *= new Vector2(1f, minImpact);
                        }
                        loops++;
                    }
                    while(collision && loops < 2);

                    // Finaler Move-Vektor auf die Position anwenden.
                    item.Position += item.move;
                    item.move = Vector2.Zero;

                    // Portal anwenden (nur Player)
                    if (area.Portals != null && item is Player)
                    {
                        Player player = item as Player;
                        bool inPortal = false;

                        foreach (var portal in area.Portals)
                        {
                            if (item.Position.X > portal.Box.Left &&
                                item.Position.X <= portal.Box.Right &&
                                item.Position.Y > portal.Box.Top &&
                                item.Position.Y <= portal.Box.Bottom)
                            {
                                inPortal = true;
                                if (player.InPortal)
                                    continue;

                                // Ziel-Area und Portal finden
                                Area destinationArea = World.Areas.First(a => a.Name.Equals(portal.DestinationArea));
                                Portal destinationPortal = destinationArea.Portals.First(p => p.DestinationArea.Equals(area.Name));

                                // Neue Position des Spielers finden
                                Vector2 position = new Vector2(
                                                       destinationPortal.Box.X + (destinationPortal.Box.Width / 2f), 
                                                       destinationPortal.Box.Y + (destinationPortal.Box.Height / 2f));

                                // Transfer in andere Area vorbereiten
                                if (Mode != SimulationMode.Client)
                                {
                                    transfers.Add(() =>
                                        {
                                            if (area.Items.Contains(item))
                                                area.Items.Remove(item);

                                            if (!destinationArea.Items.Contains(item))
                                            {
                                                destinationArea.Items.Add(item);
                                                item.Position = position;
                                            }
                                        });
                                }
                            }
                        }

                        player.InPortal = inPortal;
                    }

                    // Interaktionen durchführen
                    if (item is IInteractor)
                    {
                        IInteractor interactor = item as IInteractor;
                        if (interactor.InteractSignal)
                        {
                            // Alle Items in der Nähe aufrufen
                            foreach (var interactable in interactor.InteractableItems)
                            {
                                if (interactable.OnInteract != null)
                                    interactable.OnInteract(this, interactor, interactable);
                            }
                        }
                        interactor.InteractSignal = false;
                    }

                    // Angriff durchführen
                    if (item is IAttacker)
                    {
                        IAttacker attacker = item as IAttacker;
                        if (attacker.AttackSignal && attacker.Recovery <= TimeSpan.Zero)
                        {
                            // Alle Items in der Nähe schlagen
                            foreach (var attackable in attacker.AttackableItems)
                            {
                                attackable.Hitpoints -= attacker.AttackValue;
                                if (attackable.OnHit != null)
                                    attackable.OnHit(this, attacker, attackable);
                            }

                            // Schlagerholung anstoßen
                            attacker.Recovery = attacker.TotalRecovery;
                        }
                        attacker.AttackSignal = false;
                    }
                }
            }

            // Transfers durchführen
            if (Mode != SimulationMode.Client)
                foreach (var transfer in transfers)
                    transfer();
        }

        /// <summary>
        /// Setzt den Fortschritt des Quests.
        /// </summary>
        public void SetQuestProgress(string quest, string progress)
        {
            SetQuestProgress(quest, progress, QuestState.Active);
        }

        /// <summary>
        /// Markiert das Quest als gescheitert.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void SetQuestFail(string quest, string progress)
        {
            SetQuestProgress(quest, progress, QuestState.Failed);
        }

        /// <summary>
        /// Markiert das Quest als erfolgreich beendet.
        /// </summary>
        public void SetQuestSuccess(string quest, string progress)
        {
            SetQuestProgress(quest, progress, QuestState.Succeeded);
        }
            
        private void SetQuestProgress(string quest, string progress, QuestState state)
        {
            if (Mode != SimulationMode.Client)
            {
                var qu = World.Quests.SingleOrDefault(q => q.Name == quest);
                qu.CurrentProgress = qu.QuestProgresses.FirstOrDefault(q => q.Id.Equals(progress));
                qu.State = state;
            }
            else
            {
                // Zum Server schicken
                game.Client.SendQuestUpdate(quest, progress, state);
            }
        }

        /// <summary>
        /// Zeigt einen Interaktionsdialog an.
        /// </summary>
        public void ShowInteractionScreen(Player player, Screen screen)
        {
            if (player == game.Local.Player)
            {
                game.Screen.ShowScreen(screen);
            }
        }

        /// <summary>
        /// Transferiert ein Item von einem Inventar zum anderen.
        /// </summary>
        /// <param name="item">Betroffenes item</param>
        /// <param name="sender">Sender</param>
        /// <param name="receiver">Empfänger</param>
        public void Transfer(Item item, IInventory sender, IInventory receiver)
        {
            if (Mode != SimulationMode.Client)
            {
                // entfernen, falls vorhanden
                if (sender != null && sender.Inventory.Contains(item))
                    sender.Inventory.Remove(item);

                // Einfügen, falls noch nicht vorhanden
                if (receiver != null && !receiver.Inventory.Contains(item))
                    receiver.Inventory.Add(item);
            }
            else
            {
                // Zum Server schicken
                game.Client.SendItemTransfer(item, sender, receiver);
            }
        }
    }

    /// <summary>
    /// Liste von Modi in denen die Simulation laufen kann.
    /// </summary>
    internal enum SimulationMode
    {
        /// <summary>
        /// Simulation ist nicht aktiv.
        /// </summary>
        None,

        /// <summary>
        /// Singleplayer Mode
        /// </summary>
        Single,

        /// <summary>
        /// Server Modus
        /// </summary>
        Server,

        /// <summary>
        /// Multiplayer Client
        /// </summary>
        Client
    }
}

