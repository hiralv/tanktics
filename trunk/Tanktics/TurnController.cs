using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tanktics
{
    public class TurnController
    {
        UnitController unitController;



        //Current Phase
        //0 = Unit Placement Phase
        //1 = Point Phase (Points are awarded to player)
        //2 = Movement Phase
        //3 = Combat Phase
        //4 = Purchase Phase
        //5 = Another Players turn phase

        public int phase = 0;
        public int totalAPC = 0;
        public int totalTank = 0;
        public int totalArtil = 0;

        public int MAXAPC = 6;
        public int MAXTANK = 4;
        public int MAXARTIL = 2;

        int startingSmallX;
        int startingSmallY;
        int startingBigX;
        int startingBigY;

        public int points = 0;

        TurnController nextTurnController;



        public TurnController(UnitController UC, int ssX, int ssY, int sbX, int sbY)
        {
            unitController = UC;
            startingSmallX = ssX;
            startingSmallY = ssY;
            startingBigX = sbX;
            startingBigY = sbY;
            nextTurnController = this;
        }

        //Sets the next turn controller to given turn controller
        //Acey Boyce
        public void setNext(TurnController TC)
        {
            nextTurnController = TC;
        }

        //Returns next turn controller
        //Acey Boyce
        public TurnController getNext()
        {
            return nextTurnController;
        }

        public void nextPhase()
        {
            if (phase == 0)
            {
                phase++;
            }
            phase++;
            

            if (phase >= 6)
            {
                phase = 1;
                pointPhase();
                phase++;
            }
        }

        //Adds points based on Factory Control
        //Acey Boyce
        public void pointPhase()
        {

        }
        //Additional checks for creating a unit
        //Acey Boyce
        public Boolean createUnit(String type, int X, int Y)
        {
            //If not empty can't create unit 
            if (!unitController.isEmpty(X, Y))
            {
                return false;
            }
            
            //Must be in the players base
            if ((X >= startingSmallX) && (X <= startingBigX) && (Y >= startingSmallY) && (Y <= startingBigY))
            {
                //Must not excede maximum unit numbers
                if (type.Equals("artillery"))
                {
                    if (totalArtil < MAXARTIL)
                    {   

                        return true;
                    }
                }

                if (type.Equals("tank"))
                {
                    if (totalTank < MAXTANK)
                    {
                        return true;
                    }
                }

                if (type.Equals("apc"))
                {
                    if (totalAPC < MAXAPC)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


    }

}
