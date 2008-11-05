#region File Description
//-----------------------------------------------------------------------------
// MenuBackgroundScreen.cs
//
// Display the background image for the main menu.
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
    /// The menu background screen displays the fullscreen image of the tank barrel
    /// for the main menu background.
    /// </summary>
    class MenuBackgroundScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        Texture2D backgroundTexture;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuBackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);
        }

        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>("Main Menu/background");
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
        /// Updates the menu background screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        /// <summary>
        /// Draws the menu background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            byte fade = TransitionAlpha;

            spriteBatch.Begin();
            
            spriteBatch.Draw(backgroundTexture, fullscreen, new Color(fade, fade, fade));

            spriteBatch.End();
        }

        #endregion
    }
}
