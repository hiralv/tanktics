#region File Description
//-----------------------------------------------------------------------------
// OptionScreen.cs
//
// Display the NewGame menu.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Tanktics
{
    /// <summary>
    /// The main menu screen is the main menu...
    /// </summary>
    class NewGameScreen : GameScreen
    {
        #region Fields

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
        public NewGameScreen()
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
            MenuImage = content.Load<Texture2D>("Main Menu/single copy");
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
            //go back to main menu screen
            if (input.MenuCancel)
            {
                gameAudio.PlaySound("Menu Sound");
                ExitScreen();
                ScreenManager.AddScreen(new MainMenuScreen());
            }
            else if (!input.IsKeyDownLeft() && !input.IsKeyDownRight() && !input.IsKeyDownDown())
            {
                //select single player
                if (input.MenuSelect)
                {
                    gameAudio.PlaySound("Menu Sound");
                    //LoadingScreen.Load(ScreenManager, true, new GameplayScreen(), new Hud(650, 450, 160, 160));
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
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
            else if (input.IsKeyDownDown() || input.IsKeyDownRight() || input.IsKeyDownLeft())
            {
                //select multiplayer
                if (input.MenuSelect)
                {
                    gameAudio.PlaySound("Menu Sound");
                    ExitScreen();
                    ScreenManager.AddScreen(new MultiPlayerScreen());
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
