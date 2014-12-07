using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkers
{
    class CheckersGame
    {
        static int boardSize = 10;
        Dictionary<String, CheckersBoard> transpositionTable = new Dictionary<String, CheckersBoard>();
        static CheckersGame game = null;
        static Boolean isSpecial = false;

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
            Current = init;
        }


        public void ComputerMakeMove(int depth)
        {
            CheckersBoard next = (CheckersBoard)Current.FindNextMove1(depth);
            if (next != null)
            {
                Current = next;
                AddToTranspositionTable(next);
            }
        }

        public CheckersBoard AddToTranspositionTable(CheckersBoard newC)
        {
            CheckersBoard oldC=null;
            String key=newC.GetBoardString();
            if (transpositionTable.TryGetValue(key, out oldC))
            {
                return oldC;
            }
            else
            {
                transpositionTable.Add(key, newC);
                return newC;
            }
        }

        public void printTranspositionTable()
        {
            var enumerator = transpositionTable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CheckersBoard c = enumerator.Current.Value;
                //Console.WriteLine(c.GetBoardString());
                if (c.IsScoreDifferent())
                {
                    Console.WriteLine(c.GetScoreString());
                }
                //Console.WriteLine();
            }
        }



    }
}
