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
    //Robby Florence
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

        HUD hud;

        //unit controller and selected square
        UnitController unitControl;
        Point selected = Point.Zero;

        //Turn controllers
        TurnController TC1;
        TurnController TC2;
        TurnController TC3;
        TurnController TC4;
        TurnController[] TCs;

        //textures for apc animations for 4 players
        //(up, down, left, right, idle up, idle down, rotate)
        Texture2D[] apc1 = new Texture2D[7];
        Texture2D[] apc2 = new Texture2D[7];
        Texture2D[] apc3 = new Texture2D[7];
        Texture2D[] apc4 = new Texture2D[7];

        //textures for artillery animations for 4 players
        //(up, down, left, right, idle up, idle down, rotate)
        Texture2D[] artillery1 = new Texture2D[7];
        Texture2D[] artillery2 = new Texture2D[7];
        Texture2D[] artillery3 = new Texture2D[7];
        Texture2D[] artillery4 = new Texture2D[7];

        //textures for tank animations for 4 players
        //(up, down, left, right, idle up, idle down, rotate)
        Texture2D[] tank1 = new Texture2D[7];
        Texture2D[] tank2 = new Texture2D[7];
        Texture2D[] tank3 = new Texture2D[7];
        Texture2D[] tank4 = new Texture2D[7];

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);

            tileEngine = new TileEngine("Maps/map1.txt", 64, 64, 5, 12);

            //create main camera
            camera = new Camera2D(
                1,
                tileEngine.TileWidth, tileEngine.TileHeight,
                tileEngine.MapWidth, tileEngine.MapHeight);
            camera.Viewport = new Rectangle(0, 0, 800, 430);
            camera.Speed = 240;
            //only allow zooming out to total width of board
            camera.MinScale = (float)camera.Viewport.Width / tileEngine.WidthInPixels;

            //create minimap window
            miniMap = new Camera2D(
                1,
                tileEngine.TileWidth, tileEngine.TileHeight,
                tileEngine.MapWidth, tileEngine.MapHeight);
            miniMap.Viewport = new Rectangle(650, 450, 150, 150);
            //scale to show entire map in camera
            miniMap.Scale = (float)miniMap.Viewport.Width / tileEngine.WidthInPixels;

            hud = new HUD(0, 430, 800, 170);

            unitControl = new UnitController(tileEngine, 4);
            TC1 = new TurnController(unitControl, 0, 0, 3, 3);
            TC2 = new TurnController(unitControl, 22, 0, 25, 3);
            TC3 = new TurnController(unitControl, 0, 22, 3, 25);
            TC4 = new TurnController(unitControl, 22, 22, 25, 25);
            TC1.setNext(TC2);
            TC2.setNext(TC3);
            TC3.setNext(TC4);
            TC4.setNext(TC1);
            TCs = new TurnController[4];
            TCs[0] = TC1;
            TCs[1] = TC2;
            TCs[2] = TC3;
            TCs[3] = TC4;
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            tileEngine.Texture = content.Load<Texture2D>("fullTileSet");
            tileEngine.SelectedTexture = content.Load<Texture2D>("selected border");
            tileEngine.BlankTexture = content.Load<Texture2D>("blank");

            hud.LoadContent(content);

            //load player 1 (white) apc animations
            apc1[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Up/APC Driving Up White");
            apc1[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Down/APC Driving Down White");
            apc1[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Left/APC Driving Left White");
            apc1[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Right/APC Driving Right White");
            apc1[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Up/APC Idling Up White");
            apc1[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Down/APC Idling Down White");
            apc1[(int)Unit.Anim.Rotate] = content.Load<Texture2D>("Unit Animations/APC/APC Rotating/APC Rotating White");

            //load player 2 (green) apc animations
            apc2[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Up/APC Driving Up Green");
            apc2[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Down/APC Driving Down Green");
            apc2[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Left/APC Driving Left Green");
            apc2[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Right/APC Driving Right Green");
            apc2[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Up/APC Idling Up Green");
            apc2[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Down/APC Idling Down Green");
            apc2[(int)Unit.Anim.Rotate] = content.Load<Texture2D>("Unit Animations/APC/APC Rotating/APC Rotating Green");

            //load player 3 (grey) apc animations
            apc3[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Up/APC Driving Up Grey");
            apc3[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Down/APC Driving Down Grey");
            apc3[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Left/APC Driving Left Grey");
            apc3[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Right/APC Driving Right Grey");
            apc3[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Up/APC Idling Up Grey");
            apc3[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Down/APC Idling Down Grey");
            apc3[(int)Unit.Anim.Rotate] = content.Load<Texture2D>("Unit Animations/APC/APC Rotating/APC Rotating Grey");

            //load player 4 (brown) apc animations
            apc4[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Up/APC Driving Up Brown");
            apc4[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Down/APC Driving Down Brown");
            apc4[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Left/APC Driving Left Brown");
            apc4[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Right/APC Driving Right Brown");
            apc4[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Up/APC Idling Up Brown");
            apc4[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Down/APC Idling Down Brown");
            apc4[(int)Unit.Anim.Rotate] = content.Load<Texture2D>("Unit Animations/APC/APC Rotating/APC Rotating Brown");



            //load player 1 (white) artillery animations
            artillery1[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Up/Artillery Driving Up White");
            artillery1[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Down/Artillery Driving Down White");
            artillery1[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Left/Artillery Driving Left White");
            artillery1[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Right/Artillery Driving Right White");
            artillery1[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Up/Artillery Idling Up White");
            artillery1[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Down/Artillery Idling Down White");
            artillery1[(int)Unit.Anim.Rotate] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Rotating/Artillery Rotating White");

            //load player 2 (green) artillery animations
            artillery2[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Up/Artillery Driving Up Green");
            artillery2[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Down/Artillery Driving Down Green");
            artillery2[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Left/Artillery Driving Left Green");
            artillery2[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Right/Artillery Driving Right Green");
            artillery2[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Up/Artillery Idling Up Green");
            artillery2[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Down/Artillery Idling Down Green");
            artillery2[(int)Unit.Anim.Rotate] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Rotating/Artillery Rotating Green");

            //load player 3 (grey) artillery animations
            artillery3[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Up/Artillery Driving Up Grey");
            artillery3[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Down/Artillery Driving Down Grey");
            artillery3[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Left/Artillery Driving Left Grey");
            artillery3[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Right/Artillery Driving Right Grey");
            artillery3[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Up/Artillery Idling Up Grey");
            artillery3[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Down/Artillery Idling Down Grey");
            artillery3[(int)Unit.Anim.Rotate] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Rotating/Artillery Rotating Grey");

            //load player 4 (brown) artillery animations
            artillery4[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Up/Artillery Driving Up Brown");
            artillery4[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Down/Artillery Driving Down Brown");
            artillery4[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Left/Artillery Driving Left Brown");
            artillery4[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Right/Artillery Driving Right Brown");
            artillery4[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Up/Artillery Idling Up Brown");
            artillery4[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Down/Artillery Idling Down Brown");
            artillery4[(int)Unit.Anim.Rotate] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Rotating/Artillery Rotating Brown");



            //load player 1 (white) tank animations
            tank1[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Up/Tank Driving Up White");
            tank1[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Down/Tank Driving Down White");
            tank1[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Left/Tank Driving Left White");
            tank1[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Right/Tank Driving Right White");
            tank1[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Up/Tank Idling Up White");
            tank1[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Down/Tank Idling Down White");
            tank1[(int)Unit.Anim.Rotate] = content.Load<Texture2D>("Unit Animations/Tank/Tank Rotating/Tank Rotating White");

            //load player 2 (green) tank animations
            tank2[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Up/Tank Driving Up Green");
            tank2[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Down/Tank Driving Down Green");
            tank2[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Left/Tank Driving Left Green");
            tank2[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Right/Tank Driving Right Green");
            tank2[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Up/Tank Idling Up Green");
            tank2[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Down/Tank Idling Down Green");
            tank2[(int)Unit.Anim.Rotate] = content.Load<Texture2D>("Unit Animations/Tank/Tank Rotating/Tank Rotating Green");

            //load player 3 (grey) tank animations
            tank3[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Up/Tank Driving Up Grey");
            tank3[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Down/Tank Driving Down Grey");
            tank3[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Left/Tank Driving Left Grey");
            tank3[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Right/Tank Driving Right Grey");
            tank3[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Up/Tank Idling Up Grey");
            tank3[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Down/Tank Idling Down Grey");
            tank3[(int)Unit.Anim.Rotate] = content.Load<Texture2D>("Unit Animations/Tank/Tank Rotating/Tank Rotating Grey");

            //load player 4 (brown) tank animations
            tank4[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Up/Tank Driving Up Brown");
            tank4[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Down/Tank Driving Down Brown");
            tank4[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Left/Tank Driving Left Brown");
            tank4[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Right/Tank Driving Right Brown");
            tank4[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Up/Tank Idling Up Brown");
            tank4[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Down/Tank Idling Down Brown");
            tank4[(int)Unit.Anim.Rotate] = content.Load<Texture2D>("Unit Animations/Tank/Tank Rotating/Tank Rotating Brown");

            
            //addUnits(2, 0, tileEngine.MapWidth - 4, artillery2);
            //addUnits(3, tileEngine.MapHeight - 4, tileEngine.MapWidth - 4, artillery3);
            //addUnits(4, tileEngine.MapHeight - 4, 0, artillery4);

            //unitControl.addUnit("artillery", 1, 2, 0, artillery1);
            //unitControl.addUnit("artillery", 1, 3, 0, artillery1);
            //unitControl.addUnit("tank", 1, 1, 1, tank1);
            //unitControl.addUnit("tank", 1, 2, 1, tank1);
            //unitControl.addUnit("tank", 1, 3, 1, tank1);
            //unitControl.addUnit("tank", 1, 1, 2, tank1);
            //unitControl.addUnit("apc", 1, 2, 2, apc1);
            //unitControl.addUnit("apc", 1, 3, 2, apc1);
            //unitControl.addUnit("apc", 1, 0, 3, apc1);
            //unitControl.addUnit("apc", 1, 1, 3, apc1);
            //unitControl.addUnit("apc", 1, 2, 3, apc1);
            //unitControl.addUnit("apc", 1, 3, 3, apc1);
            //unitControl.nextUnit();
            //selected.X = unitControl.currentUnit.currentX;
            //selected.Y = unitControl.currentUnit.currentY;

            //selected.X = unitControl.currentUnit.currentX;
            //selected.Y = unitControl.currentUnit.currentY;

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

        //delete this when we finish the initialization phase
        private void addUnits(int team, int top, int left, Texture2D[] textures)
        {
            for (int y = top; y < top + 4; y++)
            {
                for (int x = left; x < left + 4; x++)
                {
                    unitControl.addUnit("artillery", team, x, y, textures);
                }
            }
            //Sets the team to be passed the set up phase sence that is basically what this is.
            TCs[team - 1].nextPhase();
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
            hud.Update(gameTime, unitControl);
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            //exit game
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

            //Combat Phase: select next unit
            if (input.IsNewKeyPress(Keys.Tab))
            {
                if (TCs[unitControl.currentPlayer - 1].phase == 2)
                {
                    unitControl.nextUnit();
                    selected.X = unitControl.currentUnit.currentX;
                    selected.Y = unitControl.currentUnit.currentY;
                }
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

            //Combat Phase:move current unit to selected square
            //Unit Placement Phase: Place next unit on selected square
            if (input.IsNewKeyPress(Keys.Space))
            {
                if (TCs[unitControl.currentPlayer - 1].phase == 2)
                {
                    unitControl.moveUnit(selected.X, selected.Y);
                    selected.X = unitControl.currentUnit.currentX;
                    selected.Y = unitControl.currentUnit.currentY;
                }

                else if (TCs[unitControl.currentPlayer - 1].phase == 0)
                {
                    if (TCs[unitControl.currentPlayer - 1].totalAPC < TCs[unitControl.currentPlayer - 1].MAXAPC)
                    {
                        if (TCs[unitControl.currentPlayer - 1].createUnit("apc", selected.X, selected.Y))
                        {
                            if (unitControl.currentPlayer == 1)
                                unitControl.addUnit("apc", unitControl.currentPlayer, selected.X, selected.Y, apc1);
                            else if (unitControl.currentPlayer == 2)
                                unitControl.addUnit("apc", unitControl.currentPlayer, selected.X, selected.Y, apc2);
                            else if (unitControl.currentPlayer == 3)
                                unitControl.addUnit("apc", unitControl.currentPlayer, selected.X, selected.Y, apc3);
                            else if (unitControl.currentPlayer == 4)
                                unitControl.addUnit("apc", unitControl.currentPlayer, selected.X, selected.Y, apc4);

                            TCs[unitControl.currentPlayer - 1].totalAPC++;
                        }
                    }
                    else if (TCs[unitControl.currentPlayer - 1].totalTank < TCs[unitControl.currentPlayer - 1].MAXTANK)
                    {
                        if (TCs[unitControl.currentPlayer - 1].createUnit("tank", selected.X, selected.Y))
                        {
                            if (unitControl.currentPlayer == 1)
                                unitControl.addUnit("tank", unitControl.currentPlayer, selected.X, selected.Y, tank1);
                            else if (unitControl.currentPlayer == 2)
                                unitControl.addUnit("tank", unitControl.currentPlayer, selected.X, selected.Y, tank2);
                            else if (unitControl.currentPlayer == 3)
                                unitControl.addUnit("tank", unitControl.currentPlayer, selected.X, selected.Y, tank3);
                            else if (unitControl.currentPlayer == 4)
                                unitControl.addUnit("tank", unitControl.currentPlayer, selected.X, selected.Y, tank4);

                            TCs[unitControl.currentPlayer - 1].totalTank++;
                        }
                    }
                    else if (TCs[unitControl.currentPlayer - 1].totalArtil < TCs[unitControl.currentPlayer - 1].MAXARTIL)
                    {
                        if (TCs[unitControl.currentPlayer - 1].createUnit("artillery", selected.X, selected.Y))
                        {
                            if (unitControl.currentPlayer == 1)
                                unitControl.addUnit("artillery", unitControl.currentPlayer, selected.X, selected.Y, artillery1);
                            else if (unitControl.currentPlayer == 2)
                                unitControl.addUnit("artillery", unitControl.currentPlayer, selected.X, selected.Y, artillery2);
                            else if (unitControl.currentPlayer == 3)
                                unitControl.addUnit("artillery", unitControl.currentPlayer, selected.X, selected.Y, artillery3);
                            else if (unitControl.currentPlayer == 4)
                                unitControl.addUnit("artillery", unitControl.currentPlayer, selected.X, selected.Y, artillery4);

                            TCs[unitControl.currentPlayer - 1].totalArtil++;
                        }
                        //If all units placed move on to next Phase
                        //Note may be changed to move to next players phase 0
                        if (TCs[unitControl.currentPlayer - 1].totalArtil == TCs[unitControl.currentPlayer - 1].MAXARTIL)
                        {
                            TCs[unitControl.currentPlayer - 1].nextPhase();
                            unitControl.nextUnit();
                            selected.X = unitControl.currentUnit.currentX;
                            selected.Y = unitControl.currentUnit.currentY;
                        }
                    }
                }

            }

            //Unit Placement Phase: No Function
            //Movement Phase: Next Phase
            //Combat Phase: Next Phase
            //Purchase Phase: Finalize and End Turn
            if (input.IsNewKeyPress(Keys.Enter))
            {

                if (TCs[unitControl.currentPlayer - 1].phase == 2)
                {
                    TCs[unitControl.currentPlayer - 1].nextPhase();
                }
                else if (TCs[unitControl.currentPlayer - 1].phase == 3)
                {
                    TCs[unitControl.currentPlayer - 1].nextPhase();
                }
                else if (TCs[unitControl.currentPlayer - 1].phase == 4)
                {
                    TCs[unitControl.currentPlayer-1].nextPhase();
                    TCs[unitControl.currentPlayer-1].getNext().nextPhase();
                    unitControl.finalize();
                    unitControl.nextUnit();
                    if (unitControl.currentUnit != null)
                    {
                        selected.X = unitControl.currentUnit.currentX;
                        selected.Y = unitControl.currentUnit.currentY;
                    }
                    //change player number for cameras
                    camera.PlayerNum = unitControl.currentPlayer;
                    miniMap.PlayerNum = unitControl.currentPlayer;
                }

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
            tileEngine.Draw(spriteBatch, miniMap, unitControl);

            hud.Draw(spriteBatch);
            
            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }


        #endregion
    }
}
