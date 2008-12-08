using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace Tanktics
{
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

        #region Values
        static public int[,] map;
        static public int[,] values = new int[25, 25] {  
                            {1, 2,	3,	4,	5,	10,	15,	20,	25,	30,	35,	40,	45,	40,	35,	30,	25,	20,	15,	10,	 5,	 4,	 3,	 2,	1},
                            {2,	2,	3,	4,	6,	11,	15,	20,	25,	31,	36,	41,	46,	41,	36,	31,	26,	21,	16,	11,	 6,	 4,	 3,	 2,	2},
                            {3,	3,	3,	4,	5,	10,	15,	20,	25,	30,	-4,	-4,	50,	-4,	-4,	30,	25,	20,	15,	10,	 5,	 4,	 3,	 3,	3},
                            {4,	4,	4,	4,	5,	10,	15,	20,	25,	30,	-4,	50,	55,	50,	-4,	30,	25,	20,	15,	10,	 5,	 4,	 4,	 4,	4},
                            {5,	6,	5,	6,	6,	11,	16,	21,	26,	31,	-4,	46,	51,	46,	-4,	31,	26,	21,	16,	11,	 6,	 5,	 5,  6,	5},
                            {10,11,	10,	10,	11,	12,	16,	21,	26,	31,	36,	41,	45,	45,	36,	31,	26,	21,	17,	12,	11, 10, 10, 10, 10},
                            {15,16,	15,	15,	15,	16,	17,	21,	26,	31,	36,	40,	40,	41,	36,	31,	26,	22,	17,	16,	16,	15, 15, 15, 15},
                            {20,21,	20,	20,	20,	21,	21,	22,	26,	36,	41,	45,	50,	40,	35,	31,	27,	22,	21, 21,	21,	20, 20, 20, 20},
                            {25,26,	25,	25,	25,	26,	26,	26,	27,	31,	45,	50,	55,	45,	36,	32,	27,	26,	26, 26, 26,	25, 25, 26, 25},
                            {30,31,	30,	30,	30,	31,	31,	31,	31,	32,	36,	56,	60,	50,	36,	32,	31,	31,	31, 31,	31,	30, 30, 31, 30},
                            {35,36,	-4,	-4,	-4,	36,	36,	35,	35,	40,	-4,	61,	65,	55,	-4,	40,	35,	36,	36, 36, -4, -4, -4, 36, 35},
                            {40,41,	-4,	50,	46,	40,	41,	40,	40,	45,	-4,	65,	71,	60,	-4,	45,	40,	40,	40, 41,	45,	50, -4, 41, 40},
                            {45,46,	50,	55,	50,	46,	45,	45,	50,	55,	-4,	70,	71,	65,	-4,	55,	45,	45,	46, 46, 50, 55, 50, 46, 45},
                            {40,41,	-4,	50,	46,	41,	40,	40,	40,	45,	-4,	60,	66,	65,	-4,	45,	40,	30,	41, 41, 46, 50, -4, 41, 40},
                            {35,36,	-4,	-4,	-4,	36,	35,	35,	35,	40,	-4,	56,	61,	60,	-4,	40,	35,	35,	36, 36, -4, -4, -4, 36, 35},
                            {30,31,	30,	30,	31,	31,	31,	31,	31,	32,	36,	51,	55,	56,	0,	32,	31,	31,	31, 31, 31, 30, 30, 31, 30},
                            {25,25,	25,	25,	26,	26,	26,	26,	27,	31,	36,	46,	51,	51,	0,	31,	27,	26,	26, 26, 26, 25, 25, 25, 25},
                            {20,20,	20,	20,	21,	21,	21,	22,	26,	31,	36,	41,	50,	46,	41,	31,	26,	22,	21, 21, 21, 20, 20, 20, 20},
                            {15,15,	15,	15,	16,	16,	17,	21,	26,	31,	35,	40,	51,	41,	35,	31,	26,	21,	17, 16, 16, 15, 15, 15, 15},
                            {10,11,	10,	10,	11,	12,	16,	21,	26,	31,	36,	41,	45,	41,	36,	31,	26,	21,	16,	12,11, 10, 10, 11, 10},
                            {5,	6,	5,	5,	6,	6,	16,	21,	26,	31,	-4,	46,	50,	46,	-4,	31,	26,	21,	16,	11,	6,	5,	5,	6,  5},
                            {4,	4,	4,	4,	5,	10,	15,	20,	25,	30,	-4,	50,	55,	50,	-4,	30,	25,	20,	15,	10,	5,	4,	4,	4,	4},
                            {3,	3,	3,	4,	5,	10,	15,	20,	25,	30,	-4,	-4,	50,	-4,	-4,	30,	25,	20,	15,	10,	5,	4,	3,	3,	3},
                            {2,	2,	3,	4,	5,	11,	16,	21,	26,	31,	36,	41,	46,	41,	36,	31,	25,	20,	15,	11,	6,	4,	3,	2,	2},
                            {1,	2,	3,	4,	5,	10,	15,	20,	25,	30,	35,	40,	45,	40,	35,	30,	25,	20,	15,	10,	5,	4,	3,	2,	1}};

        #endregion
        
        public AI(UnitController unitControl)
        {
            this.unitControl = unitControl;
            
            map = new int[25, 25];

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    map[i, j] = 0;
                }
            }
        }

        public Point NextMove()
        {
            Unit mostouter = unitControl.currentUnit;
            Unit firstunit = unitControl.currentUnit;
            unitControl.nextUnit();
            Unit nextunit = unitControl.currentUnit;

            if (mostouter.hasMoved)
            {
                unitControl.nextUnit();
                unitControl.finalize();
                return new Point(unitControl.currentUnit.currentX, unitControl.currentUnit.currentY);
                
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
            }
            else if (result == 3)
            {
                mostouter.sethasMoved(true);
            }

            Point point = new Point(unitControl.currentUnit.currentX, unitControl.currentUnit.currentY);


            return point;
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
