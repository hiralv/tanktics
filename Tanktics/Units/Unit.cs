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
        public Boolean canWater;
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

        public void Update(GameTime gameTime)
        {
            sprites[currentSprite].Update(gameTime);
        }

        public void Draw(SpriteBatch batch, Rectangle destination)
        {
            sprites[currentSprite].Draw(batch, destination);
        }
    }
}
