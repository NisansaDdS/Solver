using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AI;

namespace Checkers
{
    
   
        public enum GridEntry : byte
        {
            Empty,
            PlayerW, //White normal
            PlayerWk, //White king
            PlayerB, //Black normal
            PlayerBk //Black king
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

        public class CheckersBoard : GameState
        {
            GridEntry[] m_Values;
            List<GameState> childrenL = null;
            static int boardSize=10;

            public CheckersBoard(GridEntry[] values, bool turnForPlayerW)
            {
                m_TurnForPlayerOne = turnForPlayerW;
                m_Values =values;
                ComputeScore();
            }


            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < boardSize; i++)
                {
                    for (int j = 0; j < boardSize; j++)
                    {
                        GridEntry v = m_Values[ConvertToLinear(i , j)];
                        char c = '-';
                        if (v == GridEntry.PlayerW)
                            c = 'w';
                        else if (v == GridEntry.PlayerB)
                            c = 'b';
                        else if (v == GridEntry.PlayerWk)
                            c = 'W';
                        else if (v == GridEntry.PlayerBk)
                            c = 'B';
                        sb.Append(c);
                    }
                    sb.Append('\n');
                }
                sb.AppendFormat("score={0},ret={1},{2}", m_Score, RecursiveScore, m_TurnForPlayerOne);
                return sb.ToString();
            }

           
            private static int ConvertToLinear(int x, int y)
            {
                if ((x < 0) || (y < 0) || (x > boardSize - 1) || (y > boardSize - 1))
                {
                    return -1;
                }
                return x + y * boardSize;
            }

            private static int[] ConvertToIndex(int i)
            {
                return (new int[] { i % boardSize, i / boardSize });
            }

            public override bool IsTerminalNode()
            {
                if (GameOver)
                    return true;
                //If both have pieces and can move this is not a terminal node
                int wCount = 0;
                int bcount = 0;
                foreach (GridEntry v in m_Values)
                {
                    if (wCount > 0 && bcount > 0) //And check if each can move
                    { 
                        return false;
                    }
                    else if (v == GridEntry.PlayerB || v == GridEntry.PlayerBk)
                    {
                        bcount++;
                    }
                    else if (v == GridEntry.PlayerW || v == GridEntry.PlayerWk)
                    {
                        wCount++;
                    }
                }
                return true;
            }


            private List<CheckersBoard> getChildrenByMoving(int x, int y)
            {
                return (getChildrenByMoving(ConvertToLinear(x, y)));
            }



            public override IEnumerable<GameState> GetChildren()
            {
                if (childrenL == null)
                {
                    childrenL = new List<GameState>();
                    for (int i = 0; i < m_Values.Length; i++)
                    {
                        if ((m_TurnForPlayerOne && (m_Values[i] == GridEntry.PlayerW || m_Values[i] == GridEntry.PlayerWk)) || (!m_TurnForPlayerOne && (m_Values[i] == GridEntry.PlayerB || m_Values[i] == GridEntry.PlayerBk)))
                        {
                            List<CheckersBoard> c = getChildrenByMoving(i);
                            for (int j = 0; j < c.Count; j++)
                            {
                                childrenL.Add(c[j]);
                            }
                        }
                    }
                }

                for (int i = 0; i < childrenL.Count; i++)
                {
                    yield return childrenL[i];
                }
            }


            public override void ComputeScore()
            {
                int ret = 0;
                int kingWeight = 5;
                int pawnWeight = 1;
                for (int i = 0; i < m_Values.Length; i++)
                {
                    if ((m_TurnForPlayerOne && (m_Values[i] == GridEntry.PlayerW || m_Values[i] == GridEntry.PlayerWk)) || (!m_TurnForPlayerOne && (m_Values[i] == GridEntry.PlayerB || m_Values[i] == GridEntry.PlayerBk)))
                    {
                        switch (m_Values[i])
                        {
                            case GridEntry.PlayerB:
                                ret -= pawnWeight;
                                break;
                            case GridEntry.PlayerBk:
                                ret -= kingWeight;
                                break;
                            case GridEntry.PlayerW:
                                ret += pawnWeight;
                                break;
                            case GridEntry.PlayerWk:
                                ret += kingWeight;
                                break;
                            default:                                
                                break;
                        }
                    }
                }
                m_Score = ret;
            }


            public override GameState TransformBoard(Transform t)
            {
                return this; //Checkers board cannot be rotated like TicTacToe
            }


            private List<CheckersBoard> getChildrenByMoving(int i)
            {                
                List<CheckersBoard> children=new List<CheckersBoard>();

                int[] coords = ConvertToIndex(i);

                int x = coords[0];
                int y = coords[1];


                if (i < 0)
                {
                    return (children);
                }



                if (m_TurnForPlayerOne)
                {
                    if (m_Values[i] == GridEntry.PlayerW)
                    {
                        int possibleLeft = ConvertToLinear(x + 1, y + 1);
                        // int possibleRight = ConvertToLinear(x - 1, y + 1);
                        if (possibleLeft>0&&(m_Values[possibleLeft] == GridEntry.Empty))
                        {
                            GridEntry[] newList = getDeepCopy();
                            newList[i] = GridEntry.Empty;
                            newList[possibleLeft] = GridEntry.PlayerW;
                            CheckersBoard child =new CheckersBoard(newList, !m_TurnForPlayerOne);
                            children.Add(child);
                        }
                        // if (m_Values[possibleRight] = GridEntry.Empty)
                        //  {
                        //ToDo
                        //  }

                    }
                    //   else if (m_Values[i] = GridEntry.PlayerWk)
                    //   {

                    //                    }
                }
                else
                {
                    if (m_Values[i] == GridEntry.PlayerB)
                    {
                        int possibleLeft = ConvertToLinear(x - 1, y - 1);
                        // int possibleRight = ConvertToLinear(x + 1, y - 1);
                        if (possibleLeft>0&&(m_Values[possibleLeft] == GridEntry.Empty))
                        {
                            GridEntry[] newList = getDeepCopy();
                            newList[i] = GridEntry.Empty;
                            newList[possibleLeft] = GridEntry.PlayerB;
                            CheckersBoard child = new CheckersBoard(newList, !m_TurnForPlayerOne);
                            children.Add(child);
                        }
                        // if (m_Values[possibleRight] = GridEntry.Empty)
                        //  {
                        //ToDo
                        //  }

                    }
                    //   else if (m_Values[i] = GridEntry.PlayerBk)
                    //   {

                    //                    }
                }
               
                return (children);
            }


            private GridEntry[] getDeepCopy()
            {
                GridEntry[] newList = new GridEntry[m_Values.Length];
                for (int i = 0; i < m_Values.Length; i++)
                {
                    switch (m_Values[i])
                    {
                        case GridEntry.PlayerB:
                            newList[i]=GridEntry.PlayerB;
                            break;
                        case GridEntry.PlayerBk:
                            newList[i] = GridEntry.PlayerBk;
                            break;
                        case GridEntry.PlayerW:
                            newList[i] = GridEntry.PlayerW;
                            break;
                        case GridEntry.PlayerWk:
                            newList[i] = GridEntry.PlayerWk;
                            break;
                        default:
                            newList[i] = GridEntry.Empty;
                            break;

                    }
                }
                return (newList);
            }

            public override bool IsSameBoard(GameState a, GameState b, bool compareRecursiveScore)
            {
                CheckersBoard aa = (CheckersBoard)a;
                CheckersBoard bb = (CheckersBoard)b;

                if (aa == null || bb == null)
                {
                    return false;
                }

                if (compareRecursiveScore && Math.Abs(aa.RecursiveScore) != Math.Abs(bb.RecursiveScore))
                {
                    return (false);
                }

                return (aa.Equals(bb));
            }


            //Board state is uniquely identified by player positions 
            public override bool Equals(Object obj)
            {
                CheckersBoard boardObj = obj as CheckersBoard;
                if (boardObj == null)
                {
                    return false;
                }
                else
                {
                    for (int i = 0; i < m_Values.Length; i++)
                    {
                        if (m_Values[i] != boardObj.m_Values[i])
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            public GameState FindNextMove1(int depth)
            {
                GameState ret = null; 
                MiniMax(depth, m_TurnForPlayerOne, int.MinValue + 1, int.MaxValue - 1, out ret);               
                return ret;
            }

        }
}
