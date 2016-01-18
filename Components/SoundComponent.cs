using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace RheinwerkAdventure.Components
{
    internal class SoundComponent : GameComponent
    {
        private RheinwerkGame game;

        private SoundEffect click;

        private float volume;

        public SoundComponent(RheinwerkGame game) : base(game)
        {
            this.game = game;
            volume = 0.5f;

            click = game.Content.Load<SoundEffect>("click");
        }

        public void PlayClick()
        {
            click.Play(volume, 0f, 0f);
        }
    }
}

