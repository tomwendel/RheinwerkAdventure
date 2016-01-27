using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using RheinwerkAdventure.Model;

namespace RheinwerkAdventure.Components
{
    /// <summary>
    /// Komponente zur Musik-Wiedergabe
    /// </summary>
    internal class MusicComponent : GameComponent
    {
        // Gibt die Zeitspanne in ms an die für einen Fade benötigt werden soll.
        private float totalFadeTime = 1500f;

        private RheinwerkGame game;

        // Gibt die maximal-Lautstärke für die Hintergrund-Songs an.
        private float maxVolume;

        // Referenz auf die aktuelle Area
        private Area currentArea;

        // Hält die Liste verfügbarer Songs
        private Dictionary<string, SoundEffect> songs;

        // Hält die Instanz des aktuell laufenden Songs.
        private SoundEffectInstance currentSong = null;

        // Hält den Sound Effect des aktuellen Songs.
        private SoundEffect currentEffect = null;

        // Hält den Sound Effect des kommenden Songs.
        private SoundEffect nextEffect = null;

        // Gibt den Sound Effect des letzten Area-Calls an.
        private SoundEffect areaEffect = null;

        // Gibt an ob das Menü offen ist.
        private bool menu = false;

        public MusicComponent(RheinwerkGame game) : base(game)
        {
            this.game = game;
            maxVolume = 0.3f;

            // Songs laden
            songs = new Dictionary<string, SoundEffect>();
            songs.Add("town", game.Content.Load<SoundEffect>("townloop"));
            songs.Add("menu", game.Content.Load<SoundEffect>("menuloop"));
            songs.Add("wood", game.Content.Load<SoundEffect>("woodloop"));
            songs.Add("house", game.Content.Load<SoundEffect>("houseloop"));
        }

        public override void Update(GameTime gameTime)
        {
            // Nur wenn Komponente aktiviert wurde.
            if (!Enabled)
                return;
            
            // Nur arbeiten, wenn es eine Welt, einen Player und eine aktive Area gibt.
            Area area = game.Local.GetCurrentArea();
            if (currentArea != area)
            {
                currentArea = area;
                if (currentArea != null)
                    Play(currentArea.Song);
            }

            // Override verhindern
            if (currentEffect == nextEffect)
                nextEffect = null;

            // Ausfaden
            if (currentEffect != null && nextEffect != null)
            {
                float currentVolume = currentSong.Volume;
                currentVolume -= (float)gameTime.ElapsedGameTime.TotalMilliseconds / totalFadeTime;
                if (currentVolume <= 0f)
                {
                    // Ausschalten
                    currentSong.Volume = 0;
                    currentSong.Stop();
                    currentSong.Dispose();
                    currentSong = null;
                    currentEffect = null;
                }
                else
                {
                    // Leiser
                    currentSong.Volume = currentVolume;
                }
            }

            // Einschalten
            if (currentEffect == null && nextEffect != null)
            {
                currentEffect = nextEffect;
                nextEffect = null;

                // Initialisieren mit 0 Lautstärke
                currentSong = currentEffect.CreateInstance();
                currentSong.IsLooped = true;
                currentSong.Volume = 0f;
                currentSong.Play();
            }

            // Einfaden
            if (currentEffect != null && nextEffect == null && currentSong.Volume < maxVolume)
            {
                float currentVolume = currentSong.Volume;
                currentVolume += (float)gameTime.ElapsedGameTime.TotalMilliseconds / totalFadeTime;
                currentVolume = Math.Min(currentVolume, maxVolume);
                currentSong.Volume = currentVolume;
            }
        }

        /// <summary>
        /// Spielt den angegebenen Song ab.
        /// </summary>
        private void Play(string song)
        {
            SoundEffect soundEffect;
            if (songs.TryGetValue(song, out soundEffect))
            {
                // Den Area-Effect auf diesen Effect einstellen
                areaEffect = soundEffect;

                // Sollte das Menü nicht offen sein wird der Song als nächsten einreihen
                if (!menu)
                    nextEffect = soundEffect;
            }
        }

        public void OpenMenu()
        {
            // Menu-Song einreihen
            nextEffect = songs["menu"];
            menu = true;
        }

        public void CloseMenu()
        {
            // Den vorherigen Song einreihen
            nextEffect = areaEffect;
            menu = false;
        }
    }
}

