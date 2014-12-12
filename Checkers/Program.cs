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
            int depth = 15;
            int totalSteps = 0;
            int totalpruned = 0;
            int gameCount = 10;
            int wVictories = 0;
            int bVictories = 0;
            DateTime t1 = DateTime.Now;
           // int i = 0;
            List<Int32> powers = new List<Int32>();
            for (int i = 0; i <= 10; i++)
            {
                powers.Add(i);
            }
            for (int i = 20; i <= 100; i=i+10)
            {
                powers.Add(i);
            }
            double[] stats = new double[powers.Count];

            gameCount = powers.Count;
            int loopCount = 10;

            for (int k = 0; k < loopCount; k++)
            {


                for (int i = 0; i < powers.Count; i++)
                {


                    //  for (; i < gameCount; i++)
                    //  {
                    Console.WriteLine("Playing game " + ((i + 1) + (k * powers.Count)) + " of " + (loopCount * powers.Count));
                    CheckersGame.Reset(depth);
                    // CheckersGame.setSpecial(true);
                    CheckersGame.Power = powers[i];
                    Console.WriteLine("Power " + powers[i]);
                    CheckersGame game = (CheckersGame)CheckersGame.getInstance();
                    game.UseNewFunction = new bool[] { false, false };
                    game.UseNewFunction = new bool[] { true, true };
                    int steps = 0;
                    int depthSum = 0;
                    while (!game.Current.IsTerminalNode())
                    {
                        DateTime t3 = DateTime.Now;
                        CheckersBoard best = game.ComputerMakeMove(0);
                        for (int j = 1; j <= depth; j++)
                        {
                            DateTime t4 = DateTime.Now;
                            if ((t4 - t3).TotalMilliseconds < 100)
                            {
                                best = game.ComputerMakeMove(j);
                                game.updateState(best);
                                if (j == depth)
                                {
                                    depthSum += j;
                                }
                            }
                            else
                            {
                                depthSum += (j - 1);
                                break;
                            }
                        }
                        steps++;
                    }
                    stats[i] += ((double)depthSum / (double)steps);
                    Console.WriteLine("Avg depth: " + stats[i]);

                    /*              if (game.Current.RecursiveScore > 0)
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
                  */
                    //  totalpruned += CheckersGame.PrunedCount;                
                    // }
                }
            }


            for (int i = 0; i < powers.Count; i++)
            {
                Console.WriteLine(powers[i] + " -> " + (stats[i] / loopCount));
            }


            DateTime t2 = DateTime.Now;
      //      Console.WriteLine("Total steps used: " + totalSteps);
      //      Console.WriteLine("Avg steps used: " + ((double)totalSteps/(double)gameCount));
       //     Console.WriteLine("White won count: " + wVictories);
    //        Console.WriteLine("Black won count: " + bVictories);
    //        Console.WriteLine("Time Diff: " + (t2 - t1).TotalMilliseconds);
    //        Console.WriteLine("Pruning cuts : " + totalpruned);
            Console.ReadLine();
            
            
        }
        
    }
}
