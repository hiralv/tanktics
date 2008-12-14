#region File Description
//-----------------------------------------------------------------------------
// LogoTitleScreen.cs
//
// Display title as popup screen on top of the logo screen.-- robby Flourence
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
    /// The logo title screen displays "A Box of Elephants Production" then exits.
    /// </summary>
    //Robby Florence
    class LogoTitleScreen : GameScreen
    {
        #region Fields

        string title, iGDDline;
        TimeSpan displayTime = TimeSpan.FromSeconds(3.0);
        TimeSpan elapsedTime = TimeSpan.Zero;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogoTitleScreen()
        {
            title = "A Box of Elephants Production";
            iGDDline = "ITCS 5230 - iGDD";
            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the logo title screen. This only increments the elapsed time counter
        /// and exits the screen after a set amount of display time.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //transition off after display time is reached
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime >= displayTime)
                ExitScreen();
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
        /// Draws the logo title.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            // Center the title text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 textSize = font.MeasureString(title);
            Vector2 textPosition = new Vector2((viewport.Width - textSize.X) / 2, 80);

            byte fade = TransitionAlpha;

            spriteBatch.Begin();

            //draw title line
            spriteBatch.DrawString(font, title, textPosition, new Color(fade, fade, fade));

            //center iGDD text in the viewport
            textSize = font.MeasureString(iGDDline);
            textPosition.X = (viewport.Width - textSize.X) / 2;
            textPosition.Y = 480;
            //draw iGDD line
            spriteBatch.DrawString(font, iGDDline, textPosition, new Color(fade, fade, fade));

            spriteBatch.End();
        }

        #endregion
    }
}
