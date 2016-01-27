using System;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Client für eine Netzwerk-Verbindung.
    /// </summary>
    internal class ClientComponent : GameComponent
    {
        private RheinwerkGame game;

        public ClientComponent(RheinwerkGame game) : base(game)
        {
            this.game = game;
        }

        public override void Update(GameTime gameTime)
        {
            // Nur wenn Komponente aktiviert wurde.
            if (!Enabled)
                return;
        }
    }
}

