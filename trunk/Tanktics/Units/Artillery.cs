using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Tanktics
{
    class Artillery : Unit
    {



        public Artillery(int setTeam, int startingX, int startingY, Texture2D[] textures, int uNum)
        {
            unitNumber = uNum;

            type = "artillery";

            //Team number of unit: 1-4
            team = setTeam;

            //If the unit has moved this turn
            hasMoved = false;

            //Vision range of Unit
            vision = 4;

            //If unit can enter water
            canWater = false;

            //Movement range of unit
            movement = 3;

            //Cost in points of unit
            cost = 3;

            //Current Position on board
            currentX = startingX;
            currentY = startingY;

            //Previous Position on board this turn
            //Reset at finalize
            previousX = startingX;
            previousY = startingY;

            sprites = new AnimatingSprite[textures.Length];

            //add up animation
            sprites[(int)Anim.Up] = new AnimatingSprite();
            sprites[(int)Anim.Up].Texture = textures[(int)Anim.Up];
            sprites[(int)Anim.Up].Animations.Add("up", new Animation(1910, 1995, 50, 5, 10, 0, 0));
            //add down animation
            sprites[(int)Anim.Down] = new AnimatingSprite();
            sprites[(int)Anim.Down].Texture = textures[(int)Anim.Down];
            sprites[(int)Anim.Down].Animations.Add("down", new Animation(1890, 2625, 50, 5, 10, 0, 0));
            //add left animation
            sprites[(int)Anim.Left] = new AnimatingSprite();
            sprites[(int)Anim.Left].Texture = textures[(int)Anim.Left];
            sprites[(int)Anim.Left].Animations.Add("left", new Animation(2714, 800, 50, 5, 10, 0, 0));
            //add right animation
            sprites[(int)Anim.Right] = new AnimatingSprite();
            sprites[(int)Anim.Right].Texture = textures[(int)Anim.Right];
            sprites[(int)Anim.Right].Animations.Add("right", new Animation(2306, 800, 50, 5, 10, 0, 0));
            //add idle up animation
            sprites[(int)Anim.IdleUp] = new AnimatingSprite();
            sprites[(int)Anim.IdleUp].Texture = textures[(int)Anim.IdleUp];
            sprites[(int)Anim.IdleUp].Animations.Add("idle up", new Animation(2160, 2050, 50, 5, 10, 0, 0));
            //add idle down animation
            sprites[(int)Anim.IdleDown] = new AnimatingSprite();
            sprites[(int)Anim.IdleDown].Texture = textures[(int)Anim.IdleDown];
            sprites[(int)Anim.IdleDown].Animations.Add("idle down", new Animation(2750, 2625, 50, 5, 10, 0, 0));

            currentSprite = (int)Anim.Up;
        }

    }
}