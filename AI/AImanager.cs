using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI
{
    public class AImanger
    {

        /*
         Basic MinMax ALpha-Beta  Code from: http://www.codeproject.com/Articles/43622/Solve-Tic-Tac-Toe-with-the-MiniMax-algorithm           
        
        public int MiniMax(int depth, bool needMax, int alpha, int beta, out GameState selectedChild)
        {
            selectedChild = null;
            System.Diagnostics.Debug.Assert(m_TurnForPlayerX == needMax);
            if (depth == 0 || IsTerminalNode())
            {
                RecursiveScore = m_Score;
                return m_Score;
            }
            foreach (GameState cur in GetChildren())
            {
                GameState dummy;
                int score = cur.MiniMax(depth - 1, !needMax, alpha, beta, out dummy);
                if (!needMax)
                {
                    if (beta > score)
                    {
                        beta = score;
                        selectedChild = cur;
                        if (alpha >= beta)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (alpha < score)
                    {
                        alpha = score;
                        selectedChild = cur;
                        if (alpha >= beta)
                        {
                            break;
                        }
                    }
                }
            }
            RecursiveScore = needMax ? alpha : beta;
            return RecursiveScore;
        }*/


    }
}
