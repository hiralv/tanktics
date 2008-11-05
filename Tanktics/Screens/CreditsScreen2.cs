#region File Description
//-----------------------------------------------------------------------------
// CreditsScreen.cs
//
// Credits Screen for Tanktics.
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
    /// The credits screen is brought up over the top of the main menu
    /// screen, and provides the user a detailed list of contributors and
    /// sources.
    /// </summary>
    class CreditsScreen : GameScreen
    {
        ContentManager content;
        SpriteFont gameFont;
        SpriteBatch batch;
        AnimatingSprite sprite;
        AnimatingSprite sprite2;

        Vector2 title = new Vector2(200, 200);
        Vector2 Logo = new Vector2(320, 100);
        


        //Starting Positions for names and infintry
        int goodstart1 = 850;
        int goodstart2 = 300;


        Vector2 Name = new Vector2(850+100, 300);

        String currentJob = "Executive Producer";

        String[] Job = new String[13];

        String personsName = "  Dr  Tiffany Barnes  ";

        String[] nameArray = new String[13];

        int counter = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreditsScreen()
        {

            Job[0] = "Executive  Producer";
            Job[1] = " Code Monkey Orange";
            Job[2] = " Code Monkey  Green";
            Job[3] = " Code Monkey Yellow";
            Job[4] = " Code Monkey Blue  ";
            Job[5] = " Original  Sprites ";
            Job[6] = "   Sprite Ripper   ";
            Job[7] = "   Ripper's Site   ";
            Job[8] = "  3D Tank Modeler  ";
            Job[9] = "    Explosions     ";
            Job[10] ="     Tank SFX      ";
            Job[11] ="Giant  Sound  Pack ";
            Job[12] ="Unedited Main Theme";



            nameArray[0] = "  Dr  Tiffany Barnes  ";
            nameArray[1] = "      Acey  Boyce     ";
            nameArray[2] = "    Robby Florence    ";
            nameArray[3] = "      Hiral Vora      ";
            nameArray[4] = "  Christopher  Wykel  ";
            nameArray[5] = "     Advance Wars     ";
            nameArray[6] = "       Rogulgot       ";
            nameArray[7] = "The Spriter's Resource";
            nameArray[8] = "    Gunpoint-3d.com   ";
            nameArray[9] = "     ExploGen2.0      ";
            nameArray[10] ="      Radio Mall      ";
            nameArray[11] ="      Fede-lasse      ";
            nameArray[12] ="Jack Wall & Sam Hulick";

            sprite = new AnimatingSprite();
            sprite2 = new AnimatingSprite();

            Animation left = new Animation(510, 110, 6, 0, 0);
           
            sprite.Animations.Add("Left", left);
            sprite2.Animations.Add("Left", left);
           
            sprite.Position = new Vector2(goodstart1,goodstart2);
            sprite2.Position = new Vector2(goodstart1+630, goodstart2);
            sprite.CurrentAnimation = "Left";
            sprite2.CurrentAnimation = "Left";
        }
        
        /// <summary>
        /// Load graphics content for the credits.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("gamefont");
            batch = ScreenManager.SpriteBatch;
            sprite.Texture = content.Load<Texture2D>("infintry4");
            sprite2.Texture = content.Load<Texture2D>("infintry4");

            Thread.Sleep(1000);
            ScreenManager.Game.ResetElapsedTime();
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        //Marches the troops and scrolls the Credits

         public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            sprite.Position.X--;
            sprite2.Position.X--;
            sprite.Position.X--;
            sprite2.Position.X--;
            Name.X--;
            Name.X--;
            if (sprite2.Position.X <= -85)
            {
                if (counter < nameArray.Length-1)
                {
                    sprite.Position.X = goodstart1;
                    sprite2.Position.X = goodstart1+630;
                    Name.X = goodstart1 + 100;
                    counter++;
                    currentJob = Job[counter];
                    personsName = nameArray[counter];
                    if (counter%4 == 1)
                    {
                        sprite2.Texture = content.Load<Texture2D>("infintry");
                        sprite.Texture = content.Load<Texture2D>("infintry");
                    }
                    if (counter % 4 == 2)
                    {
                        sprite2.Texture = content.Load<Texture2D>("infintry2");
                        sprite.Texture = content.Load<Texture2D>("infintry2");
                    }
                    if (counter % 4 == 3)
                    {
                        sprite2.Texture = content.Load<Texture2D>("infintry3");
                        sprite.Texture = content.Load<Texture2D>("infintry3");
                    }
                    if (counter % 4 == 0)
                    {
                        sprite2.Texture = content.Load<Texture2D>("infintry4");
                        sprite.Texture = content.Load<Texture2D>("infintry4");
                    }
                }
                else
                {
                    LoadingScreen.Load(ScreenManager, false, new MenuBackgroundScreen(),
                                                    new MainMenuScreen());
                }
            }
            sprite.Update(gameTime);


            
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.PauseGame)
            {
                //If pause is hit return to main menu
                LoadingScreen.Load(ScreenManager, false, new MenuBackgroundScreen(),
                                                     new MainMenuScreen());
            }
            GamePadState gps = GamePad.GetState(PlayerIndex.One);
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Enter))
            {
                //If enter is hit return to main menu
                LoadingScreen.Load(ScreenManager, false, new MenuBackgroundScreen(),
                                                     new MainMenuScreen());
            }
            if (gps.Buttons.A == ButtonState.Pressed)
            {
                //If A is hit return to main menu
                LoadingScreen.Load(ScreenManager, false, new MenuBackgroundScreen(),
                                                     new MainMenuScreen());
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.White, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.DrawString(gameFont, currentJob, title, Color.DarkRed);

            spriteBatch.DrawString(gameFont, "TANKTICS!",
                                   Logo, Color.DarkRed);

            spriteBatch.DrawString(gameFont, personsName, Name, Color.DarkRed);
            sprite.Draw(batch);
            sprite2.Draw(batch);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }
    }
}