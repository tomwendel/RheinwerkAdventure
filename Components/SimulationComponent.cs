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

        /// <summary>
        /// Referenz auf das zentrale Spielmodell.
        /// </summary>
        public World World { get; private set; }

        public SimulationComponent(RheinwerkGame game)
            : base(game)
        {
            this.game = game;

            // Zu Beginn eine neue Spielwelt erzeugen.
            NewGame();
        }

        public void NewGame()
        {
            World = new World();
            World.Areas.AddRange(MapLoader.LoadAll());
        }

        /// <summary>
        /// Fügt den Player an eine der Startstellen ein
        /// </summary>
        /// <returns>Area in die der Spieler eingefügt wurde</returns>
        /// <param name="player">Player-Instanz</param>
        public void InsertPlayer(Player player)
        {
            // Den ersten verfügbaren Startplatz finden und nutzen
            Area target = World.Areas.Where(a => a.Startpoints.Count > 0).First();
            player.Position = target.Startpoints[0];
            target.Items.Add(player);
        }

        public override void Update(GameTime gameTime)
        {
            List<Action> transfers = new List<Action>();
            foreach (var area in World.Areas)
            {
                // Schleife über alle sich aktiv bewegenden Spiel-Elemente
                foreach (var character in area.Items.OfType<Character>())
                {
                    // KI Update
                    if (character.Ai != null)
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
                                //  -> Character sammelt Item ein
                                transfers.Add(() =>
                                    {
                                        area.Items.Remove(item);
                                        (character as IInventory).Inventory.Add(item);
                                        item.Position = Vector2.Zero;
                                    });
                            }
                        }
                    }
                }

                // Kollision mit blockierten Zellen
                foreach (var item in area.Items)
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
                                transfers.Add(() =>
                                    {
                                        area.Items.Remove(item);
                                        destinationArea.Items.Add(item);
                                        item.Position = position;
                                    });
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
                                    interactable.OnInteract(game, interactor, interactable);
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
                                    attackable.OnHit(game, attacker, attackable);
                            }

                            // Schlagerholung anstoßen
                            attacker.Recovery = attacker.TotalRecovery;
                        }
                        attacker.AttackSignal = false;
                    }
                }
            }

            // Transfers durchführen
            foreach (var transfer in transfers)
                transfer();
        }
    }
}

