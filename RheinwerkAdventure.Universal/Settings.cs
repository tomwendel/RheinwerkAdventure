using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace RheinwerkAdventure
{
    internal class Settings
    {
        private int musicVolume;

        private int soundVolume;

        /// <summary>
        /// Adresse zum Server.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Musik-Lautstärke
        /// </summary>
        public int MusicVolume
        {
            get { return musicVolume; }
            set { musicVolume = Math.Min(10, Math.Max(0, value)); }
        }

        /// <summary>
        /// Sound-Lautstärke
        /// </summary>
        public int SoundVolume
        {
            get { return soundVolume; }
            set { soundVolume = Math.Min(10, Math.Max(0, value)); }
        }

        public Settings()
        {
            Server = "127.0.0.1";
            MusicVolume = 7;
            SoundVolume = 7;
        }

        /// <summary>
        /// Lädt die Settings aus dem Einstellungeverzeichnis.
        /// </summary>
        /// <returns></returns>
        internal static Settings LoadSettings()
        {
            // Standard Settings
            Settings settings = new Settings();

            // Einstellungen für Musik laden
            int music;
            if (int.TryParse(ApplicationData.Current.LocalSettings.Values["Music"] as string, out music))
                settings.MusicVolume = music;

            // Einstellungen für Sound laden
            int sound;
            if (int.TryParse(ApplicationData.Current.LocalSettings.Values["Sound"] as string, out sound))
                settings.SoundVolume = sound;

            // Server auslesen
            string server = ApplicationData.Current.LocalSettings.Values["Server"] as string;
            if (!string.IsNullOrEmpty(server))
                settings.Server = server;

            return settings;
        }

        /// <summary>
        /// Speichert die aktuellen Settings im Einstellungsvrzeichnis.
        /// </summary>
        /// <param name="settings"></param>
        internal static void SaveSettings(Settings settings)
        {
            ApplicationData.Current.LocalSettings.Values["Music"] = settings.MusicVolume.ToString();
            ApplicationData.Current.LocalSettings.Values["Sound"] = settings.SoundVolume.ToString();
            ApplicationData.Current.LocalSettings.Values["Server"] = settings.Server;
        }
    }
}
