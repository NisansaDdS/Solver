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
            int depth = 4;
            CheckersGame game = new CheckersGame();
            //game.Current.TurnForPlayerOne = false; //Start with black
            Console.WriteLine(game.Current.ToString());
            Console.ReadLine();
            while (!game.Current.IsTerminalNode())
            {
                //White handicap
                if (!game.Current.TurnForPlayerOne)
               {
                    game.ComputerMakeMove(depth);
               }
               else
               {
                   game.ComputerMakeMove(1);
               }
                Console.WriteLine(game.Current.ToString());
                Console.ReadLine();
            }
        }
    }
}
