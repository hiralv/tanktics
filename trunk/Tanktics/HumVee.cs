using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Tanktics
{
    class HumVee : Unit
    {



        public HumVee(int setTeam, int startingX, int startingY, Texture2D SpriteSheet, int uNum)
        {
            unitNumber = uNum;
            type = "humvee";

            //Team number of unit: 1-4
            team = setTeam;

            //If the unit has moved this turn
            hasMoved = false;

            //Vision range of Unit
            vision = 5;

            //If unit can enter water
            canWater = true;

            //Movement range of unit
            movement = 2;

            //Cost in points of unit
            cost = 1;

            //Current Position on board
            currentX = startingX;
            currentY = startingY;

            //Previous Position on board this turn
            //Reset at finalize
            previousX = startingX;
            previousY = startingY;

            sprite = new AnimatingSprite();
            Animation leftAnim;
            Animation rightAnim;
            sprite.Texture = SpriteSheet;

            if (team == 1)
            {
                leftAnim = new Animation(510, 110, 6, 0, 0);
                rightAnim = new Animation(510, 110, 6, 0, 0);
            }
            else if (team == 2)
            {
                leftAnim = new Animation(510, 110, 6, 0, 0);
                rightAnim = new Animation(510, 110, 6, 0, 0);
            }
            else if (team == 3)
            {
                leftAnim = new Animation(510, 110, 6, 0, 0);
                rightAnim = new Animation(510, 110, 6, 0, 0);
            }
            else
            {
                leftAnim = new Animation(510, 110, 6, 0, 0);
                rightAnim = new Animation(510, 110, 6, 0, 0);
            }
            sprite.Animations.Add("Left", leftAnim);
            sprite.Animations.Add("Right", rightAnim);
        }

    }
}