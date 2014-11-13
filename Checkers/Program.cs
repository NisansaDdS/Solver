using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkers
{
    class Program
    {
        static void Main(string[] args)
        {
            int depth = 8;
            CheckersGame game = new CheckersGame();
            Console.WriteLine(game.Current.ToString());
            Console.ReadLine();
            while (!game.Current.IsTerminalNode())
            {
                game.ComputerMakeMove(depth);
                Console.WriteLine(game.Current.ToString());
                Console.ReadLine();
            }
        }
    }
}
