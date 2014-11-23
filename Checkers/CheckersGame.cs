using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkers
{
    class CheckersGame
    {
        static int boardSize = 10;

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


        public CheckersGame(Boolean a)
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

            values[0 + 0 * boardSize] = GridEntry.PlayerW;
            values[1 + 1 * boardSize] = GridEntry.PlayerB;
            values[1 + 3 * boardSize] = GridEntry.PlayerB;

            


            init = new CheckersBoard(values, true);
            Current = init;
        }

        public CheckersGame()
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
                Current = next;
        }



    }
}
