using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AI;
using System;
using System.IO;

namespace Checkers
{
    class CheckersGame : Game
    {
        static int boardSize = 10;
        static CheckersGame game = null;
        static Boolean isSpecial = false;
        static int cutOffDepth = 0;
        bool[] useNewFunction = new bool[2];
        public List<Int32> branches = new List<Int32>();
        private static int power = 0;
        static Boolean randomization = false;

        public static Boolean Randomization
        {
            get { return CheckersGame.randomization; }
            set { CheckersGame.randomization = value; }
        }

        public static int Power
        {
            get { return CheckersGame.power; }
            set { CheckersGame.power = value; }
        }

        public double GetBranchingFactor()
        {
            int sum = 0;
            for (int i = 0; i < branches.Count; i++)
            {
                sum += branches[i];
            }
            return (sum / branches.Count);
        }



        public bool[] UseNewFunction
        {
            get { return useNewFunction; }
            set {

                if (value[0])
                {
                    useNewFunction[0] = true;
                }

                if (value[1])
                {
                    useNewFunction[1] = true;
                }               
            
            }
        }

        public static void Reset(int cod)
        {
            game = null;
            cutOffDepth = cod;
            PruneHeights = new List<Int32>();
            Game.getInstance().ResetTranspositionTable();
        }


        


        public static CheckersGame getInstance()
        {
            if (game == null)
            {
                if (isSpecial)
                {
                    game = new CheckersGame(isSpecial);
                }
                else
                {
                    game = new CheckersGame();
                }
            }
            return game;
        }

        public static void setSpecial(Boolean s){
            isSpecial=s;
        }

        public CheckersBoard Current
        {
            get;
            private set;
        }


        CheckersBoard init;

        public CheckersBoard GetInitNode()
        {
            return init;
        }


        private CheckersGame(Boolean a)
        {
            GridEntry[] values = new GridEntry[boardSize * boardSize];

            //Add whites
            for (int j = 0; j < boardSize; j++)
            {
                for (int i = 0; i < boardSize; i++)
                {
                    values[i + j * boardSize] = GridEntry.Empty;
                }
            }

/*
            values[1 + 0 * boardSize] = GridEntry.PlayerW;
            values[7 + 0 * boardSize] = GridEntry.PlayerW;
            values[2 + 1 * boardSize] = GridEntry.PlayerW;
            values[3 + 2 * boardSize] = GridEntry.PlayerW;
            values[7 + 2 * boardSize] = GridEntry.PlayerW;
            values[2 + 3 * boardSize] = GridEntry.PlayerW;
            values[4 + 3 * boardSize] = GridEntry.PlayerB;
            values[6 + 3 * boardSize] = GridEntry.PlayerB;
            values[1 + 4 * boardSize] = GridEntry.PlayerW;
            values[3 + 4 * boardSize] = GridEntry.PlayerW;
            values[2 + 5 * boardSize] = GridEntry.PlayerW;
            values[4 + 5 * boardSize] = GridEntry.PlayerW;
            values[6 + 5 * boardSize] = GridEntry.PlayerB;
            values[8 + 5 * boardSize] = GridEntry.PlayerB;
            values[1 + 6 * boardSize] = GridEntry.PlayerW;
            values[2 + 7 * boardSize] = GridEntry.PlayerW;
            values[8 + 7 * boardSize] = GridEntry.PlayerB;
            values[5 + 8 * boardSize] = GridEntry.PlayerW;
            values[7 + 8 * boardSize] = GridEntry.PlayerB;
            values[9 + 8 * boardSize] = GridEntry.PlayerB;
            values[0 + 9 * boardSize] = GridEntry.PlayerW;
            values[2 + 9 * boardSize] = GridEntry.PlayerW;
            values[4 + 9 * boardSize] = GridEntry.PlayerW;
            values[8 + 9 * boardSize] = GridEntry.PlayerB;
*/
            values[6 + 6 * boardSize] = GridEntry.PlayerB;
           // values[8 + 8 * boardSize] = GridEntry.PlayerWk;
            values[3 + 4 * boardSize] = GridEntry.PlayerWk;
            values[7 + 2 * boardSize] = GridEntry.PlayerB;
            values[1 + 3 * boardSize] = GridEntry.PlayerWk;
            values[9 + 0 * boardSize] = GridEntry.PlayerWk;
            


            init = new CheckersBoard(values, true);
            init.Cutoff = cutOffDepth;
            Current = init;
        }

        private CheckersGame()
        {
            GridEntry[] values = new GridEntry[boardSize * boardSize];

            //Add whites
            for (int j = 0; j < boardSize; j++)
            {
                for (int i = 0; i < (boardSize/2)-1; i++)
                {
                    if ((i + j) % 2 != 0)
                    {
                        values[i + j * boardSize] = GridEntry.PlayerW;
                    }
                    else
                    {
                        values[i + j * boardSize] = GridEntry.Empty;
                    }
                }
            }

            //Add the two middle rows
            for (int j = 0; j < boardSize; j++)
            {
                for (int i = (boardSize / 2); i < (boardSize / 2) +2; i++)
                {
                    values[i + j * boardSize] = GridEntry.Empty;
                }
            }

            //Add blacks
            for (int j = 0; j < boardSize; j++)
            {
                for (int i = (boardSize / 2) + 1; i < boardSize ; i++)
                {
                    if ((i + j) % 2 != 0)
                    {
                        values[i + j * boardSize] = GridEntry.PlayerB;
                    }
                    else
                    {
                        values[i + j * boardSize] = GridEntry.Empty;
                    }
                }
            }


            init = new CheckersBoard(values, true);
            init.Cutoff = cutOffDepth;
            Current = init;
        }

       
        int unchangedSteps = 0;

        public CheckersBoard ComputerMakeMove(int depth)
        {
            CheckersBoard next = (CheckersBoard)Current.FindNextMove1(depth);

  /*          string path = @"D:\Documents\Visual Studio 2008\Projects\Solver\WriteLines.txt";
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine("////////////////////////////////////////////////////////");
                sw.WriteLine(Current.ToString());
                sw.WriteLine(Current.getChildrenString());
                sw.WriteLine(next.ToString());
                sw.WriteLine("////////////////////////////////////////////////////////");                
            }*/




            if (next != null)
            {
                if (Current.RecursiveScore == next.RecursiveScore)
                {
                    unchangedSteps++;
                }
                                             
                if (unchangedSteps > 100 && getBoardAge(next)>5)  //http://www.darkfish.com/checkers/rules.html
                {
                    Console.WriteLine("This");
                    next.SetGameDrawn();                   
                }


                //Current = next;
                AddToCheckersTranspositionTable(next);
            }
            return next;

        }

        public void updateState(CheckersBoard next)
        {
            if (next != null)
            {
                Current = next;
            }
        }


        public int[] AddToCheckersTranspositionTable(GameState newC)
        {
            return(Game.getInstance().AddToTranspositionTable(newC));
        }
        



    }
}
