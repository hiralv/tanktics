using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace Tanktics
{

    //HIRAL VORA
    public class moves
    {
        public int x;
        public int y;

        public moves()
        {

        }

        public moves(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
   
    class AI
    {
        UnitController unitControl;
        public static Unit currentmovingunit = new Unit();

        #region Values
        static public int[,] map;
        static public int[,] values = new int[17, 17] {  
                            { 1,  2,  3, 10, 15, 20, 25, 30, 35, 30, 25, 20, 15, 10,  3,  2,  1},
                            { 2,  2,  3, 10, 15, 20, -4, -4, 40, -4, -4, 20, 15, 10,  3,  2,  2},
                            { 3,  3,  3, 10, 15, 20, -4, 45, 45, 45, -4, 20, 15, 10,  3,  3,  3},
                            {10, 10, 10, 10, 15, 20, -4, 50, 60, 50, -4, 20, 15, 10, 10, 10, 10},
                            {15, 15, 15, 15, 20, 25, 30, 45, 45, 45, 30, 25, 20, 15, 15, 15, 15},
                            {20, 20, 20, 25, 30, 35, 40, 45, 50, 45, 40, 35, 30, 25, 20, 20, 20},
                            {25, -4, -4, -4, 35, 39, -4, 50, 55, 50, -4, 39, 35, -4, -4, -4, 25},
                            {30, -4, 55, 50, 40, 44, -4, 55, 60, 55, -4, 44, 40, 50, 55, -4, 30},
                            {35, 40, 60, 50, 45, 49, -4, 60, 65, 60, -4, 49, 45, 50, 60, 40, 35},
                            {30, -4, 55, 50, 40, 44, -4, 55, 55, 55, -4, 44, 40, 50, 55, -4, 20},
                            {25, -4, -4, -4, 35, 39, -4, 50, 50, 50, -4, 39, 35, -4, -4, -4, 25},
                            {20, 20, 20, 25, 30, 35, 40, 45, 50, 45, 40, 35, 30, 25, 20, 20, 20},
                            {15, 15, 15, 15, 20, 25, 30, 45, 45, 45, 30, 25, 20, 15, 15, 15, 15},
                            {10, 10, 10, 10, 15, 20, -4, 50, 60, 50, -4, 20, 15, 10, 10, 10, 10},
                            { 3,  3,  3, 10, 15, 20, -4, 45, 45, 45, -4, 20, 15, 10,  3,  3,  3},
                            { 2,  2,  3, 10, 15, 20, -4, -4, 40, -4, -4, 20, 15, 10,  3,  2,  2},
                            { 1,  2,  3, 10, 15, 20, 25, 30, 35, 30, 25, 20, 15, 10,  3,  2,  1}};
        #endregion
        
        public AI(UnitController unitControl)
        {
            this.unitControl = unitControl;
            
            map = new int[17, 17];

            for (int i = 0; i < 17; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    map[i, j] = 0;
                }
            }
        }

        public int NextMove()
        {
            Unit mostouter = unitControl.currentUnit;
            Unit firstunit = unitControl.currentUnit;
            unitControl.nextUnit();
            Unit nextunit = unitControl.currentUnit;

            if (mostouter.hasMoved)
            {
                return 0;
            }

            while (nextunit != firstunit)
            {
                if (AI.values[nextunit.currentX, nextunit.currentY] > AI.values[mostouter.currentX, mostouter.currentY])
                    mostouter = nextunit;

                unitControl.nextUnit();
                nextunit = unitControl.currentUnit;
            }

            while (unitControl.currentUnit != mostouter)
                unitControl.nextUnit();

            int previousx, previousy, result;
            previousx = unitControl.currentUnit.currentX;
            previousy = unitControl.currentUnit.currentY;

            List<moves> possiblemoves = unitControl.currentUnit.GetAllpossibleMoves();
            moves move = FindBestPossibleMove(possiblemoves);

            result = unitControl.moveUnit(move.x, move.y);

            if (result == 3)
            {
                while (result != 1)
                {
                    possiblemoves.Remove(move);
                    if (possiblemoves.Count > 0)
                    {
                        move = FindBestPossibleMove(possiblemoves);
                        result = unitControl.moveUnit(move.x, move.y);
                    }
                    else
                        break;
                }
            }

            if (result == 1)
            {
                //AI update map
                AI.map[mostouter.currentX, mostouter.currentY] = 0;
                AI.map[move.x, move.y] = mostouter.typeno;
                AI.currentmovingunit = mostouter;
            }
            else if (result == 3)
            {
                mostouter.sethasMoved(true);
            }

            return 1;
        }

        private moves FindBestPossibleMove(List<moves> possiblemoves)
        {
            
            Random ran = new Random();
            moves move = KillEnemy(possiblemoves);

            if (move != null)
            {
                return move;
            }
            else
                move = new moves();

            int ret = ran.Next(possiblemoves.Count);
            move = possiblemoves[ret];
            int max = values[move.x, move.y];

            for (int i = 0; i < possiblemoves.Count; i++)
            {
                if (values[possiblemoves[i].x, possiblemoves[i].y] > max)
                {
                    max = values[possiblemoves[i].x, possiblemoves[i].y];
                    move = possiblemoves[i];
                }
            }
            
            return move;
        }

        private moves KillEnemy(List<moves> possiblemoves)
        {
            List<moves> enemypositions = new List<moves>();
            moves move = null;

            for(int i=0; i<possiblemoves.Count; i++)
            {
                move = possiblemoves[i];

                switch (unitControl.currentUnit.team)
                {
                    case 1:
                        if ((map[move.x,move.y] > 0) && (map[move.x, move.y] > 30))
                            enemypositions.Add(move);
                        break;

                    case 2:
                        if ((map[move.x,move.y] > 0) && (map[move.x, move.y] < 40 || map[move.x, move.y] > 60))
                            enemypositions.Add(move);
                        break;
                   
                    case 3:
                        if ((map[move.x,move.y] > 0) && (map[move.x, move.y] < 70 || map[move.x, move.y] > 90))
                            enemypositions.Add(move);
                        break;

                    case 4:
                        if ((map[move.x,move.y] > 0) && (map[move.x, move.y] < 100))
                            enemypositions.Add(move);
                        break;
                }

            }

            if (enemypositions.Count > 0)
            {
                move = enemypositions[0];
                int max = map[move.x, move.y];

                foreach (moves m in enemypositions)
                {
                    if (map[m.x, m.y] > max)
                    {
                        max = map[m.x, m.y];
                        move = m;
                    }
                }

                return move;
            }
            else
                return null;

        }

    }


}
