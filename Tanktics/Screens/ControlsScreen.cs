﻿#region File Description
//-----------------------------------------------------------------------------
// ControlsScreen.cs
//
// Controls screenfor Tanktics. --Acey Boyce
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
    /// <summary>
    /// A Screen for informing the user of what buttons
    /// do what.
    /// </summary>
    class ControlsScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        Texture2D controlsTexture;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public ControlsScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Load graphics content for the credits.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            controlsTexture = content.Load<Texture2D>("Main Menu/controls");
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                      bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            //go back to control
            if (input.MenuCancel)
            {
                LoadingScreen.Load(ScreenManager, false, new MenuBackgroundScreen(),
                    new ControlScreen());
            }
        }

        /// <summary>
        /// Draws the controls screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            byte fade = TransitionAlpha;

            spriteBatch.Begin();

            spriteBatch.Draw(
                controlsTexture,
                fullscreen,
                new Color(fade, fade, fade)
                );

            spriteBatch.End();
        }

        #endregion
    }
}
