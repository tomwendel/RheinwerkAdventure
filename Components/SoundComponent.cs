using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace RheinwerkAdventure.Components
{
    internal class SoundComponent : GameComponent
    {
        private RheinwerkGame game;

        private Dictionary<string, SoundEffect> sounds;

        private float volume;

        public SoundComponent(RheinwerkGame game) : base(game)
        {
            this.game = game;
            volume = 0.5f;

            sounds = new Dictionary<string, SoundEffect>();
            sounds.Add("click", game.Content.Load<SoundEffect>("click"));
            sounds.Add("clock", game.Content.Load<SoundEffect>("clock"));
            sounds.Add("coin", game.Content.Load<SoundEffect>("coin"));
            sounds.Add("hit", game.Content.Load<SoundEffect>("hit"));
            sounds.Add("sword", game.Content.Load<SoundEffect>("sword"));
        }

        private void Play(string sound)
        {
            SoundEffect soundEffect;
            if (sounds.TryGetValue(sound, out soundEffect))
                soundEffect.Play(volume, 0f, 0f);
        }

        /// <summary>
        /// Click-Geräusch bei der Selektion im Menü.
        /// </summary>
        public void PlayClick()
        {
            Play("click");
        }

        /// <summary>
        /// Click-Geräusch für ein Interact im Menü.
        /// </summary>
        public void PlayClock()
        {
            Play("clock");
        }

        /// <summary>
        /// Ding-Geräusch beim Einsammeln von Münzen.
        /// </summary>
        public void PlayCoin()
        {
            Play("coin");
        }

        /// <summary>
        /// Spielt den Sound eines schwingenden Schwertes ab.
        /// </summary>
        public void PlaySword()
        {
            Play("sword");
        }

        /// <summary>
        /// Einschlag-Geräusch.
        /// </summary>
        public void PlayHit()
        {
            Play("hit");
        }
    }
}

