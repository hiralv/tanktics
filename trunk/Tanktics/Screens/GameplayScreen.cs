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
        //amount of time (in seconds) between Update calls
        float elapsed;

        //tile engine and cameras
        TileEngine tileEngine;
        Camera2D camera;
        Camera2D miniMap;

        Hud hud;

        UnitController unitControl;
        Point selected = Point.Zero;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);

            tileEngine = new TileEngine("Maps/map0.txt", 48, 48, 10, 12);

            //create main camera
            camera = new Camera2D(
                tileEngine.TileWidth, tileEngine.TileHeight,
                tileEngine.MapWidth, tileEngine.MapHeight);
            camera.Viewport = new Rectangle(0, 0, 800, 440);
            camera.Speed = 240;
            //only allow zooming out to total width of board
            camera.MinScale = (float)camera.Viewport.Width / tileEngine.WidthInPixels;

            //create minimap window
            miniMap = new Camera2D(
                tileEngine.TileWidth, tileEngine.TileHeight,
                tileEngine.MapWidth, tileEngine.MapHeight);
            miniMap.Viewport = new Rectangle(650, 450, 150, 150);
            //scale to show entire map in camera
            miniMap.Scale = (float)miniMap.Viewport.Width / tileEngine.WidthInPixels;

            hud = new Hud(640,440,160,160);

            unitControl = new UnitController(tileEngine.MapWidth, tileEngine.MapHeight, 4);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            tileEngine.Texture = content.Load<Texture2D>("fulltileset_alpha");

            hud.LoadContent(content);

            Texture2D infantry1 = content.Load<Texture2D>("Infintry");
            Texture2D infantry2 = content.Load<Texture2D>("Infintry2");
            Texture2D infantry3 = content.Load<Texture2D>("Infintry3");
            Texture2D infantry4 = content.Load<Texture2D>("Infintry4");

            addUnits(1, 0, 0, infantry1);
            addUnits(2, 0, tileEngine.MapWidth - 4, infantry2);
            addUnits(3, tileEngine.MapHeight - 4, tileEngine.MapWidth - 4, infantry3);
            addUnits(4, tileEngine.MapHeight - 4, 0, infantry4);

            tileEngine.SelectedTexture = content.Load<Texture2D>("selected border");

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

        private void addUnits(int team, int top, int left, Texture2D texture)
        {
            for (int y = top; y < top + 4; y++)
            {
                for (int x = left; x < left + 4; x++)
                {
                    unitControl.addUnit("humvee", team, x, y, texture);
                }
            }
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
            
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            unitControl.update(gameTime);
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

            Vector2 cameraMotion = Vector2.Zero;

            //move camera
            if (input.IsKeyDownUp())
                cameraMotion.Y--;
            if (input.IsKeyDownDown())
                cameraMotion.Y++;
            if (input.IsKeyDownLeft())
                cameraMotion.X--;
            if (input.IsKeyDownRight())
                cameraMotion.X++;

            if (cameraMotion != Vector2.Zero)
            {
                //normalize so camera moves at same speed diagonally as horizontal/vertical
                cameraMotion.Normalize();
                camera.Move(cameraMotion, elapsed);
            }

            //zoom in/out with pageup/pagedown keys
            if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.PageUp))
                camera.ZoomIn(elapsed);
            if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.PageDown))
                camera.ZoomOut(elapsed);

            //select next unit
            if (input.IsNewKeyPress(Keys.Tab))
            {
                unitControl.nextUnit();
                selected.X = unitControl.currentUnit.currentX;
                selected.Y = unitControl.currentUnit.currentY;
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            //draw main camera and minimap
            tileEngine.Draw(spriteBatch, camera, unitControl, selected);
            tileEngine.Draw(spriteBatch, miniMap);

            hud.Draw(spriteBatch);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }


        #endregion
    }
}
