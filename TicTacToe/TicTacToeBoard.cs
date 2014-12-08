using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AI;

namespace TicTacToe
{

    public enum GridEntry : byte
    {
        Empty,
        PlayerX,
        PlayerO
    }

    public struct Point
    {
        public int x;
        public int y;
        public Point(int x0, int y0)
        {
            x = x0;
            y = y0;
        }
    }

    public class TicTacToeBoard : GameState
    {
        GridEntry[] m_Values;


        public TicTacToeBoard(GridEntry[] values, bool turnForPlayerX)
        {
            m_TurnForPlayerOne = turnForPlayerX;
            m_Values = values;
            ComputeScore();
        }

 


     public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    GridEntry v = m_Values[i*3 + j];
                    char c = '-';
                    if (v == GridEntry.PlayerX)
                        c = 'X';
                    else if (v == GridEntry.PlayerO)
                        c = 'O';
                    sb.Append(c);
                }
                sb.Append('\n');
            }
            sb.AppendFormat("score={0},ret={1},{2}", m_Score, RecursiveScore, m_TurnForPlayerOne);
            return sb.ToString();
        }



        //Create a child with x,y filled
        public GameState GetChildAtPosition(int x, int y)
        {
            int i = x + y*3;
            GridEntry[] newValues = (GridEntry[])m_Values.Clone();

            if (m_Values[i] != GridEntry.Empty) 
                throw new Exception(string.Format("invalid index [{0},{1}] is taken by {2}",x, y, m_Values[i]));

            newValues[i] = m_TurnForPlayerOne?GridEntry.PlayerX:GridEntry.PlayerO;
            return new TicTacToeBoard(newValues, !m_TurnForPlayerOne);
        }



        public override bool IsTerminalNode()
        {
            if (GameOver)
                return true;
            //if all entries are set, then it is a leaf node
            foreach (GridEntry v in m_Values)
            {
                if (v == GridEntry.Empty)
                    return false;
            }
            return true;
        }


        public override IEnumerable<GameState> GetChildren()
        {
            for (int i = 0; i < m_Values.Length; i++)
            {
                if (m_Values[i] == GridEntry.Empty)
                {
                    GridEntry[] newValues = (GridEntry[])m_Values.Clone();
                    newValues[i] = m_TurnForPlayerOne ? GridEntry.PlayerX : GridEntry.PlayerO;
                    yield return new TicTacToeBoard(newValues, !m_TurnForPlayerOne);
                }
            }
        }


        int GetScoreForOneLine(GridEntry[] values)
        {
            int countX = 0, countO = 0;
            foreach (GridEntry v in values)
            {
                if (v == GridEntry.PlayerX)
                    countX++;
                else if (v == GridEntry.PlayerO)
                    countO++;
            }

            if (countO == 3 || countX == 3)
            {
                GameOver = true;
            }

            //The player who has turn should have more advantage.
            //What we should have done
            int advantage = 1;
            if (countO == 0)
            {
                if (m_TurnForPlayerOne)
                    advantage = 3;
                return (int)System.Math.Pow(10, countX) * advantage;
            }
            else if (countX == 0)
            {
                if (!m_TurnForPlayerOne)
                    advantage = 3;
                return -(int)System.Math.Pow(10, countO) * advantage;
            }
            return 0;
        }


        public override void ComputeScore()
        {
            int ret = 0;
            int[,] lines = { { 0, 1, 2 }, 
                           { 3, 4, 5 }, 
                           { 6, 7, 8 }, 
                           { 0, 3, 6 }, 
                           { 1, 4, 7 }, 
                           { 2, 5, 8 }, 
                           { 0, 4, 8 }, 
                           { 2, 4, 6 } 
                           };

            for (int i = lines.GetLowerBound(0); i <= lines.GetUpperBound(0); i++)
            {
                ret += GetScoreForOneLine(new GridEntry[] { m_Values[lines[i, 0]], m_Values[lines[i, 1]], m_Values[lines[i, 2]] });
            }
            m_Score = ret;
        }


        public override GameState TransformBoard(Transform t)
        {
            GridEntry[] values = Enumerable.Repeat(GridEntry.Empty, 9).ToArray();
            TicTacToeTrasform tt = (TicTacToeTrasform)t;
            for (int i = 0; i < 9; i++)
            {
                Point p = new Point(i % 3, i / 3);
                p = tt.ActOn(p);
                int j = p.x + p.y * 3;
                System.Diagnostics.Debug.Assert(values[j] == GridEntry.Empty);
                values[j] = this.m_Values[i];
            }
            return new TicTacToeBoard(values, m_TurnForPlayerOne);
        }


        public override bool IsSameBoard(GameState a, GameState b, bool compareRecursiveScore)
        {
            TicTacToeBoard aa = (TicTacToeBoard)a;
            TicTacToeBoard bb = (TicTacToeBoard)b;

            if (aa == bb)
                return true;
            if (aa == null || bb == null)
                return false;
            for (int i = 0; i < aa.m_Values.Length; i++)
            {
                if (aa.m_Values[i] != bb.m_Values[i])
                    return false;
            }

            if (aa.m_Score != bb.m_Score)
                return false;

            if (compareRecursiveScore && Math.Abs(aa.RecursiveScore) != Math.Abs(bb.RecursiveScore))
                return false;

            return true;
        }

        public static bool IsSimilarBoard(GameState a, GameState b)
        {

            TicTacToeBoard aa = (TicTacToeBoard)a;
            TicTacToeBoard bb = (TicTacToeBoard)b;


            if (aa.IsSameBoard(aa, bb, false))
                return true;

            foreach (Transform t in TicTacToeTrasform.s_transforms)
            {
                TicTacToeBoard newB = (TicTacToeBoard)bb.TransformBoard(t);
                if (aa.IsSameBoard(aa, newB, false))
                {
                    return true;
                }
            }
            return false;
        }


        public override int CompareTo(GameState other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(GameState other)
        {
            throw new NotImplementedException();
        }

        public override String GetBoardString()
        {
            throw new NotImplementedException();
        }
    }

}
