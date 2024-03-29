#region File Description
//-----------------------------------------------------------------------------
// AudioScreen.cs
//
// Display the AudioScreen menu. --Chris Wykel
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
    /// The AudioScreen menu...
    /// </summary>
    class AudioScreen : GameScreen
    {
        #region Fields

        //graphics and audio
        ContentManager content;
        GameAudio gameAudio;

        //textures
        Texture2D MenuImage;

        //vectors
        private Vector2 origin;
        private Vector2 screenpos;

        //rotation angles for turning animation
        private float RotationAngle = 0.0f;
        private float top = 0.0f;
        private float rightside = -2.25f;
        private float leftside = 2.15f;
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
        public AudioScreen()
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
            MenuImage = content.Load<Texture2D>("Main Menu/Master copy");
            origin.X = MenuImage.Width / 2;
            origin.Y = MenuImage.Height / 2;

            gameAudio = ScreenManager.Audio;
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
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //get elapsed time since last update
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            GamePadState gps = GamePad.GetState(PlayerIndex.One);

            //go back to options screen
            if (input.MenuCancel)
            {
                gameAudio.PlaySound("Menu Sound");
                ExitScreen();
                ScreenManager.AddScreen(new OptionsScreen());
            }
            else if ((!input.IsKeyDownLeft() && !input.IsKeyDownRight()))
            {
                //select master audio
                if (input.MenuSelect)
                {
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
                //select sound effects
                if (input.MenuSelect)
                {
                }

                //rotate menu
                if (RotationAngle < leftside)
                {
                    RotationAngle += elapsed * 1.5f;
                    RotationAngle = RotationAngle % circle;
                }
                if (RotationAngle > leftside)
                {
                    RotationAngle -= elapsed * 1.5f;
                    RotationAngle = RotationAngle % circle;
                }
            }
            else if (input.IsKeyDownRight())
            {
                //select background music
                if (input.MenuSelect)
                {
                }

                //rotate menu
                if (RotationAngle > rightside)
                {
                    RotationAngle -= elapsed * 1.5f;
                    RotationAngle = RotationAngle % circle;
                }
                if (RotationAngle < rightside)
                {
                    RotationAngle += elapsed * 1.5f;
                    RotationAngle = RotationAngle % circle;
                }
            }
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                if ((gps.DPad.Left == ButtonState.Released && gps.DPad.Right == ButtonState.Released))
                {
                    //select master audio
                    if (input.MenuSelect)
                    {
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
                    //select sound effects
                    if (input.MenuSelect)
                    {
                    }

                    //rotate menu
                    if (RotationAngle < leftside)
                    {
                        RotationAngle += elapsed * 1.5f;
                        RotationAngle = RotationAngle % circle;
                    }
                    if (RotationAngle > leftside)
                    {
                        RotationAngle -= elapsed * 1.5f;
                        RotationAngle = RotationAngle % circle;
                    }
                }
                else if (gps.DPad.Right == ButtonState.Pressed)
                {
                    //select background music
                    if (input.MenuSelect)
                    {
                    }

                    //rotate menu
                    if (RotationAngle > rightside)
                    {
                        RotationAngle -= elapsed * 1.5f;
                        RotationAngle = RotationAngle % circle;
                    }
                    if (RotationAngle < rightside)
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
            Vector2 textPosition = new Vector2((viewport.Width - 2*textSize.X) / 2, (viewport.Height - 2*textSize.Y) / 2);
                        
            spriteBatch.Begin();

            //draw menu items
            spriteBatch.Draw(
                MenuImage,
                screenpos,
                null,
                new Color(fade, fade, fade), //fade during transitions
                RotationAngle,
                origin,
                1.0f, //scale
                SpriteEffects.None,
                0f
                );

            //draw menu title
            spriteBatch.DrawString(
                font,
                title,
                textPosition,
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
