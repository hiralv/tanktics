#region File Description
//-----------------------------------------------------------------------------
// TankIntroScreen.cs
//
// Display the tank animations to transition to the main menu.--Robby Florence
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Tanktics
{
    /// <summary>
    /// The tank intro screen displays a tank shooting the logo, moving to the center
    /// of the screen, and zooming in on the tank's gun barrel. Then this screen
    /// exits and transitions to the main menu.
    /// </summary>
    //Robby Florence
    class TankIntroScreen : GameScreen
    {
        #region Fields
        //Graphics and Audio
        GameAudio gameAudio;
        ContentManager content;

        AnimatingSprite driveSprite, turnSprite, zoomSprite;

        //increment to move animations each frame
        float moveX = 0;

        //previous frame of drive and turn animations
        //used to determine when the frame advances
        int drivePrevFrame = -1, turnPrevFrame = -1;

        //if the animation has been delayed between turn and zoom
        bool zoomDelayed = false;

        //if the positions of the animations have been set in the Draw method
        bool initialized = false;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public TankIntroScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);

            //create animating sprites
            driveSprite = new AnimatingSprite();
            turnSprite = new AnimatingSprite();
            zoomSprite = new AnimatingSprite();

            //create animations
            Animation drive = new Animation(1600, 1200, 21, 5, 5, 0, 0);
            Animation turn = new Animation(1600, 1200, 21, 5, 5, 0, 0);
            Animation zoom = new Animation(1600, 1200, 21, 5, 5, 0, 0);

            //add animations to corresponding sprites
            driveSprite.Animations.Add("drive", drive);
            turnSprite.Animations.Add("turn", turn);
            zoomSprite.Animations.Add("zoom", zoom);
            driveSprite.CurrentAnimation = "drive";
            turnSprite.CurrentAnimation = "turn";
            zoomSprite.CurrentAnimation = "zoom";
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

            driveSprite.Texture = content.Load<Texture2D>("Intro/Driving Tank Animation");
            turnSprite.Texture = content.Load<Texture2D>("Intro/turning tank animation");
            zoomSprite.Texture = content.Load<Texture2D>("Intro/Zooming tank Animation");

            //wait for 1.25 seconds
            //this is just an easy way to delay the start of the tank moving on the screen
            Thread.Sleep(1250);
            ScreenManager.Game.ResetElapsedTime();

            gameAudio = ScreenManager.Audio;
            gameAudio.AddSound("tank running engine");
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
        /// Updates the tank intro screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (!driveSprite.IsFinished)
            {
                gameAudio.PlaySound("tank running engine");
                //frame has advanced
                if (driveSprite.Animations["drive"].CurrentFrameNum > drivePrevFrame)
                {
                    //move left
                    driveSprite.Position.X -= moveX;
                    drivePrevFrame = driveSprite.Animations["drive"].CurrentFrameNum;
                }

                driveSprite.Update(gameTime);
            }
            //drive has finished
            else if (!turnSprite.IsFinished)
            {
                gameAudio.PlaySound("tank running engine");
                //frame has advanced
                if (turnSprite.Animations["turn"].CurrentFrameNum > turnPrevFrame)
                {
                    //move left
                    turnSprite.Position.X -= moveX;
                    turnPrevFrame = turnSprite.Animations["turn"].CurrentFrameNum;
                }

                turnSprite.Update(gameTime);
            }
            //drive and turn have finished
            else if (!zoomSprite.IsFinished)
            {
                //zoom animation has not started yet
                if (!zoomDelayed)
                {
                    //wait for 1 second before starting zoom
                    Thread.Sleep(1000);
                    ScreenManager.Game.ResetElapsedTime();
                    zoomDelayed = true;
                }

                zoomSprite.Update(gameTime);
            }
            //all animations have finished
            else
            {
                //transition to main menu
                ExitScreen();
                ScreenManager.AddScreen(new MenuBackgroundScreen());
                ScreenManager.AddScreen(new MainMenuScreen());
            }
            gameAudio.UpdateAudio();
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

            //set the positions of animations the first time Draw is called
            //have to do this here because the Viewport isnt initialized
            //when the constructor is called
            if (!initialized)
            {
                //start the drive animation just off the right edge of the screen
                driveSprite.Position = new Vector2(viewport.Width, 0);
                //determine increments to move drive and turn animations
                //320 = width of each frame, 2.5 = scale
                //42 = 21 drive frames + 21 turn frames
                moveX = (float) ((viewport.Width + 320*2.5) / (2 * 42));
                //start the turn animation after the drive animation moves 21 increments left
                turnSprite.Position = new Vector2(viewport.Width - 21 * moveX, 0);
                //start the zoom animation after the turn animation moves 21 more increments left
                zoomSprite.Position = new Vector2(viewport.Width - 42 * moveX, 0);

                initialized = true;
            }

            spriteBatch.Begin();

            if (!driveSprite.IsFinished)
                driveSprite.Draw(spriteBatch, 2.5f);
            else if (!turnSprite.IsFinished)
                turnSprite.Draw(spriteBatch, 2.5f);
            else if (!zoomSprite.IsFinished)
                zoomSprite.Draw(spriteBatch, 2.5f);

            spriteBatch.End();
        }

        #endregion
    }
}
