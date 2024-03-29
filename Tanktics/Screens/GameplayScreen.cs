#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// The GameplayScreen is where the game is played. -- Robby Florence
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
#endregion

namespace Tanktics
{
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
        Point prevSelected = Point.Zero;

        //Turn controllers
        TurnController TC1;
        TurnController TC2;
        TurnController TC3;
        TurnController TC4;
        TurnController[] TCs;

        Boolean gameOver = false;

        //textures for apc animations for 4 players
        //(up, down, left, right, idle up, idle down)
        Texture2D[] apc1 = new Texture2D[6];
        Texture2D[] apc2 = new Texture2D[6];
        Texture2D[] apc3 = new Texture2D[6];
        Texture2D[] apc4 = new Texture2D[6];

        //textures for artillery animations for 4 players
        //(up, down, left, right, idle up, idle down)
        Texture2D[] artillery1 = new Texture2D[6];
        Texture2D[] artillery2 = new Texture2D[6];
        Texture2D[] artillery3 = new Texture2D[6];
        Texture2D[] artillery4 = new Texture2D[6];

        //textures for tank animations for 4 players
        //(up, down, left, right, idle up, idle down)
        Texture2D[] tank1 = new Texture2D[6];
        Texture2D[] tank2 = new Texture2D[6];
        Texture2D[] tank3 = new Texture2D[6];
        Texture2D[] tank4 = new Texture2D[6];

        AI ai1;
        //Unit previousunit;
        //stores whether players are human/AI
        Boolean[] humanPlayers;
        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(Boolean human1, Boolean human2, Boolean human3, Boolean human4)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);

            humanPlayers = new Boolean[4];
            humanPlayers[0] = human1;
            humanPlayers[1] = human2;
            humanPlayers[2] = human3;
            humanPlayers[3] = human4;

            tileEngine = new TileEngine("Maps/map0.txt", 64, 64, 5, 12);

            //create main camera
            camera = new Camera2D(
                1,
                tileEngine.TileWidth, tileEngine.TileHeight,
                tileEngine.MapWidth, tileEngine.MapHeight);
            camera.Speed = 240;

            //create minimap window
            miniMap = new Camera2D(
                1,
                tileEngine.TileWidth, tileEngine.TileHeight,
                tileEngine.MapWidth, tileEngine.MapHeight);

            unitControl = new UnitController(tileEngine, 4);
            TC1 = new TurnController(unitControl, 1, 0, 0, 2, 2);
            TC2 = new TurnController(unitControl, 2, tileEngine.MapWidth - 3, 0, tileEngine.MapWidth - 1, 2);
            TC3 = new TurnController(unitControl, 3, tileEngine.MapWidth - 3, tileEngine.MapHeight - 3,
                tileEngine.MapWidth - 1, tileEngine.MapHeight - 1);
            TC4 = new TurnController(unitControl, 4, 0, tileEngine.MapHeight - 3, 2, tileEngine.MapHeight - 1);
            TC1.setNext(TC2);
            TC2.setNext(TC3);
            TC3.setNext(TC4);
            TC4.setNext(TC1);
            TCs = new TurnController[4];
            TCs[0] = TC1;
            TCs[1] = TC2;
            TCs[2] = TC3;
            TCs[3] = TC4;

            ai1 = new AI(unitControl);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            //set camera and hud viewports as percentages of window
            //viewport isnt available in constructor, so do it here
            camera.Viewport = new Rectangle(
                viewport.X, viewport.Y,
                viewport.Width, (int)(430f / 600f * viewport.Height));
            //only allow zooming out to total width of board
            camera.MinScale = (float)camera.Viewport.Width / tileEngine.WidthInPixels;
            miniMap.Viewport = new Rectangle(
                viewport.X + (int)(650f / 800f * viewport.Width),
                viewport.Y + (int)(450f / 600f * viewport.Height),
                (int)(150f / 800f * viewport.Width),
                (int)(150f / 600f * viewport.Height));
            //scale to show entire map in camera
            miniMap.Scale = (float)miniMap.Viewport.Width / tileEngine.WidthInPixels;

            hud = new HUD(
                viewport.X,
                viewport.Y + (int)(430f / 800f * viewport.Width),
                viewport.Width,
                (int)(170f / 600f * viewport.Height));
            hud.LoadContent(content);

            tileEngine.LoadContent(content);

            unitControl.explosion.Texture = content.Load<Texture2D>("Unit Explosion");


            //load player 1 (white) apc animations
            apc1[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Up/APC Driving Up White");
            apc1[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Down/APC Driving Down White");
            apc1[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Left/APC Driving Left White");
            apc1[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Right/APC Driving Right White");
            apc1[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Up/APC Idling Up White");
            apc1[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Down/APC Idling Down White");

            //load player 2 (green) apc animations
            apc2[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Up/APC Driving Up Green");
            apc2[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Down/APC Driving Down Green");
            apc2[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Left/APC Driving Left Green");
            apc2[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Right/APC Driving Right Green");
            apc2[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Up/APC Idling Up Green");
            apc2[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Down/APC Idling Down Green");

            //load player 3 (grey) apc animations
            apc3[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Up/APC Driving Up Grey");
            apc3[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Down/APC Driving Down Grey");
            apc3[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Left/APC Driving Left Grey");
            apc3[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Right/APC Driving Right Grey");
            apc3[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Up/APC Idling Up Grey");
            apc3[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Down/APC Idling Down Grey");

            //load player 4 (brown) apc animations
            apc4[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Up/APC Driving Up Brown");
            apc4[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Down/APC Driving Down Brown");
            apc4[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Left/APC Driving Left Brown");
            apc4[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/APC/APC Driving Right/APC Driving Right Brown");
            apc4[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Up/APC Idling Up Brown");
            apc4[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/APC/APC Idling Down/APC Idling Down Brown");



            //load player 1 (white) artillery animations
            artillery1[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Up/Artillery Driving Up White");
            artillery1[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Down/Artillery Driving Down White");
            artillery1[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Left/Artillery Driving Left White");
            artillery1[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Right/Artillery Driving Right White");
            artillery1[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Up/Artillery Idling Up White");
            artillery1[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Down/Artillery Idling Down White");

            //load player 2 (green) artillery animations
            artillery2[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Up/Artillery Driving Up Green");
            artillery2[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Down/Artillery Driving Down Green");
            artillery2[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Left/Artillery Driving Left Green");
            artillery2[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Right/Artillery Driving Right Green");
            artillery2[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Up/Artillery Idling Up Green");
            artillery2[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Down/Artillery Idling Down Green");

            //load player 3 (grey) artillery animations
            artillery3[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Up/Artillery Driving Up Grey");
            artillery3[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Down/Artillery Driving Down Grey");
            artillery3[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Left/Artillery Driving Left Grey");
            artillery3[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Right/Artillery Driving Right Grey");
            artillery3[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Up/Artillery Idling Up Grey");
            artillery3[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Down/Artillery Idling Down Grey");

            //load player 4 (brown) artillery animations
            artillery4[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Up/Artillery Driving Up Brown");
            artillery4[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Down/Artillery Driving Down Brown");
            artillery4[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Left/Artillery Driving Left Brown");
            artillery4[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Driving Right/Artillery Driving Right Brown");
            artillery4[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Up/Artillery Idling Up Brown");
            artillery4[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Idling Down/Artillery Idling Down Brown");



            //load player 1 (white) tank animations
            tank1[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Up/Tank Driving Up White");
            tank1[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Down/Tank Driving Down White");
            tank1[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Left/Tank Driving Left White");
            tank1[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Right/Tank Driving Right White");
            tank1[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Up/Tank Idling Up White");
            tank1[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Down/Tank Idling Down White");

            //load player 2 (green) tank animations
            tank2[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Up/Tank Driving Up Green");
            tank2[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Down/Tank Driving Down Green");
            tank2[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Left/Tank Driving Left Green");
            tank2[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Right/Tank Driving Right Green");
            tank2[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Up/Tank Idling Up Green");
            tank2[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Down/Tank Idling Down Green");

            //load player 3 (grey) tank animations
            tank3[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Up/Tank Driving Up Grey");
            tank3[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Down/Tank Driving Down Grey");
            tank3[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Left/Tank Driving Left Grey");
            tank3[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Right/Tank Driving Right Grey");
            tank3[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Up/Tank Idling Up Grey");
            tank3[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Down/Tank Idling Down Grey");

            //load player 4 (brown) tank animations
            tank4[(int)Unit.Anim.Up] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Up/Tank Driving Up Brown");
            tank4[(int)Unit.Anim.Down] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Down/Tank Driving Down Brown");
            tank4[(int)Unit.Anim.Left] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Left/Tank Driving Left Brown");
            tank4[(int)Unit.Anim.Right] = content.Load<Texture2D>("Unit Animations/Tank/Tank Driving Right/Tank Driving Right Brown");
            tank4[(int)Unit.Anim.IdleUp] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Up/Tank Idling Up Brown");
            tank4[(int)Unit.Anim.IdleDown] = content.Load<Texture2D>("Unit Animations/Tank/Tank Idling Down/Tank Idling Down Brown");

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
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            unitControl.update(gameTime);
            hud.Update(gameTime, TCs[unitControl.currentPlayer - 1], unitControl);

            //select next unit after current unit has finished moving
            if (TCs[unitControl.currentPlayer - 1].phase == 2 &&
                unitControl.currentUnit != null &&
                unitControl.currentUnit.hasMoved && !unitControl.currentUnit.isMoving)
            {
                unitControl.nextUnit();
                selected.X = unitControl.currentUnit.currentX;
                selected.Y = unitControl.currentUnit.currentY;
            }

            //check game over
            if (TCs[unitControl.currentPlayer - 1].playerWon)
                gameOver = true;
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            #region Gamepad Setup
            String leftString = "";
            String rightString = "";
            GamePadState gps = GamePad.GetState(PlayerIndex.One);

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                Vector2 leftStick = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
                Vector2 rightStick = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;
                if (leftStick.Length() > .2f)
                {
                    float stickAngle = (float)Math.Atan2(leftStick.Y, leftStick.X);
                    if (stickAngle > -MathHelper.PiOver4 &&
                        stickAngle < MathHelper.PiOver4)
                    {
                        leftString = "right";
                    }
                    else if (stickAngle > MathHelper.PiOver4 &&
                    stickAngle < 3f * MathHelper.PiOver4)
                    {
                        leftString = "up";
                    }

                    else if (stickAngle > -(3f * MathHelper.PiOver4) &&
                    stickAngle < -MathHelper.PiOver4)
                    {
                        leftString = "down";  
                    }
                    else
                    {
                        leftString = "left";
                    }
                }
                if (rightStick.Length() > .2f)
                {
                    float stickAngle = (float)Math.Atan2(rightStick.Y, rightStick.X);
                    if (stickAngle > -MathHelper.PiOver4 &&
                        stickAngle < MathHelper.PiOver4)
                    {
                        rightString = "right";
                    }
                    else if (stickAngle > MathHelper.PiOver4 &&
                    stickAngle < 3f * MathHelper.PiOver4)
                    {
                        rightString = "up";
                    }

                    else if (stickAngle > -(3f * MathHelper.PiOver4) &&
                    stickAngle < -MathHelper.PiOver4)
                    {
                        rightString = "down";  
                    }
                    else
                    {
                        rightString = "left";
                    }
                }
            }
            #endregion

            //exit game
            if (input.IsNewKeyPress(Keys.Escape)|| gps.Buttons.Back==ButtonState.Pressed)
            {
                LoadingScreen.Load(ScreenManager, false, new MenuBackgroundScreen(),
                    new MainMenuScreen());
            }

            #region Camera Controls
            //move camera
            Vector2 cameraMotion = Vector2.Zero;
            if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.W) ||
                rightString.Equals("up"))
                cameraMotion.Y--;
            if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.S) ||
                rightString.Equals("down"))
                cameraMotion.Y++;
            if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.A) ||
                rightString.Equals("left"))
                cameraMotion.X--;
            if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.D) ||
                rightString.Equals("right"))
                cameraMotion.X++;
            if (cameraMotion != Vector2.Zero)
            {
                //normalize so camera moves at same speed diagonally as horizontal/vertical
                cameraMotion.Normalize();
                camera.Move(cameraMotion, elapsed);
            }

            //zoom in/out with pageup/pagedown keys
            if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.PageUp) ||
                gps.Triggers.Right> 0)
                camera.ZoomIn(elapsed);
            if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.PageDown) ||
                gps.Triggers.Left> 0)
                camera.ZoomOut(elapsed);
            #endregion

            //only exit game or move camera in game over
            if (gameOver)
                return;

            #region Move Selected Square
            //move selected square
            if ((input.IsNewKeyPress(Keys.Up) ||
                gps.DPad.Up == ButtonState.Pressed ||
                 leftString.Equals("up")) && 
                selected.Y > 0)
                selected.Y--;
            if ((input.IsNewKeyPress(Keys.Down) ||
                gps.DPad.Down == ButtonState.Pressed ||
                 leftString.Equals("down"))&& 
                 selected.Y < tileEngine.MapHeight - 1)
                selected.Y++;
            if ((input.IsNewKeyPress(Keys.Left) ||
                gps.DPad.Left == ButtonState.Pressed ||
                 leftString.Equals("left")) && 
                 selected.X > 0)
                selected.X--;
            if ((input.IsNewKeyPress(Keys.Right) ||
                gps.DPad.Right == ButtonState.Pressed ||
                 leftString.Equals("right"))&& 
                selected.X < tileEngine.MapWidth - 1)
                selected.X++;
            #endregion

            #region Select Next/Previous Unit
            //Combat Phase: select next unit
            if (humanPlayers[unitControl.currentPlayer - 1] &&
                (input.IsNewKeyPress(Keys.Tab) || gps.Buttons.RightShoulder == ButtonState.Pressed))
            {
                if (TCs[unitControl.currentPlayer - 1].phase == 2)
                {
                    unitControl.nextUnit();
                    selected.X = unitControl.currentUnit.currentX;
                    selected.Y = unitControl.currentUnit.currentY;
                }
            }

            //Combat Phase: select previous unit
            if (humanPlayers[unitControl.currentPlayer - 1] &&
                (input.IsNewKeyPress(Keys.Back) || gps.Buttons.LeftShoulder == ButtonState.Pressed))
            {
                if (TCs[unitControl.currentPlayer - 1].phase == 2)
                {
                    unitControl.prevUnit();
                    selected.X = unitControl.currentUnit.currentX;
                    selected.Y = unitControl.currentUnit.currentY;
                }
            }
            #endregion

            #region Purchase Units
            //Purchase Phase: Buy an APC
            //Acey Boyce
            if (humanPlayers[unitControl.currentPlayer - 1] &&
                (input.IsNewKeyPress(Keys.D1) || gps.Buttons.X == ButtonState.Pressed))
            {
                if (TCs[unitControl.currentPlayer - 1].phase == 4)
                {
                    if (TCs[unitControl.currentPlayer - 1].totalAPC < TCs[unitControl.currentPlayer - 1].MAXAPC)
                    {
                        if (TCs[unitControl.currentPlayer - 1].points >= 2)
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
                                TCs[unitControl.currentPlayer - 1].points = TCs[unitControl.currentPlayer - 1].points - 2;
                            }
                        }
                    }
                }
            }

            //Purchase Phase: Buy a tank
            //Acey Boyce
            if (humanPlayers[unitControl.currentPlayer - 1] &&
                (input.IsNewKeyPress(Keys.D2) || gps.Buttons.Y == ButtonState.Pressed))
            {
                if (TCs[unitControl.currentPlayer - 1].phase == 4)
                {
                    if (TCs[unitControl.currentPlayer - 1].totalTank < TCs[unitControl.currentPlayer - 1].MAXTANK)
                    {
                        if (TCs[unitControl.currentPlayer - 1].points >= 3)
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
                                TCs[unitControl.currentPlayer - 1].points = TCs[unitControl.currentPlayer - 1].points - 3;
                            }
                        }
                    }
                }
            }

            //Purchase Phase: Buy an artillery
            //Acey Boyce
            if (humanPlayers[unitControl.currentPlayer - 1] &&
                (input.IsNewKeyPress(Keys.D3) || gps.Buttons.B == ButtonState.Pressed))
            {
                if (TCs[unitControl.currentPlayer - 1].phase == 4)
                {
                    if (TCs[unitControl.currentPlayer - 1].totalArtil < TCs[unitControl.currentPlayer - 1].MAXARTIL)
                    {
                        if (TCs[unitControl.currentPlayer - 1].points >= 4)
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
                                TCs[unitControl.currentPlayer - 1].points = TCs[unitControl.currentPlayer - 1].points - 4;
                            }
                        }
                    }
                }
            }
            #endregion

            #region Automatic Unit Placment
            //Robby Florence
            if (!humanPlayers[unitControl.currentPlayer - 1] && TCs[unitControl.currentPlayer - 1].phase == 0)
            {
                int x, y;
                Random rand = new Random();

                //add apcs
                for (int i = 0; i < TC1.MAXAPC; i++)
                {
                    //find an empty location in the player's base
                    do
                    {
                        x = rand.Next(TCs[unitControl.currentPlayer - 1].startingSmallX, TCs[unitControl.currentPlayer - 1].startingBigX + 1);
                        y = rand.Next(TCs[unitControl.currentPlayer - 1].startingSmallY, TCs[unitControl.currentPlayer - 1].startingBigY + 1);
                    }
                    while (!TCs[unitControl.currentPlayer - 1].createUnit("apc", x, y));

                    //create unit
                    if (unitControl.currentPlayer == 1)
                        unitControl.addUnit("apc", 1, x, y, apc1);
                    else if (unitControl.currentPlayer == 2)
                        unitControl.addUnit("apc", 2, x, y, apc2);
                    else if (unitControl.currentPlayer == 3)
                        unitControl.addUnit("apc", 3, x, y, apc3);
                    else if (unitControl.currentPlayer == 4)
                        unitControl.addUnit("apc", 4, x, y, apc4);

                    TCs[unitControl.currentPlayer - 1].totalAPC++;
                }

                //add tanks
                for (int i = 0; i < TC1.MAXTANK; i++)
                {
                    //find an empty location in the player's base
                    do
                    {
                        x = rand.Next(TCs[unitControl.currentPlayer - 1].startingSmallX, TCs[unitControl.currentPlayer - 1].startingBigX + 1);
                        y = rand.Next(TCs[unitControl.currentPlayer - 1].startingSmallY, TCs[unitControl.currentPlayer - 1].startingBigY + 1);
                    }
                    while (!TCs[unitControl.currentPlayer - 1].createUnit("tank", x, y));

                    //create unit
                    if (unitControl.currentPlayer == 1)
                        unitControl.addUnit("tank", 1, x, y, tank1);
                    else if (unitControl.currentPlayer == 2)
                        unitControl.addUnit("tank", 2, x, y, tank2);
                    else if (unitControl.currentPlayer == 3)
                        unitControl.addUnit("tank", 3, x, y, tank3);
                    else if (unitControl.currentPlayer == 4)
                        unitControl.addUnit("tank", 4, x, y, tank4);

                    TCs[unitControl.currentPlayer - 1].totalTank++;
                }

                //add artillery
                for (int i = 0; i < TC1.MAXARTIL; i++)
                {
                    //find an empty location in the player's base
                    do
                    {
                        x = rand.Next(TCs[unitControl.currentPlayer - 1].startingSmallX, TCs[unitControl.currentPlayer - 1].startingBigX + 1);
                        y = rand.Next(TCs[unitControl.currentPlayer - 1].startingSmallY, TCs[unitControl.currentPlayer - 1].startingBigY + 1);
                    }
                    while (!TCs[unitControl.currentPlayer - 1].createUnit("artillery", x, y));

                    //create unit
                    if (unitControl.currentPlayer == 1)
                        unitControl.addUnit("artillery", 1, x, y, artillery1);
                    else if (unitControl.currentPlayer == 2)
                        unitControl.addUnit("artillery", 2, x, y, artillery2);
                    else if (unitControl.currentPlayer == 3)
                        unitControl.addUnit("artillery", 3, x, y, artillery3);
                    else if (unitControl.currentPlayer == 4)
                        unitControl.addUnit("artillery", 4, x, y, artillery4);

                    TCs[unitControl.currentPlayer - 1].totalAPC++;
                }

                finalizeUnitPlacement();
            }
            #endregion

            #region Movement and Unit Placement
            //Movement Phase:move current unit to selected square
            //Unit Placement Phase: Place next unit on selected square
            if (humanPlayers[unitControl.currentPlayer - 1] &&
                (input.IsNewKeyPress(Keys.Space) || gps.Buttons.A == ButtonState.Pressed))
            {
                if (TCs[unitControl.currentPlayer - 1].phase == 2)
                {
                    if ((unitControl.unitAt(selected.X, selected.Y).team == unitControl.currentPlayer) &&
                        !unitControl.unitAt(selected.X, selected.Y).hasMoved)
                    {
                        unitControl.currentUnit = unitControl.unitAt(selected.X, selected.Y);
                    }
                    else
                    {
                        AI.map[unitControl.currentUnit.currentX, unitControl.currentUnit.currentY] = 0;
                        unitControl.moveUnit(selected.X, selected.Y);   
                    }
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
                            finalizeUnitPlacement();
                        }
                    }
                }
            }
            #endregion

            #region Next Phase/Finalize Turn
            //Unit Placement Phase: No Function
            //Movement Phase: Next Phase
            //Purchase Phase: Finalize and End Turn
            if (humanPlayers[unitControl.currentPlayer - 1] &&
                (input.IsNewKeyPress(Keys.Enter) || gps.Buttons.Start == ButtonState.Pressed))
            {
                if (TCs[unitControl.currentPlayer - 1].phase == 2)
                {
                    TCs[unitControl.currentPlayer - 1].nextPhase();

                    //move selection to player's base for purchase phase
                    if (unitControl.currentPlayer == 1)
                    {
                        selected.X = 0;
                        selected.Y = 0;
                    }
                    else if (unitControl.currentPlayer == 2)
                    {
                        selected.X = tileEngine.MapWidth - 3;
                        selected.Y = 0;
                    }
                    else if (unitControl.currentPlayer == 3)
                    {
                        selected.X = tileEngine.MapWidth - 3;
                        selected.Y = tileEngine.MapHeight - 3;
                    }
                    else if (unitControl.currentPlayer == 4)
                    {
                        selected.X = 0;
                        selected.Y = tileEngine.MapHeight - 3;
                    }
                }
                else if (TCs[unitControl.currentPlayer - 1].phase == 4)
                {
                    int prevTeam = unitControl.currentPlayer;
                    TCs[unitControl.currentPlayer - 1].nextPhase();
                    TCs[prevTeam - 1].setNext(TCs[unitControl.finalize() - 1]);
                    TCs[prevTeam - 1].getNext().nextPhase();
                    selected.X = unitControl.currentUnit.currentX;
                    selected.Y = unitControl.currentUnit.currentY;
                    //change player number for cameras
                    camera.PlayerNum = unitControl.currentPlayer;
                    miniMap.PlayerNum = unitControl.currentPlayer;
                }
                if (TCs[unitControl.currentPlayer - 1].phase == 5)
                {
                    int prevTeam = unitControl.currentPlayer;
                    TCs[prevTeam - 1].setNext(TCs[unitControl.finalize() - 1]);
                    TCs[prevTeam - 1].getNext().nextPhase();
                    selected.X = unitControl.currentUnit.currentX;
                    selected.Y = unitControl.currentUnit.currentY;
                    //change player number for cameras
                    camera.PlayerNum = unitControl.currentPlayer;
                    miniMap.PlayerNum = unitControl.currentPlayer;
                }
            }
            #endregion


            //This code is executed when AI gets its turn
            //Hiral Vora
            #region AI Movement
            if (!humanPlayers[unitControl.currentPlayer - 1])
            {

                #region Old method
                //if (previousunit == null)
                //{
                //    previousunit = new NullUnit();
                //    previousunit = unitControl.currentUnit;
                //    Point point = ai1.NextMove();
                //    selected.X = point.X;
                //    selected.Y = point.Y;

                //} 
                #endregion

                if (TCs[unitControl.currentPlayer - 1].phase == 2)
                {
                    if (!AI.currentmovingunit.isMoving)
                    {
                        //previousunit = unitControl.currentUnit;
                        int result = ai1.NextMove();
                        selected.X = AI.currentmovingunit.currentX;
                        selected.Y = AI.currentmovingunit.currentY;
                        camera.PlayerNum = unitControl.currentPlayer;
                        miniMap.PlayerNum = unitControl.currentPlayer;

                        if (result == 0)
                        {
                            TCs[unitControl.currentPlayer - 1].nextPhase();

                            if (TCs[unitControl.currentPlayer - 1].phase == 3)
                            {
                                TCs[unitControl.currentPlayer - 1].nextPhase();
                            }

                            if (TCs[unitControl.currentPlayer - 1].phase == 4)
                            {
                                int prevTeam = unitControl.currentPlayer;
                                TCs[unitControl.currentPlayer - 1].nextPhase();
                                TCs[prevTeam - 1].setNext(TCs[unitControl.finalize() - 1]);
                                TCs[prevTeam - 1].getNext().nextPhase();
                                selected.X = unitControl.currentUnit.currentX;
                                selected.Y = unitControl.currentUnit.currentY;
                                //change player number for cameras
                                camera.PlayerNum = unitControl.currentPlayer;
                                miniMap.PlayerNum = unitControl.currentPlayer;
                            }
                            if (TCs[unitControl.currentPlayer - 1].phase == 5)
                            {
                                int prevTeam = unitControl.currentPlayer;
                                TCs[prevTeam - 1].setNext(TCs[unitControl.finalize() - 1]);
                                TCs[prevTeam - 1].getNext().nextPhase();
                                selected.X = unitControl.currentUnit.currentX;
                                selected.Y = unitControl.currentUnit.currentY;
                                //change player number for cameras
                                camera.PlayerNum = unitControl.currentPlayer;
                                miniMap.PlayerNum = unitControl.currentPlayer;
                            }

                            #region Old AI
                            //Unit mostouter = unitControl.currentUnit;
                            //Unit firstunit = unitControl.currentUnit;
                            //unitControl.nextUnit();
                            //Unit nextunit = unitControl.currentUnit;

                            //while (nextunit != firstunit)
                            //{
                            //    if (AI.values[nextunit.currentX, nextunit.currentY] > AI.values[mostouter.currentX, mostouter.currentY])
                            //        mostouter = nextunit;

                            //    unitControl.nextUnit();
                            //    nextunit = unitControl.currentUnit;
                            //}

                            //while (unitControl.currentUnit != mostouter)
                            //    unitControl.nextUnit();

                            //int previousx, previousy, result;
                            //previousx = unitControl.currentUnit.currentX;
                            //previousy = unitControl.currentUnit.currentY;

                            //List<moves> possiblemoves = unitControl.currentUnit.GetAllpossibleMoves();
                            //moves move = ai1.FindBestPossibleMove(possiblemoves);

                            //result = unitControl.moveUnit(move.x, move.y);

                            //if (result == 3)
                            //{
                            //    while (result != 1)
                            //    {
                            //        possiblemoves.Remove(move);
                            //        if (possiblemoves.Count > 0)
                            //        {
                            //            move = ai1.FindBestPossibleMove(possiblemoves);
                            //            result = unitControl.moveUnit(move.x, move.y);
                            //        }
                            //        else
                            //            break;
                            //    }
                            //}

                            //if (result == 1)
                            //{
                            //    //AI update map
                            //    AI.map[previousx, previousy] = 0;
                            //    AI.map[unitControl.currentUnit.currentX, unitControl.currentUnit.currentY] = unitControl.currentUnit.typeno;
                            //}

                            //selected.X = unitControl.currentUnit.currentX;
                            //selected.Y = unitControl.currentUnit.currentY;

                            #endregion
                        }
                    }
                }
            }
            #endregion

            //move camera to selected square
            if (!prevSelected.Equals(selected))
                camera.JumpTo(selected.X, selected.Y);
            prevSelected = selected;
        }

        //this code is repeated at the end of AI and human unit placement
        //so it needs to be in one method
        //it advances to next player and moves selection to next player's base
        //Robby Florence
        public void finalizeUnitPlacement()
        {
            TCs[unitControl.currentPlayer - 1].phase = 5;
            unitControl.currentPlayer++;
            if (unitControl.currentPlayer == 2)
            {
                selected.X = tileEngine.MapWidth - 3;
                selected.Y = 0;
            }
            else if (unitControl.currentPlayer == 3)
            {
                selected.X = tileEngine.MapWidth - 3;
                selected.Y = tileEngine.MapHeight - 3;
            }
            else if (unitControl.currentPlayer == 4)
            {
                selected.X = 0;
                selected.Y = tileEngine.MapHeight - 3;
            }
            else if (unitControl.currentPlayer == 5)
            {
                unitControl.currentPlayer = 1;
                TCs[unitControl.currentPlayer - 1].nextPhase();
                unitControl.nextUnit();
                selected.X = unitControl.currentUnit.currentX;
                selected.Y = unitControl.currentUnit.currentY;
            }
            camera.PlayerNum = unitControl.currentPlayer;
            miniMap.PlayerNum = unitControl.currentPlayer;
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
            tileEngine.Draw(spriteBatch, camera, unitControl, selected, gameOver);
            tileEngine.Draw(spriteBatch, miniMap, unitControl, gameOver);

            hud.Draw(spriteBatch);

            spriteBatch.End();

            #region Draw Game Over
            //draw game over text
            if (gameOver)
            {
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                SpriteFont font = ScreenManager.Font;
                String winnerText;
                String conditionText = "";
                String exitText = "Press Esc/Back To Exit";
                Color textColor;
                float textScale;
                Vector2 textSize;
                Vector2 textPosition;

                //set text and color for winning team
                if (unitControl.currentPlayer == 1)
                {
                    winnerText = "White Team Wins!";
                    textColor = new Color(240, 240, 240);
                }
                else if (unitControl.currentPlayer == 2)
                {
                    winnerText = "Green Team Wins!";
                    textColor = new Color(150, 200, 140);
                }
                else if (unitControl.currentPlayer == 3)
                {
                    winnerText = "Grey Team Wins!";
                    textColor = new Color(180, 180, 180);
                }
                else
                {
                    winnerText = "Brown Team Wins!";
                    textColor = new Color(190, 160, 100);
                }

                //set text for victory condition
                if (TCs[unitControl.currentPlayer - 1].victoryCondition == (int)TurnController.VictoryCond.Factories)
                    conditionText = "Control 80% of Factories";
                else if (TCs[unitControl.currentPlayer - 1].victoryCondition == (int)TurnController.VictoryCond.Points)
                    conditionText = "Accumulate " + TCs[unitControl.currentPlayer - 1].maxPoints + " Points";
                else if (TCs[unitControl.currentPlayer - 1].victoryCondition == (int)TurnController.VictoryCond.NoOpponents)
                    conditionText = "Eliminate All Opponents";

                
                ScreenManager.FadeBackBufferToBlack(150);
                spriteBatch.Begin();

                //draw winner text
                textSize = font.MeasureString(winnerText);
                textScale = 0.1f * viewport.Height / textSize.Y;
                //center text horizontally
                textPosition = new Vector2(
                    viewport.X + 0.5f * (viewport.Width - textScale * textSize.X),
                    viewport.Y + 0.2f * viewport.Height);

                spriteBatch.DrawString(
                    font,
                    winnerText,
                    textPosition,
                    textColor,
                    0f,
                    Vector2.Zero,
                    textScale,
                    SpriteEffects.None,
                    0f);

                //draw victory condition text
                textSize = font.MeasureString(conditionText);
                textScale *= 0.75f;
                //center text horizontally
                textPosition = new Vector2(
                    viewport.X + 0.5f * (viewport.Width - textScale * textSize.X),
                    viewport.Y + 0.3f * viewport.Height);

                spriteBatch.DrawString(
                    font,
                    conditionText,
                    textPosition,
                    textColor,
                    0f,
                    Vector2.Zero,
                    textScale,
                    SpriteEffects.None,
                    0f);

                //draw exit text
                textSize = font.MeasureString(exitText);
                //center text horizontally
                textPosition = new Vector2(
                    viewport.X + 0.5f * (viewport.Width - textScale * textSize.X),
                    viewport.Y + 0.6f * viewport.Height);

                spriteBatch.DrawString(
                    font,
                    exitText,
                    textPosition,
                    textColor,
                    0f,
                    Vector2.Zero,
                    textScale,
                    SpriteEffects.None,
                    0f);

                spriteBatch.End();
            }
            #endregion

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }
        #endregion
    }
}