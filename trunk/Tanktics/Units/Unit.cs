using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Tanktics
{
    public class Unit
    {
        public enum Anim { Up, Down, Left, Right, IdleUp, IdleDown };

        public Boolean hasMoved;
        public int vision;
        public int movement;
        public int team;
        public int cost;
        public String type;
        public int unitNumber;
        //map position
        public int currentX;
        public int currentY;
        public int previousX;
        public int previousY;

        //animation variables
        public AnimatingSprite[] sprites;
        public int currentSprite;
        public Rectangle currentSpriteRect;

        //variables for animating moves
        public Boolean isMoving = false;
        public int[] moves;
        public int currentDirection = 0;
        public int numMoves;
        //offset during movements (percentage of tile width/height)
        public float offsetX = 0f;
        public float offsetY = 0f;


        public void sethasMoved(Boolean toSet)
        {
            hasMoved = toSet;
        }

        //Sets prevX and prevY to currX and CurrY
        //Used at end of turn
        public void updatePreXandY()
        {
            previousX = currentX;
            previousY = currentY;
            hasMoved = false;
        }

        //Robby Florence
        public void Update(GameTime gameTime, UnitController control)
        {
            if (isMoving)
            {
                //move offset
                if (moves[currentDirection] == (int)Anim.Up)
                {
                    currentSprite = (int)Anim.Up;
                    currentSpriteRect = sprites[currentSprite].Animations["up"].CurrentFrame;
                    offsetY -= 0.01f;
                }
                else if (moves[currentDirection] == (int)Anim.Down)
                {
                    currentSprite = (int)Anim.Down;
                    currentSpriteRect = sprites[currentSprite].Animations["down"].CurrentFrame;
                    offsetY += 0.01f;
                }
                else if (moves[currentDirection] == (int)Anim.Left)
                {
                    currentSprite = (int)Anim.Left;
                    currentSpriteRect = sprites[currentSprite].Animations["left"].CurrentFrame;
                    offsetX -= 0.01f;
                }
                else if (moves[currentDirection] == (int)Anim.Right)
                {
                    currentSprite = (int)Anim.Right;
                    currentSpriteRect = sprites[currentSprite].Animations["right"].CurrentFrame;
                    offsetX += 0.01f;
                }

                //set new unit location when offset has reached a new tile
                if (offsetX >= 1f)
                {
                    currentDirection++;
                    currentX++;
                    control.setCurrentBoard(new NullUnit(), team, currentX-1, currentY);
                    control.setCurrentBoard(this, team, currentX, currentY);
                    offsetX = 0f;
                }
                else if (offsetX <= -1f)
                {
                    currentDirection++;
                    currentX--;
                    control.setCurrentBoard(new NullUnit(), team, currentX+1, currentY);
                    control.setCurrentBoard(this, team, currentX, currentY);
                    offsetX = 0f;
                }
                if (offsetY >= 1f)
                {
                    currentDirection++;
                    currentY++;
                    control.setCurrentBoard(new NullUnit(), team, currentX, currentY-1);
                    control.setCurrentBoard(this, team, currentX, currentY);
                    offsetY = 0f;
                }
                else if (offsetY <= -1f)
                {
                    currentDirection++;
                    currentY--;
                    control.setCurrentBoard(new NullUnit(), team, currentX, currentY+1);
                    control.setCurrentBoard(this, team, currentX, currentY);
                    offsetY = 0f;
                }

                //stop moving
                if (currentDirection >= numMoves)
                {
                    if (moves[currentDirection - 1] == (int)Anim.Up)
                    {
                        currentSprite = (int)Anim.IdleUp;
                        currentSpriteRect = sprites[currentSprite].Animations["idle up"].CurrentFrame;
                    }
                    else if (moves[currentDirection - 1] == (int)Anim.Down)
                    {
                        currentSprite = (int)Anim.IdleDown;
                        currentSpriteRect = sprites[currentSprite].Animations["idle down"].CurrentFrame;
                    }

                    currentDirection = 0;
                    isMoving = false;
                }
            }

            sprites[currentSprite].Update(gameTime);
        }

        //Robby Florence
        public void Draw(SpriteBatch batch, Rectangle destination, Color fade)
        {
            //offset destination while moving
            destination.X += (int)(offsetX * destination.Width);
            destination.Y += (int)(offsetY * destination.Height);

            //calculate scale to make the sprite fit in destination
            float scale = Math.Min(
                (float)destination.Width / currentSpriteRect.Width,
                (float)destination.Height / currentSpriteRect.Height);

            //make apcs smaller since they have no gun barrels
            if (type.Equals("apc"))
                scale *= 0.75f;

            //use scale to get new destination rectangle
            //centered in original destination
            Rectangle spriteDest = new Rectangle(
                0, 0,
                (int)(scale * currentSpriteRect.Width),
                (int)(scale * currentSpriteRect.Height));
            spriteDest.X = destination.X + destination.Width / 2 - spriteDest.Width / 2;
            spriteDest.Y = destination.Y + destination.Height / 2 - spriteDest.Height / 2;

            sprites[currentSprite].Draw(batch, spriteDest, fade);
        }
    }
}
