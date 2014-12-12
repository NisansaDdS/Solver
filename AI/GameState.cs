using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI
{
    public abstract class GameState : IComparable<GameState>, IEquatable<GameState>
    {
        public GameState parent;
        public List<GameState> children = new List<GameState>();

        protected int m_Score;
        protected bool m_TurnForPlayerOne;
        protected int r_Score;
        int distFromCutoff = 0;

        public int DistFromCutoff
        {
            get { return distFromCutoff; }
            set { distFromCutoff = value; }
        }
        private int cutoff = 0;

        public int Cutoff
        {
            get { return cutoff; }
            set { cutoff = value; }
        }

        public String GetScoreString()
        {
            return "R score: " + r_Score + " M score: " + m_Score;
        }

        public bool IsScoreDifferent()
        {
            return r_Score != m_Score;
        }

        public bool TurnForPlayerOne
        {
            get { return m_TurnForPlayerOne; }
            set { m_TurnForPlayerOne = value; }
        }

        public int[] GetScores()
        {
            return new int[]{r_Score,m_Score};
        }

        public void SetScores(int[] scores)
        {
            r_Score = scores[0];
            m_Score = scores[1];
        }
       
        public int RecursiveScore
        {
            get;
            protected set;
        }
       public bool GameOver
        {
            get;
            protected set;
        }



        public abstract IEnumerable<GameState> GetChildren();

    //    public abstract double getEval();

   //     public abstract String Hash();

        public abstract bool IsTerminalNode();

        public abstract void ComputeScore();

        public abstract GameState TransformBoard(Transform t);

        public abstract /*static*/ bool IsSameBoard(GameState a, GameState b, bool compareRecursiveScore);


        public abstract String GetBoardString();

     //   public abstract /*static*/ bool IsSimilarBoard(GameState a, GameState b);


        //http://en.wikipedia.org/wiki/Alpha-beta_pruning
        public int MiniMaxShortVersion(int depth, int alpha, int beta, out GameState childWithMax)
        {
            childWithMax = null;
            if (depth == 0 || IsTerminalNode())
            {
                //When it is turn for PlayO, we need to find the minimum score.
                RecursiveScore = m_Score;
                return m_TurnForPlayerOne ? m_Score : -m_Score;
            }

            foreach (GameState cur in GetChildren())
            {
                GameState dummy;
                int score = -cur.MiniMaxShortVersion(depth - 1, -beta, -alpha, out dummy);
                if (alpha < score)
                {
                    alpha = score;
                    childWithMax = cur;
                    if (alpha >= beta)
                    {
                        break;
                    }
                }
            }

            RecursiveScore = alpha;
            return alpha;
        }



        //http://www.ocf.berkeley.edu/~yosenl/extras/alphabeta/alphabeta.html
        public int MiniMax(int depth, bool needMax, int alpha, int beta, out GameState childWithMax)
        {
            childWithMax = null;
            System.Diagnostics.Debug.Assert(m_TurnForPlayerOne == needMax);
            if (depth == 0 || IsTerminalNode())
            {
                RecursiveScore = m_Score;
                return m_Score;
            }

            distFromCutoff = depth;
           
            foreach (GameState cur in GetChildren())
            {
                GameState dummy;
                int score = cur.MiniMax(depth - 1, !needMax, alpha, beta, out dummy);            
                if (!needMax)
                {
                    if (beta > score)
                    {
                        beta = score;
                        childWithMax = cur;
                        if (alpha >= beta)
                        {
                            break;
                        }
                    }
                    else
                    {
                        Game.PruneHeights.Add(depth - 1);
                    }
                }
                else
                {
                    if (alpha < score)
                    {
                        alpha = score;
                        childWithMax = cur;
                        if (alpha >= beta)
                        {
                            break;
                        }
                    }
                    else
                    {
                        Game.PruneHeights.Add(depth - 1);
                    }
                }

                Game.getInstance().updateTranspositionTable(cur);

            }

           
            RecursiveScore = needMax ? alpha : beta;
            r_Score = RecursiveScore;           
            return RecursiveScore;
        }




        public GameState FindNextMove(int depth)
        {
            GameState ret = null;
            GameState ret1 = null;
            MiniMaxShortVersion(depth, int.MinValue + 1, int.MaxValue - 1, out ret1);
            MiniMax(depth, m_TurnForPlayerOne, int.MinValue + 1, int.MaxValue - 1, out ret);

            //compare the two versions of MiniMax give the same results
            if (!IsSameBoard(ret, ret1, true))
            {
                Console.WriteLine("ret={0}\n,!= ret1={1},\ncur={2}", ret, ret1, this);
                throw new Exception("Two MinMax functions don't match.");
            }
            return ret;
        }



        #region IComparable<GameState> Members

        public abstract int CompareTo(GameState other);

        #endregion

        #region IEquatable<GameState> Members

        public abstract bool Equals(GameState other);

        #endregion
    }
}
