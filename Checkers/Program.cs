using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkers
{
    class Program
    {
     /*   static void Main(string[] args)
        {
            int depth = 4;
            CheckersGame game = CheckersGame.getInstance();
            //game.Current.TurnForPlayerOne = false; //Start with black
            Console.WriteLine(game.Current.ToString());
            Console.ReadLine();
            int steps = 0;
            while (!game.Current.IsTerminalNode())
            {
                //White handicap
              //  if (!game.Current.TurnForPlayerOne)
               //{
                    game.ComputerMakeMove(depth);
              // }
              // else
              // {
             //      game.ComputerMakeMove(1);
             //  }
                Console.WriteLine(game.Current.ToString());
                Console.ReadLine();
                steps++;
            }
            game.printTranspositionTable();
            Console.WriteLine("Steps used: " + steps);
            while (true)
            {
                Console.ReadLine();
            }
        }*/


        static void Main(string[] args)
        {
            int depth = 5;
            int totalSteps = 0;
            int totalpruned = 0;
            int gameCount = 10;
            int wVictories = 0;
            int bVictories = 0;
            DateTime t1 = DateTime.Now;
            for (int i = 0; i < gameCount; i++)
            {
                Console.WriteLine("Playing game "+(i+1)+" of "+gameCount);
                CheckersGame.Reset(depth);                 
                CheckersGame game = (CheckersGame)CheckersGame.getInstance();
                game.UseNewFunction = new bool[] { true,true};
                int steps = 0;
                while (!game.Current.IsTerminalNode())
                {
                    game.ComputerMakeMove(depth);
                    steps++;
                }
                //Console.WriteLine(game.Current.ToString());
               
                if (game.Current.RecursiveScore > 0)
                {
                    wVictories++;
                }
                else if (game.Current.RecursiveScore < 0)
                {
                    bVictories++;
                }
                totalSteps += steps;

                double brancFac=game.GetBranchingFactor();
                for (int j = 0; j < CheckersGame.PruneHeights.Count; j++)
                {
                    totalpruned += (int)Math.Pow(brancFac, CheckersGame.PruneHeights[j]);
                }

              //  totalpruned += CheckersGame.PrunedCount;                
            }
            DateTime t2 = DateTime.Now;
            Console.WriteLine("Total steps used: " + totalSteps);
            Console.WriteLine("Avg steps used: " + ((double)totalSteps/(double)gameCount));
            Console.WriteLine("White won count: " + wVictories);
            Console.WriteLine("Black won count: " + bVictories);
            Console.WriteLine("Time Diff: " + (t2 - t1).TotalMilliseconds);
            Console.WriteLine("Pruning cuts : " + totalpruned);
            Console.ReadLine();
            
            
        }
        
    }
}
