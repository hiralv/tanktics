using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Tanktics
{
    class APC : Unit
    {
        public APC(int setTeam, int startingX, int startingY, Texture2D[] textures, int uNum)
        {
            unitNumber = uNum;
            type = "apc";

            //Team number of unit: 1-4
            team = setTeam;

            //If the unit has moved this turn
            hasMoved = false;

            //Vision range of Unit
            vision = 5;

            //Movement range of unit
            movement = 2;
            moves = new int[movement];

            //Cost in points of unit
            cost = 1;

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
            sprites[(int)Anim.Up].Animations.Add("up", new Animation(710, 575, 50, 5, 10, 0, 0));
            sprites[(int)Anim.Up].CurrentAnimation = "up";
            //add down animation
            sprites[(int)Anim.Down] = new AnimatingSprite();
            sprites[(int)Anim.Down].Texture = textures[(int)Anim.Down];
            sprites[(int)Anim.Down].Animations.Add("down", new Animation(710, 575, 50, 5, 10, 0, 0));
            sprites[(int)Anim.Down].CurrentAnimation = "down";
            //add left animation
            sprites[(int)Anim.Left] = new AnimatingSprite();
            sprites[(int)Anim.Left].Texture = textures[(int)Anim.Left];
            sprites[(int)Anim.Left].Animations.Add("left", new Animation(1490, 410, 50, 5, 10, 0, 0));
            sprites[(int)Anim.Left].CurrentAnimation = "left";
            //add right animation
            sprites[(int)Anim.Right] = new AnimatingSprite();
            sprites[(int)Anim.Right].Texture = textures[(int)Anim.Right];
            sprites[(int)Anim.Right].Animations.Add("right", new Animation(1490, 410, 50, 5, 10, 0, 0));
            sprites[(int)Anim.Right].CurrentAnimation = "right";
            //add idle up animation
            sprites[(int)Anim.IdleUp] = new AnimatingSprite();
            sprites[(int)Anim.IdleUp].Texture = textures[(int)Anim.IdleUp];
            sprites[(int)Anim.IdleUp].Animations.Add("idle up", new Animation(710, 575, 50, 5, 10, 0, 0));
            sprites[(int)Anim.IdleUp].CurrentAnimation = "idle up";
            //add idle down animation
            sprites[(int)Anim.IdleDown] = new AnimatingSprite();
            sprites[(int)Anim.IdleDown].Texture = textures[(int)Anim.IdleDown];
            sprites[(int)Anim.IdleDown].Animations.Add("idle down", new Animation(710, 575, 50, 5, 10, 0, 0));
            sprites[(int)Anim.IdleDown].CurrentAnimation = "idle down";
            //add rotate animation
            sprites[(int)Anim.Rotate] = new AnimatingSprite();
            sprites[(int)Anim.Rotate].Texture = textures[(int)Anim.Rotate];
            sprites[(int)Anim.Rotate].Animations.Add("rotate", new Animation(1410, 380, 50, 5, 10, 0, 0));
            sprites[(int)Anim.Rotate].CurrentAnimation = "rotate";

            currentSprite = (int)Anim.IdleDown;
            currentSpriteRect = sprites[currentSprite].Animations["idle down"].CurrentFrame;

            #region Map Values
            switch (setTeam)
            {
                case 1:
                    typeno = 20;
                    AI.map[startingX, startingY] = 20;
                    break;

                case 2:
                    typeno = 50;
                    AI.map[startingX, startingY] = 50;
                    break;

                case 3:
                    typeno = 80;
                    AI.map[startingX, startingY] = 80;
                    break;

                case 4:
                    typeno = 110;
                    AI.map[startingX, startingY] = 110;
                    break;
            }
            #endregion
        }

        public override List<moves> GetAllpossibleMoves()
        {
            List<moves> possiblemoves = new List<moves>();


            #region Move List 1
            for (int i = 1; i <= 2; i++)
            {
                if (currentX + i < 25)
                {
                    possiblemoves.Add(new moves(currentX + i, currentY));
                }

                if (currentY + i < 25)
                {
                    possiblemoves.Add(new moves(currentX, currentY + i));
                }

                if (currentX - i > -1)
                {
                    possiblemoves.Add(new moves(currentX - i, currentY));
                }

                if (currentY - i > -1)
                {
                    possiblemoves.Add(new moves(currentX, currentY - i));
                }
            } 
            #endregion

            #region Move List 2
            if (currentX + 1 < 25)
            {
                if (currentY + 1 < 25)
                {
                    possiblemoves.Add(new moves(currentX + 1, currentY + 1));
                }

                if (currentY - 1 > -1)
                {
                    possiblemoves.Add(new moves(currentX + 1, currentY - 1));
                }
            }


            if (currentX - 1 > -1)
            {
                if (currentY - 1 > -1)
                {
                    possiblemoves.Add(new moves(currentX - 1, currentY - 1));
                }

                if (currentY + 1 < 25)
                {
                    possiblemoves.Add(new moves(currentX - 1, currentY + 1));
                }
            }
            
            #endregion

            #region Move team places
            for (int i = 0; i < possiblemoves.Count; i++)
            {
                moves move = possiblemoves[i];

                switch (team)
                {
                    case 1:
                        if (AI.map[move.x,move.y] >= 10 && AI.map[move.x, move.y] <= 30)
                            possiblemoves.RemoveAt(i);
                        break;

                    case 2:
                        if (AI.map[move.x, move.y] >= 40 && AI.map[move.x, move.y] <= 60)
                            possiblemoves.RemoveAt(i);
                        break;

                    case 3:
                        if (AI.map[move.x, move.y] >= 70 && AI.map[move.x, move.y] <= 90)
                            possiblemoves.RemoveAt(i);
                        break;

                    case 4:
                        if (AI.map[move.x, move.y] >= 100 && AI.map[move.x, move.y] <= 120)
                            possiblemoves.RemoveAt(i);
                        break;
                }
            } 
            #endregion

            return possiblemoves;
        }
    }
}