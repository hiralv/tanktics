#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// The gameplay screen is currently a placeholder for
// where the actual game will be played.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Tanktics
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        string player = "Gameplay goes here!";
        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 playerSize;
        Vector2 playerMovement = new Vector2(3, 4);

        Viewport viewport;
        bool noViewport = true;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = ScreenManager.Font;// content.Load<SpriteFont>("gamefont");

            //calculate the size of the player string
            playerSize = gameFont.MeasureString(player);

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //bounce the player string around the screen
            
            //viewport has been found in Draw
            if (!noViewport)
            {
                //bounce off left border
                if (playerPosition.X < 0f)
                    playerMovement.X = -playerMovement.X;
                //bounce off right border
                else if ((playerPosition.X + playerSize.X) > viewport.Width)
                    playerMovement.X = -playerMovement.X;

                //bounce off top border
                if (playerPosition.Y < 0f)
                    playerMovement.Y = -playerMovement.Y;
                //bounce off bottom border
                else if ((playerPosition.Y + playerSize.Y) > viewport.Height)
                    playerMovement.Y = -playerMovement.Y;

                //move player
                playerPosition += playerMovement;
            }

        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.PauseGame)
            {
                LoadingScreen.Load(ScreenManager, false, new MenuBackgroundScreen(),
                    new MainMenuScreen());
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            //get the viewport (used by Update)
            viewport = ScreenManager.GraphicsDevice.Viewport;
            noViewport = false;

            spriteBatch.Begin();

            spriteBatch.DrawString(gameFont, player, playerPosition, Color.White);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }


        #endregion
    }
}
