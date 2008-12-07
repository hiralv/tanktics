using System;
using System.Collections.Generic;
using System.Text;


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
        public int[,] map;
        public int[,] values;
        
        public AI()
        {
            map = new int[25, 25];

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    map[i, j] = 0;
                }
            }
        }

        public moves FindBestPossibleMove(List<moves> possiblemoves)
        {
            possiblemoves = RemovesOccupiedSpaces(possiblemoves);
            Random ran = new Random();
            moves move = new moves();
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

        private List<moves> RemovesOccupiedSpaces(List<moves> possiblemoves)
        {
            moves move = new moves();
            for (int i = 0; i < possiblemoves.Count; i++)
            {
                move = possiblemoves[i];

                if (map[move.x, move.y] == 90 || map[move.x, move.y] == 80 || map[move.x, move.y] == 70)
                    possiblemoves.RemoveAt(i);
            }

            return possiblemoves;
        }
    }


}
