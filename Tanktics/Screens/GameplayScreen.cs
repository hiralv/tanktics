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

        //Hud elements
        Hud hud;
        DataHud dataHud;
        ButtonHud buttonHud;
        ModelHud modelHud;
        GraphHud graphHud;

        //unit controler and selected square
        UnitController unitControl;
        Point selected = Point.Zero;

        //textures for unit animations (up, down, left, right, idle up, idle down)
        Texture2D[] tank1 = new Texture2D[6];
        Texture2D[] tank2 = new Texture2D[6];
        Texture2D[] tank3 = new Texture2D[6];
        Texture2D[] tank4 = new Texture2D[6];

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
            graphHud = new GraphHud(480, 480, 160, 160);
            buttonHud = new ButtonHud(320, 480, 160, 160);
            dataHud = new DataHud(160, 480, 160, 160);
            modelHud = new ModelHud(0, 440, 160, 160);

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
            tileEngine.SelectedTexture = content.Load<Texture2D>("selected border");

            hud.LoadContent(content);
            buttonHud.LoadContent(content);
            graphHud.LoadContent(content);
            dataHud.LoadContent(content);
            modelHud.LoadContent(content);

            Texture2D infantry1 = content.Load<Texture2D>("Infintry");
            Texture2D infantry2 = content.Load<Texture2D>("Infintry2");
            Texture2D infantry3 = content.Load<Texture2D>("Infintry3");
            Texture2D infantry4 = content.Load<Texture2D>("Infintry4");

            //load player 1 (white) tank animations
            //tank1[0] = content.Load<Texture2D>("Tank Animations/Tank Driving Up Animation");
            //tank1[1] = content.Load<Texture2D>("Tank Animations/Tank Driving Down Animation");
            //tank1[2] = content.Load<Texture2D>("Tank Animations/Tank Driving Left Animation");
            //tank1[3] = content.Load<Texture2D>("Tank Animations/Tank Driving Right Animation");
            tank1[4] = content.Load<Texture2D>("Tank Animations/Tank Idleing Up Animation");
            tank1[5] = content.Load<Texture2D>("Tank Animations/Tank Idleing Down Animation");

            //load player 2 (grey) tank animations
            //tank2[0] = content.Load<Texture2D>("Tank Animations/Tank Driving Up Animation Grey");
            //tank2[1] = content.Load<Texture2D>("Tank Animations/Tank Driving Down Animation Grey");
            //tank2[2] = content.Load<Texture2D>("Tank Animations/Tank Driving Left Animation Grey");
            //tank2[3] = content.Load<Texture2D>("Tank Animations/Tank Driving Right Animation Grey");
            tank2[4] = content.Load<Texture2D>("Tank Animations/Tank Idleing Up Animation");
            tank2[5] = content.Load<Texture2D>("Tank Animations/Tank Idleing Down Animation");

            //load player 3 (green) tank animations
            //tank3[0] = content.Load<Texture2D>("Tank Animations/Tank Driving Up Animation Green");
            //tank3[1] = content.Load<Texture2D>("Tank Animations/Tank Driving Down Animation Green");
            //tank3[2] = content.Load<Texture2D>("Tank Animations/Tank Driving Left Animation Green");
            //tank3[3] = content.Load<Texture2D>("Tank Animations/Tank Driving Right Animation Green");
            tank3[4] = content.Load<Texture2D>("Tank Animations/Tank Idleing Up Animation");
            tank3[5] = content.Load<Texture2D>("Tank Animations/Tank Idleing Down Animation");

            //load player 4 (brown) tank animations
            //tank4[0] = content.Load<Texture2D>("Tank Animations/Tank Driving Up Animation Brown");
            //tank4[1] = content.Load<Texture2D>("Tank Animations/Tank Driving Down Animation Brown");
            //tank4[2] = content.Load<Texture2D>("Tank Animations/Tank Driving Left Animation Brown");
            //tank4[3] = content.Load<Texture2D>("Tank Animations/Tank Driving Right Animation Brown");
            tank4[4] = content.Load<Texture2D>("Tank Animations/Tank Idleing Up Animation");
            tank4[5] = content.Load<Texture2D>("Tank Animations/Tank Idleing Down Animation");

            addUnits(1, 0, 0, tank1);
            addUnits(2, 0, tileEngine.MapWidth - 4, tank2);
            addUnits(3, tileEngine.MapHeight - 4, tileEngine.MapWidth - 4, tank3);
            addUnits(4, tileEngine.MapHeight - 4, 0, tank4);

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

        private void addUnits(int team, int top, int left, Texture2D[] textures)
        {
            for (int y = top; y < top + 4; y++)
            {
                for (int x = left; x < left + 4; x++)
                {
                    unitControl.addUnit("tank", team, x, y, textures);
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

            //move selected square
            if (input.IsNewKeyPress(Keys.W) && selected.Y > 0)
                selected.Y--;
            if (input.IsNewKeyPress(Keys.S) && selected.Y < tileEngine.MapHeight - 1)
                selected.Y++;
            if (input.IsNewKeyPress(Keys.A) && selected.X > 0)
                selected.X--;
            if (input.IsNewKeyPress(Keys.D) && selected.X < tileEngine.MapWidth - 1)
                selected.X++;

            //move current unit to selected square
            if (input.IsNewKeyPress(Keys.Space))
            {
                unitControl.moveUnit(selected.X, selected.Y);
                selected.X = unitControl.currentUnit.currentX;
                selected.Y = unitControl.currentUnit.currentY;
            }

            //finalize turn
            if (input.IsNewKeyPress(Keys.Enter))
            {
                unitControl.finalize();
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
            buttonHud.Draw(spriteBatch);
            graphHud.Draw(spriteBatch);
            dataHud.Draw(spriteBatch);
            modelHud.Draw(spriteBatch);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }


        #endregion
    }
}
