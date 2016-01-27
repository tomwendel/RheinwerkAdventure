using System;
using Microsoft.Xna.Framework;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Server Komponente für das Netzwerspiel
    /// </summary>
    internal class ServerComponent : GameComponent
    {
        private RheinwerkGame game;

        public ServerComponent(RheinwerkGame game) : base(game)
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

