using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Komponente zur Musik-Wiedergabe
    /// </summary>
    internal class MusicComponent : GameComponent
    {
        private RheinwerkGame game;

        private float volume;

        private Dictionary<string, SoundEffect> songs;

        private SoundEffectInstance currentSong;

        public MusicComponent(RheinwerkGame game) : base(game)
        {
            this.game = game;
            volume = 0.3f;

            songs = new Dictionary<string, SoundEffect>();
            songs.Add("town", game.Content.Load<SoundEffect>("townloop"));
            songs.Add("menu", game.Content.Load<SoundEffect>("menuloop"));

            Play("town");
        }

        /// <summary>
        /// Spielt den angegebenen Song ab.
        /// </summary>
        public void Play(string song)
        {
            // Den laufenden Song stoppen
            if (currentSong != null)
            {
                currentSong.Stop();
                currentSong.Dispose();
            }

            SoundEffect soundEffect;
            if (songs.TryGetValue(song, out soundEffect))
            {
                currentSong = soundEffect.CreateInstance();
                currentSong.IsLooped = true;
                currentSong.Volume = volume;
                currentSong.Play();
            }
        }
    }
}

