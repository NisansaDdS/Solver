using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    class TicTacToeGame
    {
        public TicTacToeBoard Current
        {
            get;
            private set;
        }
        TicTacToeBoard init;

        public TicTacToeGame()
        {
            GridEntry[] values = Enumerable.Repeat(GridEntry.Empty, 9).ToArray();
            init = new TicTacToeBoard(values, true);
            Current = init;
        }

        public void ComputerMakeMove(int depth)
        {
            TicTacToeBoard next = (TicTacToeBoard)Current.FindNextMove(depth);
            if (next != null)
                Current = next;
        }

        public TicTacToeBoard GetInitNode()
        {
            return init;
        }

        public void GetNextMoveFromUser()
        {
            if (Current.IsTerminalNode())
                return;

            while (true)
            {
                try
                {
                    Console.WriteLine("Current Node is\n{0}\n Please type in x:[0-2]", Current);
                    int x = int.Parse(Console.ReadLine());
                    Console.WriteLine("Please type in y:[0-2]");
                    int y = int.Parse(Console.ReadLine());
                    Console.WriteLine("x={0},y={1}", x, y);
                    Current = (TicTacToeBoard)Current.GetChildAtPosition(x, y);
                    Console.WriteLine(Current);
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
