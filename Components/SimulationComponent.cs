using System;
using System.Linq;
using Microsoft.Xna.Framework;
using RheinwerkAdventure.Model;

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

        /// <summary>
        /// Referenz auf den aktuellen Spieler.
        /// </summary>
        public Player Player { get; private set; }

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

            // Die erste Area mit leeren Tiles füllen.
            Area area = new Area(2, 30, 20);
            World.Areas.Add(area);
            for (int x = 0; x < area.Width; x++)
            {
                for (int y = 0; y < area.Height; y++)
                {
                    area.Layers[0].Tiles[x, y] = new Tile();
                    area.Layers[1].Tiles[x, y] = new Tile();

                    if (x == 0 || y == 0 || x == area.Width - 1 || y == area.Height - 1)
                        area.Layers[0].Tiles[x, y].Blocked = true;
                }
            }

            // Den Spieler einfügen.
            Player = new Player() { Position = new Vector2(15, 10), Radius = 0.25f };
            area.Items.Add(Player);

            // Einen Diamanten einfügen.
            Diamant diamant = new Diamant() { Position = new Vector2(10, 10), Radius = 0.25f };
            area.Items.Add(diamant);
        }

        public override void Update(GameTime gameTime)
        {
            #region Player Input

            Player.Velocity = game.Input.Movement * 10f;

            #endregion

            #region Character Movement

            foreach (var area in World.Areas)
            {
                // Schleife über alle sich aktiv bewegenden Spiel-Elemente
                foreach (var character in area.Items.OfType<Character>())
                {
                    // Neuberechnung der Character-Position.
                    character.move += character.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Attacker identifizieren
                    IAttacker attacker = null;
                    if (character is IAttacker)
                    {
                        attacker = (IAttacker)character;
                        attacker.AttackableItems.Clear();
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
                            attacker.AttackableItems.Add(item);
                        }

                        // Ermittlung der interagierbaren Items.
                        if (interactor != null &&
                            item is IInteractable &&
                            distance.Length() - interactor.InteractionRange - item.Radius < 0f)
                        {
                            interactor.InteractableItems.Add(item);
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
                        }
                    }
                }

                // Kollision mit blockierten Zellen
                foreach (var item in area.Items)
                {
                    bool collision = false;
                    int loops = 0;

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

                }
            }

            #endregion

            base.Update(gameTime);
        }
    }
}

