#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using RheinwerkAdventure.Components;

#endregion

namespace RheinwerkAdventure
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    internal class RheinwerkGame : Game
    {
        GraphicsDeviceManager graphics;

        public Settings Settings { get; private set; }

        public HudComponent Hud { get; private set; }

        public InputComponent Input { get; private set; }

        public SimulationComponent Simulation { get; private set; }

        public SceneComponent Scene { get; private set; }

        public MusicComponent Music { get; private set; }

        public SoundComponent Sound { get; private set; }

        public ScreenComponent Screen { get; private set; }

        public LocalComponent Local { get; private set; }

        public IServerComponent Server { get; private set; }

        public IClientComponent Client { get; private set; }

        public RheinwerkGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            IsMouseVisible = true;	

            Input = new InputComponent(this);
            Input.UpdateOrder = 0;
            Components.Add(Input);

            Screen = new ScreenComponent(this);
            Screen.UpdateOrder = 1;
            Screen.DrawOrder = 2;
            Components.Add(Screen);

            Local = new LocalComponent(this);
            Local.UpdateOrder = 2;
            Components.Add(Local);

            var client = new ClientComponent(this);
            Client = client;
            client.UpdateOrder = 3;
            Components.Add(client);

            var server = new ServerComponent(this);
            Server = server;
            server.UpdateOrder = 4;
            Components.Add(server);

            Simulation = new SimulationComponent(this);
            Simulation.UpdateOrder = 5;
            Components.Add(Simulation);

            Scene = new SceneComponent(this);
            Scene.UpdateOrder = 6;
            Scene.DrawOrder = 0;
            Components.Add(Scene);

            Hud = new HudComponent(this);
            Hud.UpdateOrder = 7;
            Hud.DrawOrder = 1;
            Components.Add(Hud);

            Music = new MusicComponent(this);
            Music.UpdateOrder = 8;
            Components.Add(Music);

            Sound = new SoundComponent(this);
            Sound.UpdateOrder = 9;
            Components.Add(Sound);

            // Einstellungen laden
            Settings = Settings.LoadSettings();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            // Beim Beenden der Applikation ggf. offene Client/Server-Verbindungen schließen.
            Client.Close();
            Server.CloseServer();

            // Settings speichern
            Settings.SaveSettings(Settings);

            base.OnExiting(sender, args);
        }
    }
}

