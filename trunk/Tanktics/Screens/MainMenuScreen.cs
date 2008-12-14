#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Displays MainMenuScreen. --Chris Wykel
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Tanktics
{
    /// <summary>
    /// The main menu screen is the main menu...
    /// </summary>
    class MainMenuScreen : GameScreen
    {
        #region Fields

        //graphics and Audio
        ContentManager content;
        GameAudio gameAudio;

        //textures
        Texture2D MenuImage;

        //vectors
        private Vector2 origin;
        private Vector2 screenpos;

        //rotation angle for turning animation
        private float RotationAngle = 0.0f;
        private float top = 0.0f; 
        private float right = -1.65f;
        private float left = 1.55f;
        private float bottom = -3.2f;
        private float circle = MathHelper.Pi * 2;

        //elapsed time between updates
        float elapsed = 0f;

        //menu title
        string title = "TankTics";

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainMenuScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);
        }

        /// <summary>
        /// Loads graphics content for this screen. The menu texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            //load stuff
            MenuImage = content.Load<Texture2D>("Main Menu/new copy");
            origin.X = MenuImage.Width / 2;
            origin.Y = MenuImage.Height / 2;

            //gets Audio from ScreenManager
            gameAudio = ScreenManager.Audio;
            gameAudio.AddSound("Menu Sound");
        }

        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the main menu screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //get elapsed time since last update
            elapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (!input.IsKeyDownLeft() && !input.IsKeyDownRight() && !input.IsKeyDownDown())
            {
                //select new game
                if (input.MenuSelect)
                {
                    gameAudio.PlaySound("Menu Sound");
                    ExitScreen();
                    ScreenManager.AddScreen(new NewGameScreen());
                }

                //rotate menu
                if (RotationAngle > top)
                {
                    RotationAngle -= elapsed * 1.5f;
                    RotationAngle = RotationAngle % circle;
                }
                if (RotationAngle < top)
                {
                    RotationAngle += elapsed * 1.5f;
                    RotationAngle = RotationAngle % circle;
                }
            }
            else if (input.IsKeyDownLeft())
            {
                //select credits
                if (input.MenuSelect)
                {
                    gameAudio.PlaySound("Menu Sound");
                    LoadingScreen.Load(ScreenManager, false, new CreditsScreen());
                }

                //rotate menu
                if (RotationAngle < left)
                {
                    RotationAngle += elapsed * 1.5f;
                    RotationAngle = RotationAngle % circle;
                }
                if (RotationAngle > left)
                {
                    RotationAngle -= elapsed * 1.5f;
                    RotationAngle = RotationAngle % circle;
                }
            }
            else if (input.IsKeyDownRight())
            {
                //select options
                if (input.MenuSelect)
                {
                    gameAudio.PlaySound("Menu Sound");
                    ExitScreen();
                    ScreenManager.AddScreen(new OptionsScreen());
                }

                //rotate menu
                if (RotationAngle > right)
                {
                    RotationAngle -= elapsed * 1.5f;
                    RotationAngle = RotationAngle % circle;
                }
                if (RotationAngle < right)
                {
                    RotationAngle += elapsed * 1.5f;
                    RotationAngle = RotationAngle % circle;
                }
            }
            else if (input.IsKeyDownDown())
            {
                //select exit
                if (input.MenuSelect)
                {
                    gameAudio.PlaySound("Menu Sound");
                    ScreenManager.Game.Exit();
                }
                
                //rotate menu
                if (RotationAngle > bottom)
                {
                    RotationAngle -= elapsed * 1.5f;
                    RotationAngle = RotationAngle % circle;
                }
                if (RotationAngle < bottom)
                {
                    RotationAngle += elapsed * 1.5f;
                    RotationAngle = RotationAngle % circle;
                }
            }
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                GamePadState gps = GamePad.GetState(PlayerIndex.One);
                if (gps.DPad.Left == ButtonState.Released && gps.DPad.Right == ButtonState.Released && gps.DPad.Down == ButtonState.Released)
                {
                    //select new game
                    if (input.MenuSelect)
                    {
                        gameAudio.PlaySound("Menu Sound");
                        ExitScreen();
                        ScreenManager.AddScreen(new NewGameScreen());
                    }

                    //rotate menu
                    if (RotationAngle > top)
                    {
                        RotationAngle -= elapsed * 1.5f;
                        RotationAngle = RotationAngle % circle;
                    }
                    if (RotationAngle < top)
                    {
                        RotationAngle += elapsed * 1.5f;
                        RotationAngle = RotationAngle % circle;
                    }
                }
                else if (gps.DPad.Left == ButtonState.Pressed)
                {
                    //select credits
                    if (input.MenuSelect)
                    {
                        gameAudio.PlaySound("Menu Sound");
                        LoadingScreen.Load(ScreenManager, false, new CreditsScreen());
                    }

                    //rotate menu
                    if (RotationAngle < left)
                    {
                        RotationAngle += elapsed * 1.5f;
                        RotationAngle = RotationAngle % circle;
                    }
                    if (RotationAngle > left)
                    {
                        RotationAngle -= elapsed * 1.5f;
                        RotationAngle = RotationAngle % circle;
                    }
                }
                else if (gps.DPad.Right == ButtonState.Pressed)
                {
                    //select options
                    if (input.MenuSelect)
                    {
                        gameAudio.PlaySound("Menu Sound");
                        ExitScreen();
                        ScreenManager.AddScreen(new OptionsScreen());
                    }

                    //rotate menu
                    if (RotationAngle > right)
                    {
                        RotationAngle -= elapsed * 1.5f;
                        RotationAngle = RotationAngle % circle;
                    }
                    if (RotationAngle < right)
                    {
                        RotationAngle += elapsed * 1.5f;
                        RotationAngle = RotationAngle % circle;
                    }
                }
                else if (gps.DPad.Down == ButtonState.Pressed)
                {
                    //select exit
                    if (input.MenuSelect)
                    {
                        gameAudio.PlaySound("Menu Sound");
                        ScreenManager.Game.Exit();
                    }

                    //rotate menu
                    if (RotationAngle > bottom)
                    {
                        RotationAngle -= elapsed * 1.5f;
                        RotationAngle = RotationAngle % circle;
                    }
                    if (RotationAngle < bottom)
                    {
                        RotationAngle += elapsed * 1.5f;
                        RotationAngle = RotationAngle % circle;
                    }
                }
            }
        }

        /// <summary>
        /// Draws the main menu screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            screenpos.X = viewport.Width / 2;
            screenpos.Y = viewport.Height / 2;
            byte fade = TransitionAlpha;
            
            SpriteFont font = ScreenManager.Font;
            Vector2 textSize = font.MeasureString(title);
            Vector2 textPosition = new Vector2((viewport.Width - 2 * textSize.X) / 2, (viewport.Height - 2 * textSize.Y) / 2);
            
            spriteBatch.Begin();

            //draw menu items
            spriteBatch.Draw(
                MenuImage,
                screenpos,
                null,
                new Color(fade, fade, fade), //fade during transitions
                RotationAngle, //rotate
                origin,
                1.0f, //scale
                SpriteEffects.None,
                0f
                );

            //draw menu title
            spriteBatch.DrawString(
                font,
                title,
                textPosition, //centered
                Color.Goldenrod,
                0.0f,
                Vector2.Zero,
                2.0f, //scale
                SpriteEffects.None,
                0.0f
                );

            spriteBatch.End();
        }

        #endregion
    }
}


