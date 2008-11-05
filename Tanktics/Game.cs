#region File Description
//--------------------------------------------------------------------------------
//Game.cs - Tanktics
//
//Acey Boyce
//Robby Florence
//Hiral Vora
//Chris Wykel
//
//MODEL-VIEW-CONTROLLER DESIGN PATTERN
//
//The Model for our code is all the menus.  Each one operates in a similar 
//manner all conforming to sets of rules for what a player may do in these menus.  
//Although there are very little calculations done in this menu system, this is 
//where they are done and stored.  It tells the View how to and what to render at 
//any given time.
//
//Our View are the GUI menus.  They render a given screen in a preset way until 
//the user gives some input.
//
//The Controller is the user input from an Xbox controller or keyboard.  Although 
//the options have not been implimented when they are these inputs will have 
//direct effect on the options stored in the Model.  As of now the main purpose 
//of the Control input is to change between the different screens that are to be 
//rendered in View.
//
//As you can see this menu system follows the all important Model View Controller 
//design pattern.
//
//GAME LOOP
//
//Tanktics currently contains only a Front-End game loop. The ScreenManager is responsible
//for looping through the steps for each active GameScreen.
//
//Gather Input - The menu screens contain a HandleInput method which, using the InputState
//class, gather's user input from the keyboard or Xbox 360 controller.
//
//Render Screen - All screens contain a Draw method which renders the current state of
//the screen to the window.
//
//Update Front-End State - All screens contain an Update method which updates the state 
//of the screen. For the intro screens, these methods track the elapsed time since the 
//screen was created. Some also update animations or start new animations. For the menu
//screens, these methods use the data from the Gather Input state to rotate the menu
//sprites.
//
//Trigger Any State Changes - In each screen's Update method, state changes can be triggered.
//For the intro screens, the state change is triggered by a set time limit or when an animation
//has finished. For the menu screens, the state change is triggered by the data from the
//Gather Input state. When a MenuSelect button has been pressed, the state changes to whatever
//menu item is currently selected. When a MenuCancel button has been pressed, the state changes
//to the previous menu. All screens remove themselves and inform the ScreenManager to load
//the new screen(s) whenever a state change is triggered.
//
//--------------------------------------------------------------------------------

#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Tanktics
{
    

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        #endregion

        #region Initialization

        public Game()
        {

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);



            // Activate the first screen.
            screenManager.AddScreen(new WarningScreen());



        }

        protected override void Initialize()
        {
           base.Initialize();
        }

        protected override void LoadContent()
        {

        }
        protected override void UnloadContent()
        {

        }

        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }

        #endregion
    }

    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (Game game = new Game())
            {
                game.Run();
            }
        }
    }

    #endregion
}











