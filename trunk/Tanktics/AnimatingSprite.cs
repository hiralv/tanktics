using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;



    /// <summary>
    /// Our general purpose Animation set. Contains an array
    /// of source Rectangles for our AnimatedSprite to use when
    /// rendering. Also handles updating itself based on a given
    /// frame rate.
    /// </summary>
    public class Animation
    {
        //the frames of our animation
        Rectangle[] frames;

        //how long each frame lasts (in seconds)
        float frameLength = 1f / 10f;

        //a timer to use for tracking frame time
        float timer = 0f;

        //the current frame
        int currentFrame = 0;

        //if the animation has completed one loop
        bool isFinished = false;

        /// <summary>
        /// Gets the current frame as a source Rectangle
        /// </summary>
        public Rectangle CurrentFrame
        {
            get { return frames[currentFrame]; }
        }

        public int CurrentFrameNum
        {
            get { return currentFrame; }
        }

        /// <summary>
        /// Gets or sets the animation speed
        /// </summary>
        public int FramesPerSecond
        {
            //we need to convert from our floating point frameLength
            //variable to an integer number that represents frames per
            //second and vice versa.
            get { return (int)(1f / frameLength); }
            set { frameLength = 1f / (float)value; }
        }

        public bool IsFinished
        {
            get { return isFinished; }
        }

        /// <summary>
        /// Creates a new Animation with sprites in a single row
        /// </summary>
        /// <param name="width">The total width of the animation</param>
        /// <param name="frameHeight">The height of each frame in the animation</param>
        /// <param name="numFrames">The number of frames in the animation</param>
        /// <param name="xOffset">The offset along the X axis</param>
        /// <param name="yOffset">The offset along the Y axis</param>
        public Animation(int width, int height, int numFrames, int xOffset, int yOffset)
        {
            //create our frames array
            frames = new Rectangle[numFrames];

            //determine the width of a single frame
            int frameWidth = width / numFrames;

            //create the frame Rectangles
            for (int i = 0; i < numFrames; i++)
            {
                //each Rectangle is created in a row. we use the xOffset as the starting point for
                //the Rectangle's X parameter and add to it for each frame. Since each frame is in
                //a horizontal line, the yOffset is used for the Y parameter. Each frame is the same
                //width and height so we use frameWidth and height for the Rectangle's width and height
                //parameters, respectively.
                frames[i] = new Rectangle(xOffset + (frameWidth * i), yOffset, frameWidth, height);
            }
        }

        /// <summary>
        /// Creates a new Animation with sprites in multiple rows
        /// </summary>
        /// <param name="width">The total width of the animation</param>
        /// <param name="height">The total height of the animation</param>
        /// <param name="numFrames">The total number of frames in the animation</param>
        /// <param name="rows">The number of rows in the animation</param>
        /// <param name="cols">The number of columns in the animation</param>
        /// <param name="xOffset">The offset along the X axis</param>
        /// <param name="yOffset">The offset along the Y axis</param>
        public Animation(int width, int height, int numFrames, int rows, int cols, int xOffset, int yOffset)
        {
            int count = 0;

            //create our frames array
            frames = new Rectangle[numFrames];

            //determine the width and height of a single frame
            int frameWidth = width / cols;
            int frameHeight = height / rows;

            //create the frame Rectangles
            //loop through rows
            for (int i = 0; i < rows; i++)
            {
                //loop through columns
                for (int j = 0; j < cols && count < numFrames; j++)
                {
                    frames[count] = new Rectangle(xOffset + (frameWidth * j), yOffset + (frameHeight * i), frameWidth, frameHeight);
                    count++;
                }
            }
        }

        /// <summary>
        /// Updates the animation
        /// </summary>
        /// <param name="gameTime">The current gameTime</param>
        public void Update(GameTime gameTime)
        {
            //add to our timer based on the time past
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //if the timer has past the frameLength...
            if (timer >= frameLength)
            {
                //reset the timer to 0
                timer = 0f;

                //set finished variable if at the last frame
                if (currentFrame == frames.Length - 1)
                    isFinished = true;

                //increment the currentFrame. we use the mod function (%) to make sure
                //our currentFrame stays within the bounds of the frames array.
                currentFrame = (currentFrame + 1) % frames.Length;
            }
        }

        /// <summary>
        /// Resets the currentFrame and timer to 0
        /// </summary>
        public void Reset()
        {
            //simply resetting the animation back to 0.
            currentFrame = 0;
            timer = 0f;
        }
    }

    /// <summary>
    /// A basic class for creating an animated sprite. The Sprite
    /// maintains its own position as well as a Dictionary of animations
    /// it can use.
    /// </summary>
    public class AnimatingSprite
    {
        /// <summary>
        /// The current position of the sprite
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// A set of Animations the sprite can use
        /// </summary>
        public Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();

        //the sprite's texture sheet
        Texture2D texture;

        //the current animation
        string currentAnimation;

        //whether or not to Update the animation
        bool updateAnimation = true;

        /// <summary>
        /// Gets or sets the sprite's texture sheet
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        /// <summary>
        /// Gets or sets the current animation of the sprite
        /// </summary>
        public string CurrentAnimation
        {
            //get is easy. just return the value.
            get { return currentAnimation; }

            //set is a little more advanced...
            set
            {
                //if the value passed in is not a key in our dictionary, we can't use it
                //so we will throw an exception.
                if (!Animations.ContainsKey(value))
                    throw new Exception("Invalid animation specified.");

                //we only want to change animations if the currentAnimation is unset (null)
                //or if the new value is different than the currentAnimation.
                if (currentAnimation == null || !currentAnimation.Equals(value))
                {
                    //set the current animation
                    currentAnimation = value;

                    //reset the animation
                    Animations[currentAnimation].Reset();
                }
            }
        }

        public bool IsFinished
        {
            get { return Animations[currentAnimation].IsFinished; }
        }

        /// <summary>
        /// Starts animating the sprite if not already animating.
        /// </summary>
        public void StartAnimation()
        {
            updateAnimation = true;
        }

        /// <summary>
        /// Stops animating the sprite.
        /// </summary>
        public void StopAnimation()
        {
            updateAnimation = false;
        }

        /// <summary>
        /// Updates the sprite
        /// </summary>
        /// <param name="gameTime">The current gameTime</param>
        /// 

        public void Update(GameTime gameTime)
        {
            //here we have to make sure we have a valid animation to use if
            //nobody has yet set the currentAnimation.
            if (currentAnimation == null)
            {
                //if there are no keys, we don't have any animations so we just quit
                if (Animations.Keys.Count == 0)
                    return;

                //get a list of the valid Keys in the Animations dictionary.
                string[] keys = new string[Animations.Keys.Count];

                //copy the keys out of the dictionary to the array
                Animations.Keys.CopyTo(keys, 0);

                //set the currentAnimation to the first animation
                currentAnimation = keys[0];
            }

            //if we are updating the animation, call its Update method
            if (updateAnimation)
                Animations[currentAnimation].Update(gameTime);
        }

        /// <summary>
        /// Draws the sprite with the given SpriteBatch
        /// </summary>
        /// <param name="batch">The SpriteBatch with which to draw the sprite</param>
        public void Draw(SpriteBatch batch)
        {
            //simple draw call. we use our Animations[currentAnimation].CurrentFrame to
            //get the source rectangle to use for picking our sprite.
            batch.Draw(
                texture,
                Position,
                Animations[currentAnimation].CurrentFrame,
                Color.White);
        }

        /// <summary>
        /// Draws the sprite with the given SpriteBatch and scale
        /// </summary>
        /// <param name="batch">The SpriteBatch with which to draw the sprite</param>
        public void Draw(SpriteBatch batch, float scale)
        {
            //simple draw call. we use our Animations[currentAnimation].CurrentFrame to
            //get the source rectangle to use for picking our sprite.
            batch.Draw(
                texture,
                Position,
                Animations[currentAnimation].CurrentFrame,
                Color.White,
                0.0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0.0f);
        }

        /// <summary>
        /// Draws the sprite with the given SpriteBatch and destination Rectangle
        /// </summary>
        /// <param name="batch">The SpriteBatch with which to draw the sprite</param>
        public void Draw(SpriteBatch batch, Rectangle destination)
        {
            //simple draw call. we use our Animations[currentAnimation].CurrentFrame to
            //get the source rectangle to use for picking our sprite.
            batch.Draw(
                texture,
                destination,
                Animations[currentAnimation].CurrentFrame,
                Color.White);
        }
    }
