#region File Description
//-----------------------------------------------------------------------------
// GDDLogoScreen.cs
//
// Display the GDD logo as a popup screen on top of the logo screen.
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
    /// The GDD logo screen enlarges the GDD logo image, then 
    /// decreases its size back to 0, then exits.
    /// </summary>
    class GDDLogoScreen : GameScreen
    {
        #region Fields
        TimeSpan elapsedTime = TimeSpan.Zero;
        bool drawGDDLogo = true, drawExplosion = false;
        TimeSpan startExplosionTime = TimeSpan.FromSeconds(4.0);
        TimeSpan stopGDDTime = TimeSpan.FromSeconds(6.0);
        GameAudio gameAudio;


        AnimatingSprite explosion;
        ContentManager content;
        Texture2D logoTexture;
        float scale = 0.0f;
        float explosionScale = 1.5f;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GDDLogoScreen()
        {
            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);
            
            explosion = new AnimatingSprite();

            Animation explo = new Animation(3990, 399, 37, 4, 10, 0, 0);
            explo.FramesPerSecond = 15;

            explosion.Animations.Add("explo", explo);
            explosion.CurrentAnimation = "explo";
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

            logoTexture = content.Load<Texture2D>("Intro/GDD2007_Transparent");
            explosion.Texture = content.Load<Texture2D>("Intro/Intro Explosion no black");

            gameAudio = ScreenManager.Audio;
            gameAudio.AddSound("Explosion");
            
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
        /// Updates the GDD logo screen. This only increments the elapsed time counter
        /// and exits the screen after a set amount of display time.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            
            elapsedTime += gameTime.ElapsedGameTime;
            
            //explosion has not started yet
            if (!drawExplosion)
            {
                //enlarge scale up to 0.67
                if (scale <= 0.67f)
                    scale += 0.01f;

                //start explosion after set amount of time
                if (elapsedTime >= startExplosionTime)
                {
                    drawExplosion = true;
                    explosion.StartAnimation();
                    gameAudio.PlaySound("Explosion");
                }
            }
            //explosion animation is active
            else
            {
                //stop drawing GDD logo after set amount of time
                if (elapsedTime > stopGDDTime)
                    drawGDDLogo = false;

                explosion.Update(gameTime);

                //load tank intro screen after explosion finishes
                if (explosion.IsFinished)
                    LoadingScreen.Load(ScreenManager, false, new TankIntroScreen());
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
        /// Draws the GDD logo and/or explosion.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            //set explosion position if it has not been set yet
            if (explosion.Position == Vector2.Zero)
                explosion.Position = new Vector2((float) ((viewport.Width - 399*explosionScale) / 2), (float) ((viewport.Height - 399*explosionScale) / 2));

            spriteBatch.Begin();
            
            if (drawGDDLogo)
            {
                int spriteW = (int)(scale * logoTexture.Width);
                int spriteH = (int)(scale * logoTexture.Height);

                spriteBatch.Draw(logoTexture,
                    new Vector2((viewport.Width - spriteW) / 2, (viewport.Height - spriteH) / 2), //centered
                    null,
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0.0f
                    );
            }
            
            if (drawExplosion)
            {
                explosion.Draw(spriteBatch, explosionScale);
            }
            spriteBatch.End();
        }

        #endregion
    }
}
