using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    class Program
    {

        //Tic-Tac-Toe source http://www.codeproject.com/Articles/43622/Solve-Tic-Tac-Toe-with-the-MiniMax-algorithm
        static void Main(string[] args)
        {
            TicTacToeGame game = new TicTacToeGame();
   
            bool stop = false;
            while (!stop)
            {
                bool userFirst = false;
                game = new TicTacToeGame();
                Console.WriteLine("User play against computer, Do you place the first step?[y/n]");
                if (Console.ReadLine().StartsWith("y", StringComparison.InvariantCultureIgnoreCase))
                {
                    userFirst = true;
                }

                int depth = 8;
                Console.WriteLine("Please select level:[1..8]. 1 is easiet, 8 is hardest");
                int.TryParse(Console.ReadLine(), out depth);

                Console.WriteLine("{0} play first, level={1}", userFirst ? "User" : "Computer", depth);

                while (!game.Current.IsTerminalNode())
                {
                    if (userFirst)
                    {
                        game.GetNextMoveFromUser();
                        game.ComputerMakeMove(depth);
                    }
                    else
                    {
                        game.ComputerMakeMove(depth);
                        game.GetNextMoveFromUser();
                    }
                }
                Console.WriteLine("The final result is \n" + game.Current);
                if (game.Current.RecursiveScore < -200)
                    Console.WriteLine("PlayerO has won.");
                else if (game.Current.RecursiveScore > 200)
                    Console.WriteLine("PlayerX has won.");
                else
                    Console.WriteLine("It is a tie.");

                Console.WriteLine("Try again?[y/n]");
                if (!Console.ReadLine().StartsWith("y", StringComparison.InvariantCultureIgnoreCase))
                {
                    stop = true;
                }
            }

            Console.WriteLine("bye");
        }
    }
}
