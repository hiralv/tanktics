#region File Description
//-----------------------------------------------------------------------------
// WarningScreen.cs
//
// Display the "A for Awesome" warning image.
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
    /// The warning screen displays the warning logo and then exits.
    /// </summary>
    class WarningScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        Texture2D warningTexture;
        TimeSpan displayTime = TimeSpan.FromSeconds(3.0);
        TimeSpan elapsedTime = TimeSpan.Zero;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public WarningScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
        }

        /// <summary>
        /// Loads graphics content for this screen. The warning texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            warningTexture = content.Load<Texture2D>("Intro/WarningLogo");
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
        /// Updates the warning screen. This only increments the elapsed time counter
        /// and loads the logo screen after a set amount of display time.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //transition to logo screen after display time is reached
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime >= displayTime)
                LoadingScreen.Load(ScreenManager, false, new LogoScreen(), new LogoTitleScreen());
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            //skip intro
            if (input.PauseGame)
            {
                LoadingScreen.Load(ScreenManager, false, new MenuBackgroundScreen(),
                    new MainMenuScreen());
            }
        }

        /// <summary>
        /// Draws the warning screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            //scale the sprite by 1/2
            int spriteW = warningTexture.Width / 2;
            int spriteH = warningTexture.Height / 2;

            byte fade = TransitionAlpha;

            spriteBatch.Begin();

            spriteBatch.Draw(warningTexture,
                new Vector2((viewport.Width - spriteW) / 2, (viewport.Height - spriteH) / 2), //centered
                null,
                new Color(fade, fade, fade), //fade during transitions
                0.0f,
                Vector2.Zero,
                0.5f, //scale by 1/2
                SpriteEffects.None,
                0.0f
                );

            spriteBatch.End();
        }

        #endregion
    }
}
