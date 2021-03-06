﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI
{
    public class Game
    {
        private Dictionary<String, int[]> transpositionTable = new Dictionary<String, int[]>();

        private static Game game = null;
        private static List<Int32> pruneHeights = new List<Int32>();

        public static List<Int32> PruneHeights
        {
            get { return pruneHeights; }
            set { pruneHeights = value; }
        }

        public void ResetTranspositionTable()
        {
            transpositionTable = new Dictionary<String, int[]>();
            System.GC.Collect();
        }


        public static Game getInstance()
        {
            if (game == null)
            {
                game = new Game();
            }
            return game;
        }


        public void updateTranspositionTable(GameState newC)
        {            
          //  String key = newC.GetBoardString();
          //  if (transpositionTable.ContainsKey(key))
          //  {
          //      transpositionTable.Remove(key);
          //  }

            AddToTranspositionTable(newC);
          //  transpositionTable.Add(key, newC.GetScores());
           // Console.WriteLine("U: "+transpositionTable.Count);
        }

        public int getBoardAge(GameState newC)
        {
            int[] scores = GetFromTranspositionTable(newC);
            if (scores == null)
            {
                return 0;
            }
            else
            {
                return scores[2];
            }
        }
        
        
        public int[] GetFromTranspositionTable(GameState newC)
        {
            int[] scores = null;
            transpositionTable.TryGetValue(newC.GetBoardString(), out scores);         
            return scores;
        }


        public int[] AddToTranspositionTable(GameState newC)
        {
            int[] scores = null;
            String key = newC.GetBoardString();
            int[] tempScores = newC.GetScores();
            if (!transpositionTable.TryGetValue(key, out scores))
            {
                scores = new int[3];                
                scores[0] = tempScores[0];
                scores[1] = tempScores[1];
                scores[2] = 1;               
            }
            else
            {   
                scores[0] = tempScores[0];
                scores[1] = tempScores[1];
                scores[2]++;
                transpositionTable.Remove(key);
            }
            transpositionTable.Add(key, scores);
            //Console.WriteLine("A: " + transpositionTable.Count);
            return scores;
        }

        public void printTranspositionTable()
        {
            var enumerator = transpositionTable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int[] c = enumerator.Current.Value;
                //Console.WriteLine(c.GetBoardString());
                if (c[0] != c[1])
                {
                    Console.WriteLine("R score: " + c[0] + " M score: " + c[1]);
                }
                //Console.WriteLine();
            }
        }
    }
}
