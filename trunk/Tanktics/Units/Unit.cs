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
        public int currentX;
        public int currentY;
        public int previousX;
        public int previousY;
        public AnimatingSprite[] sprites;
        public int currentSprite;
        public String type;
        public int unitNumber;

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

        public void Update(GameTime gameTime, UnitController control)
        {
            if (isMoving)
            {
                //move offset
                if (moves[currentDirection] == (int)Anim.Up)
                {
                    currentSprite = (int)Anim.Up;
                    offsetY -= 0.01f;
                }
                else if (moves[currentDirection] == (int)Anim.Down)
                {
                    currentSprite = (int)Anim.Down;
                    offsetY += 0.01f;
                }
                else if (moves[currentDirection] == (int)Anim.Left)
                {
                    currentSprite = (int)Anim.Left;
                    offsetX -= 0.01f;
                }
                else if (moves[currentDirection] == (int)Anim.Right)
                {
                    currentSprite = (int)Anim.Right;
                    offsetX += 0.01f;
                }

                //set new unit location when offset has reached a new tile
                if (offsetX >= 1f)
                {
                    currentDirection++;
                    control.setCurrentBoard(new NullUnit(), currentX, currentY);
                    currentX++;
                    control.setCurrentBoard(this, currentX, currentY);
                    offsetX = 0f;
                }
                else if (offsetX <= -1f)
                {
                    currentDirection++;
                    control.setCurrentBoard(new NullUnit(), currentX, currentY);
                    currentX--;
                    control.setCurrentBoard(this, currentX, currentY);
                    offsetX = 0f;
                }
                if (offsetY >= 1f)
                {
                    currentDirection++;
                    control.setCurrentBoard(new NullUnit(), currentX, currentY);
                    currentY++;
                    control.setCurrentBoard(this, currentX, currentY);
                    offsetY = 0f;
                }
                else if (offsetY <= -1f)
                {
                    currentDirection++;
                    control.setCurrentBoard(new NullUnit(), currentX, currentY);
                    currentY--;
                    control.setCurrentBoard(this, currentX, currentY);
                    offsetY = 0f;
                }

                //stop moving
                if (currentDirection >= numMoves)
                {
                    if (moves[currentDirection - 1] == (int)Anim.Up)
                        currentSprite = (int)Anim.IdleUp;
                    else if (moves[currentDirection - 1] == (int)Anim.Down)
                        currentSprite = (int)Anim.IdleDown;

                    currentDirection = 0;
                    isMoving = false;
                }
            }

            sprites[currentSprite].Update(gameTime);
        }

        public void Draw(SpriteBatch batch, Rectangle destination)
        {
            destination.X += (int)(offsetX * destination.Width);
            destination.Y += (int)(offsetY * destination.Height);
            sprites[currentSprite].Draw(batch, destination);
        }
    }
}
