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

        public InputComponent Input { get; private set; }

        public SimulationComponent Simulation { get; private set; }

        public SceneComponent Scene { get; private set; }

        public RheinwerkGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";	            
            graphics.IsFullScreen = false;		

            Input = new InputComponent(this);
            Input.UpdateOrder = 0;
            Components.Add(Input);

            Simulation = new SimulationComponent(this);
            Simulation.UpdateOrder = 1;
            Components.Add(Simulation);

            Scene = new SceneComponent(this);
            Scene.UpdateOrder = 2;
            Scene.DrawOrder = 0;
            Components.Add(Scene);
        }
    }
}

