#region File Description
//-----------------------------------------------------------------------------
// LogoScreen.cs
//
// Display the Box of Elephants logo and start the GDD logo screen.
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
    /// The logo screen displays the Box of Elephants logo and starts the GDD logo screen.
    /// </summary>
    //Robby Florence
    class LogoScreen : GameScreen
    {
        #region Fields
        GameAudio gameAudio;
        ContentManager content;
        Texture2D logoTexture;
        float scale = 0.67f;
        TimeSpan elapsedTime = TimeSpan.Zero;

        bool startedGDDLogo = false;
        TimeSpan startGDDLogoTime = TimeSpan.FromSeconds(4.0);

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogoScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
        }

        /// <summary>
        /// Loads graphics content for this screen. The logo texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            logoTexture = content.Load<Texture2D>("Intro/BoxOfElephants");

            gameAudio = ScreenManager.Audio;
            gameAudio.AddSound("Elephant Logo");
            gameAudio.PlaySound("Elephant Logo");
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
        /// Updates the logo screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            elapsedTime += gameTime.ElapsedGameTime;

            //start the GDD logo screen after the logo title screen exits
            if (!startedGDDLogo && elapsedTime >= startGDDLogoTime)
            {
                startedGDDLogo = true;
                ScreenManager.AddScreen(new GDDLogoScreen());
                ExitScreen();
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

            //skip intro
            if (input.PauseGame)
            {
                LoadingScreen.Load(ScreenManager, false, new MenuBackgroundScreen(),
                    new MainMenuScreen());
            }
        }

        /// <summary>
        /// Draws the logo screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            int spriteW = (int)(scale * logoTexture.Width);
            int spriteH = (int)(scale * logoTexture.Height);

            byte fade = TransitionAlpha;

            spriteBatch.Begin();
            
            spriteBatch.Draw(logoTexture,
                new Vector2((viewport.Width-spriteW)/2, (viewport.Height-spriteH)/2), //centered
                null,
                new Color(fade, fade, fade), //fade during transitions
                0.0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0.0f
                );

            spriteBatch.End();
        }

        #endregion
    }
}
