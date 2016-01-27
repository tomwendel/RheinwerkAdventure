using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RheinwerkAdventure.Model;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using RheinwerkAdventure.Rendering;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Game Komponente zur Ausgabe der Spiel-Szene an den Bildschirm.
    /// </summary>
    internal class SceneComponent : DrawableGameComponent
    {
        private readonly string mapPath = Path.Combine(Environment.CurrentDirectory, "Maps");

        private readonly string contentPath = Path.Combine(Environment.CurrentDirectory, "Content");

        private readonly RheinwerkGame game;

        private readonly Dictionary<string, Texture2D> tilesetTextures;

        private readonly Dictionary<string, Texture2D> itemTextures;

        private readonly Dictionary<Item, ItemRenderer> itemRenderer;

        private SpriteBatch spriteBatch;

        private SpriteFont font;

        private Texture2D background;

        private Area currentArea = null;

        /// <summary>
        /// Kamera-Einstellungen für diese Szene.
        /// </summary>
        public Camera Camera { get; private set; }

        public SceneComponent(RheinwerkGame game)
            : base(game)
        {
            this.game = game;
            tilesetTextures = new Dictionary<string, Texture2D>();
            itemTextures = new Dictionary<string, Texture2D>();
            itemRenderer = new Dictionary<Item, ItemRenderer>();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Camera = new Camera(GraphicsDevice.Viewport.Bounds.Size);
            font = Game.Content.Load<SpriteFont>("HudFont");
            background = Game.Content.Load<Texture2D>("background");
        }

        public Texture2D GetTileset(string name)
        {
            // Leere Strings ignorieren
            if (string.IsNullOrEmpty(name))
                return null;

            // Bereits geladene Texturen ignorieren
            Texture2D result;
            if (!tilesetTextures.TryGetValue(name, out result))
            {
                // Textur nachladen
                using (Stream stream = File.OpenRead(mapPath + "\\" + name))
                {
                    result = Texture2D.FromStream(GraphicsDevice, stream);
                    tilesetTextures.Add(name, result);
                }
            }

            return result;
        }

        public Texture2D GetItemTexture(string name)
        {
            // Leere Strings ignorieren
            if (string.IsNullOrEmpty(name))
                return null;

            // Bereits geladene Texturen ignorieren
            Texture2D result;
            if (!itemTextures.TryGetValue(name, out result))
            {
                // Textur nachladen
                using (Stream stream = File.OpenRead(contentPath + "\\" + name))
                {
                    result = Texture2D.FromStream(GraphicsDevice, stream);
                    itemTextures.Add(name, result);
                }
            }

            return result;
        }

        public override void Update(GameTime gameTime)
        {
            // Nur wenn Komponente aktiviert wurde.
            if (!Enabled)
                return;

            // Nur arbeiten, wenn es eine Welt, einen Player und eine aktive Area gibt.
            Area area = game.Local.GetCurrentArea();
            if (game.Simulation.World == null || game.Local.Player == null || area == null)
                return;

            if (currentArea != area)
            {
                // Aktuelle Area wechseln
                currentArea = area;

                // Initiale Kameraposition (temporär)
                Vector2 areaSize = new Vector2(currentArea.Width, currentArea.Height);
                Camera.SetFocusExplizit(game.Local.Player.Position, areaSize);
            }

            // Platziert den Kamerafokus auf den Spieler.
            Camera.SetFocus(game.Local.Player.Position);
        }

        public override void Draw(GameTime gameTime)
        {
            // Nur wenn Komponente sichtbar ist.
            if (!Visible)
                return;

            // Nur arbeiten, wenn es eine Welt, einen Player und eine aktive Area gibt.
            Area area = game.Local.GetCurrentArea();
            if (game.Simulation.World == null || game.Local.Player == null || area == null)
            {
                // Standard-Hintergrund
                spriteBatch.Begin(samplerState: SamplerState.LinearWrap);
                spriteBatch.Draw(background, GraphicsDevice.Viewport.Bounds, GraphicsDevice.Viewport.Bounds, Color.White);
                spriteBatch.End();
                return;
            }
            
            // Bildschirm leeren
            GraphicsDevice.Clear(currentArea.Background);

            spriteBatch.Begin();

            // Berechnet den Render-Offset mit Hilfe der Kamera-Einstellungen
            Point offset = (Camera.Offset * Camera.Scale).ToPoint();

            // Alle Layer der Render-Reihenfolge nach durchlaufen
            for (int l = 0; l < currentArea.Layers.Length; l++)
            {
                RenderLayer(currentArea, currentArea.Layers[l], offset);
                if (l == 4)
                    RenderItems(currentArea, offset, gameTime);
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Rendert einen Layer der aktuellen Szene
        /// </summary>
        private void RenderLayer(Area area, Layer layer, Point offset)
        {
            // TODO: Nur den sichtbaren Bereich rendern
            for (int x = 0; x < area.Width; x++)
            {
                for (int y = 0; y < area.Height; y++)
                {
                    // Prüfen, ob diese Zelle ein Tile enthält
                    int tileId = layer.Tiles[x, y];
                    if (tileId == 0)
                        continue;

                    // Tile ermitteln
                    Tile tile = area.Tiles[tileId];
                    Texture2D texture = GetTileset(tile.Texture);

                    // Position ermitteln
                    int offsetX = (int)(x * Camera.Scale) - offset.X;
                    int offsetY = (int)(y * Camera.Scale) - offset.Y;

                    // Zelle mit der Standard-Textur (Gras) ausmalen
                    spriteBatch.Draw(texture, new Rectangle(offsetX, offsetY, (int)Camera.Scale, (int)Camera.Scale), tile.SourceRectangle, Color.White);
                }
            }
        }

        /// <summary>
        /// Rendert die Spielelemente der aktuellen Szene
        /// </summary>
        private void RenderItems(Area area, Point offset, GameTime gameTime)
        {
            // Items von hinten nach vorne rendern
            foreach (var item in area.Items.OrderBy(i => i.Position.Y))
            {
                // Renderer ermitteln und ggf. neu erzeugen
                ItemRenderer renderer;
                if (!itemRenderer.TryGetValue(item, out renderer))
                {
                    // ACHTUNG: Hier können potentiell neue Items nachträglich hinzu kommen zu denen die Textur noch fehlt
                    // Das muss geprüft und ggf nachgeladen werden.
                    Texture2D texture = GetItemTexture(item.Texture);
                    
                    if (item is Character)
                        renderer = new CharacterRenderer(item as Character, Camera, texture, font);
                    else
                        renderer = new SimpleItemRenderer(item, Camera, texture, font);

                    itemRenderer.Add(item, renderer);
                }

                // Ermitteln, ob Item im Interaktionsbereich ist
                bool highlight = false;
                if (item is IInteractable && game.Local.Player.InteractableItems.Contains(item as IInteractable) ||
                    item is IAttackable && game.Local.Player.AttackableItems.Contains(item as IAttackable))
                    highlight = true;

                // Item rendern
                renderer.Draw(spriteBatch, offset, gameTime, highlight);
            }

            // TODO: Nicht mehr verwendete Renderer entfernen
        }
    }
}

