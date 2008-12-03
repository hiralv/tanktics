using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Tanktics
{
    class Tank : Unit
    {



        public Tank(int setTeam, int startingX, int startingY, Texture2D[] textures, int uNum)
        {
            unitNumber = uNum;
            type = "tank";

            //Team number of unit: 1-4
            team = setTeam;

            //If the unit has moved this turn
            hasMoved = false;

            //Vision range of Unit
            vision = 4;

            //Movement range of unit
            movement = 3;
            moves = new int[movement];

            //Cost in points of unit
            cost = 2;

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
            sprites[(int)Anim.Up].Animations.Add("up", new Animation(510, 555, 50, 5, 10, 0, 0));
            sprites[(int)Anim.Up].CurrentAnimation = "up";
            //add down animation
            sprites[(int)Anim.Down] = new AnimatingSprite();
            sprites[(int)Anim.Down].Texture = textures[(int)Anim.Down];
            sprites[(int)Anim.Down].Animations.Add("down", new Animation(510, 550, 50, 5, 10, 0, 0));
            sprites[(int)Anim.Down].CurrentAnimation = "down";
            //add left animation
            sprites[(int)Anim.Left] = new AnimatingSprite();
            sprites[(int)Anim.Left].Texture = textures[(int)Anim.Left];
            sprites[(int)Anim.Left].Animations.Add("left", new Animation(1340, 305, 50, 5, 10, 0, 0));
            sprites[(int)Anim.Left].CurrentAnimation = "left";
            //add right animation
            sprites[(int)Anim.Right] = new AnimatingSprite();
            sprites[(int)Anim.Right].Texture = textures[(int)Anim.Right];
            sprites[(int)Anim.Right].Animations.Add("right", new Animation(1360, 315, 50, 5, 10, 0, 0));
            sprites[(int)Anim.Right].CurrentAnimation = "right";
            //add idle up animation
            sprites[(int)Anim.IdleUp] = new AnimatingSprite();
            sprites[(int)Anim.IdleUp].Texture = textures[(int)Anim.IdleUp];
            sprites[(int)Anim.IdleUp].Animations.Add("idle up", new Animation(480, 535, 50, 5, 10, 0, 0));
            sprites[(int)Anim.IdleUp].CurrentAnimation = "idle up";
            //add idle down animation
            sprites[(int)Anim.IdleDown] = new AnimatingSprite();
            sprites[(int)Anim.IdleDown].Texture = textures[(int)Anim.IdleDown];
            sprites[(int)Anim.IdleDown].Animations.Add("idle down", new Animation(530, 535, 50, 5, 10, 0, 0));
            sprites[(int)Anim.IdleDown].CurrentAnimation = "idle down";

            currentSprite = (int)Anim.IdleDown;
        }

    }
}