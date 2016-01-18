using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Komponente zur Musik-Wiedergabe
    /// </summary>
    internal class MusicComponent : GameComponent
    {
        private RheinwerkGame game;

        private float volume;

        private SoundEffect town;

        private SoundEffectInstance currentSong;

        public MusicComponent(RheinwerkGame game) : base(game)
        {
            this.game = game;
            volume = 0.3f;

            town = game.Content.Load<SoundEffect>("townloop");

            currentSong = town.CreateInstance();
            currentSong.IsLooped = true;
            currentSong.Volume = volume;
            currentSong.Play();
        }
    }
}

