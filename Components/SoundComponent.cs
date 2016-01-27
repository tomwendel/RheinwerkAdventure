using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using RheinwerkAdventure.Model;

namespace RheinwerkAdventure.Components
{
    internal class SoundComponent : GameComponent
    {
        private RheinwerkGame game;

        private Dictionary<string, SoundEffect> sounds;

        private float volume;

        // Player Referenz
        private Player player;

        // Referenz auf die sichtbare Area.
        private Area area;

        // Anzahl Münzen
        private int coins;

        // Mapping von Angreifern zu deren Recovery Times.
        private Dictionary<IAttacker, TimeSpan> recoveryTimes;

        // Mapping von Angreifbaren Items zu Hitpoints.
        private Dictionary<IAttackable, int> hitpoints;

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

            recoveryTimes = new Dictionary<IAttacker, TimeSpan>();
            hitpoints = new Dictionary<IAttackable, int>();
        }

        public override void Update(GameTime gameTime)
        {
            // Nur wenn Komponente aktiviert wurde.
            if (!Enabled)
                return;

            // Nur arbeiten, wenn es eine Welt, einen Player und eine aktive Area gibt.
            Area nextArea = game.Local.GetCurrentArea();
            if (game.Simulation.World == null || game.Local.Player == null || nextArea == null)
                return;
            
            // Reset aller Variablen falls sich der Player ändert
            if (player != game.Local.Player)
            {
                player = game.Local.Player;
                coins = player.Inventory.Count(i => i is Coin);
                recoveryTimes.Clear();
            }

            // Reset der Item variablen falls sich die Area ändert
            if (area != nextArea)
            {
                area = nextArea;

                // Recovery Times
                recoveryTimes.Clear();
                foreach (var item in area.Items.OfType<IAttacker>())
                    recoveryTimes.Add(item, item.Recovery);

                // Hitpoints
                hitpoints.Clear();
                foreach (var item in area.Items.OfType<IAttackable>())
                    hitpoints.Add(item, item.Hitpoints);
            }

            // Coins
            int c = player.Inventory.Count(i => i is Coin);
            if (coins < c)
                Play("coin");
            coins = c;

            // Sword
            foreach (var item in area.Items.OfType<IAttacker>())
            {
                TimeSpan recovery;
                if (!recoveryTimes.TryGetValue(item, out recovery))
                {
                    recovery = item.Recovery;
                    recoveryTimes.Add(item, item.Recovery);
                }

                if (recovery < item.Recovery)
                    Play("sword");
                recoveryTimes[item] = item.Recovery;
            }

            // Hit
            foreach (var item in area.Items.OfType<IAttackable>())
            {
                int points;
                if (!hitpoints.TryGetValue(item, out points))
                {
                    points = item.Hitpoints;
                    hitpoints.Add(item, item.Hitpoints);
                }

                if (points > item.Hitpoints)
                    Play("hit");
                hitpoints[item] = item.Hitpoints;
            }
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
    }
}

