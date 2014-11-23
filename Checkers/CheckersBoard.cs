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
                TurnForPlayerOne = turnForPlayerW;
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
                        int val = ConvertToLinear(i, j);
                        GridEntry v = m_Values[val];
                        char c ='-';
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
                sb.AppendFormat("score={0},ret={1},{2}", m_Score, RecursiveScore, TurnForPlayerOne);
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
                return (getChildrenByMoving(ConvertToLinear(x, y),false));
            }



            public override IEnumerable<GameState> GetChildren()
            {
                if (childrenL == null)
                {
                    childrenL = new List<GameState>();
                    for (int i = 0; i < m_Values.Length; i++)
                    {
                        if ((TurnForPlayerOne && (m_Values[i] == GridEntry.PlayerW || m_Values[i] == GridEntry.PlayerWk)) || (!TurnForPlayerOne && (m_Values[i] == GridEntry.PlayerB || m_Values[i] == GridEntry.PlayerBk)))
                        {
                            List<CheckersBoard> c = getChildrenByMoving(i,false);
                            for (int j = 0; j < c.Count; j++)
                            {
                                childrenL.Add(c[j]);
                            }

                          //  if (c.Count == 0)
                           // {
                          //      int[] coords = ConvertToIndex(i);
                          //      Console.WriteLine("Cannot move " + m_Values[i] + " at "+i+ "-> ( " + coords[0] + " , " + coords[1]+" )");
                          //  }
                        }
                    }



                   // Console.WriteLine(childrenL.Count);
                   




                }
                else
                {
                    Console.WriteLine("Not null");
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
                    if ((TurnForPlayerOne && (m_Values[i] == GridEntry.PlayerW || m_Values[i] == GridEntry.PlayerWk)) || (!TurnForPlayerOne && (m_Values[i] == GridEntry.PlayerB || m_Values[i] == GridEntry.PlayerBk)))
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










            private List<CheckersBoard> getChildrenByMoving(int i,Boolean isRecursive)
            {
               
                

                List<CheckersBoard> children=new List<CheckersBoard>();

                int[] coords = ConvertToIndex(i);

                int x = coords[0];
                int y = coords[1];


                if (i < 0)
                {
                    return (children);
                }



                if (TurnForPlayerOne)
                {
                    if (m_Values[i] == GridEntry.PlayerW)
                    {
                        Boolean captureHappened = false;

                        //Forward capture 
                        int possibleLeft = ConvertToLinear(x + 1, y + 1);
                        int possibleRight = ConvertToLinear(x + 1, y - 1);
                        int nextRow = x + 1;

                        int possibleLeft2 = ConvertToLinear(x + 2, y + 2);
                        int possibleRight2 = ConvertToLinear(x + 2, y - 2);
                        int nextRow2 = x + 2;
                        

                        if (possibleLeft2 >= 0 && (m_Values[possibleLeft2] == GridEntry.Empty) && ((m_Values[possibleLeft] == GridEntry.PlayerB) || (m_Values[possibleLeft] == GridEntry.PlayerBk)))
                        {                           
                            CaptureWhiteNormal(i, children, possibleLeft, possibleLeft2, nextRow2);
                            captureHappened = true;                            
                        }
                        if (possibleRight2 >= 0 && (m_Values[possibleRight2] == GridEntry.Empty) && ((m_Values[possibleRight] == GridEntry.PlayerB) || (m_Values[possibleRight] == GridEntry.PlayerBk)))
                        {                           
                            CaptureWhiteNormal(i, children, possibleRight, possibleRight2, nextRow2);
                            captureHappened = true;                           
                        }

                        //Backward capture
                        int possibleBackLeft = ConvertToLinear(x - 1, y + 1);
                        int possibleBackRight = ConvertToLinear(x - 1, y - 1);
                        int nextBackRow = x - 1;

                        int possibleBackLeft2 = ConvertToLinear(x - 2, y + 2);
                        int possibleBackRight2 = ConvertToLinear(x - 2, y - 2);
                        int nextBackRow2 = x - 2;

                        if (possibleBackLeft2 >= 0 && (m_Values[possibleBackLeft2] == GridEntry.Empty) && ((m_Values[possibleBackLeft] == GridEntry.PlayerB) || (m_Values[possibleBackLeft] == GridEntry.PlayerBk)))
                        {
                            CaptureWhiteNormal(i, children, possibleBackLeft, possibleBackLeft2, nextBackRow2);
                            captureHappened = true;
                        }
                        if (possibleBackRight2 >= 0 && (m_Values[possibleBackRight2] == GridEntry.Empty) && ((m_Values[possibleBackRight] == GridEntry.PlayerB) || (m_Values[possibleBackRight] == GridEntry.PlayerBk)))
                        {
                            CaptureWhiteNormal(i, children, possibleBackRight, possibleBackRight2, nextBackRow2);
                            captureHappened = true;
                        }


                        //Moving one forward is only done if there was no capture in the same step and if this is not an intemediate recursive step.
                        if (!captureHappened && !isRecursive)
                        {
                            if (possibleLeft >= 0 && (m_Values[possibleLeft] == GridEntry.Empty))
                            {
                                MoveWhite(GridEntry.PlayerW, i, children, possibleLeft, nextRow);
                            }
                            if (possibleRight >= 0 && (m_Values[possibleRight] == GridEntry.Empty))
                            {
                                MoveWhite(GridEntry.PlayerW, i, children, possibleRight, nextRow);
                            }
                        }

                        if (isRecursive && children.Count == 0) //We cannot move this piece anymore with captures. Which means this is the state that we hand over to the other player
                        {
                            GridEntry[] newList = getDeepCopy();
                            CheckersBoard child = new CheckersBoard(newList, !TurnForPlayerOne);
                            children.Add(child);
                        }
                        
                    }
                    else if (m_Values[i] == GridEntry.PlayerWk)
                    {
                        Console.WriteLine("Trying to move the white king at "+x+" "+y);
                        Boolean captureHappened = false;
                        
                        
                        //Left back
                        for (int j = 1; ; j++)
                        {
                            int leftCandidate = ConvertToLinear(x - j, y - j);
                            
                            //If out of the board, stop
                            if (leftCandidate < 0)
                            {
                                break;
                            }

                            //If blocked by a friendly piece, stop
                            if (m_Values[leftCandidate] == GridEntry.PlayerW || m_Values[leftCandidate] == GridEntry.PlayerWk)
                            {
                                break;
                            }

                            //If blocked by an enemy piece,
                            if (m_Values[leftCandidate] == GridEntry.PlayerB || m_Values[leftCandidate] == GridEntry.PlayerBk)
                            {
                                int leftJumpCandidate = ConvertToLinear(x + i + 1, y + i + 1);

                                //If jumping position out of the board, stop
                                if (leftJumpCandidate < 0)
                                {
                                    break;
                                }

                                //If jumping position blocked, stop
                                if (m_Values[leftCandidate] != GridEntry.Empty)
                                {
                                    break;
                                }

                                //Capture!!!!

                            }
                            else
                            {
                                MoveWhite(GridEntry.PlayerWk, i, children, leftCandidate, x + j);
                            }


                        }


                        List<Int32> BackLeftDiag = new List<Int32>();
                        List<Int32> BackRightDiag = new List<Int32>();
                        List<Int32> FrontLeftDiag = new List<Int32>();
                        List<Int32> FrontRightDiag = new List<Int32>();
                        
                    }
                  
                }
                else
                {
                    if (m_Values[i] == GridEntry.PlayerB)
                    {
                        Boolean captureHappened = false;

                        //Forward capture 
                        int possibleLeft = ConvertToLinear(x - 1, y - 1);
                        int possibleRight = ConvertToLinear(x - 1, y + 1);
                        int nextRow = x - 1;

                        int possibleLeft2 = ConvertToLinear(x - 2, y - 2);
                        int possibleRight2 = ConvertToLinear(x - 2, y + 2);
                        int nextRow2 = x - 2;

                        if (possibleLeft2 >= 0 && (m_Values[possibleLeft2] == GridEntry.Empty) && ((m_Values[possibleLeft] == GridEntry.PlayerW) || (m_Values[possibleLeft] == GridEntry.PlayerWk)))
                        {
                            CaptureBlackNormal(i, children, possibleLeft, possibleLeft2, nextRow2);
                            captureHappened = true;
                        }
                        if (possibleRight2 >= 0 && (m_Values[possibleRight2] == GridEntry.Empty) && ((m_Values[possibleRight] == GridEntry.PlayerW) || (m_Values[possibleRight] == GridEntry.PlayerWk)))
                        {
                            CaptureBlackNormal(i, children, possibleRight, possibleRight2, nextRow2);
                            captureHappened = true;
                        }

                        //Backward capture
                        int possibleBackLeft = ConvertToLinear(x + 1, y - 1);
                        int possibleBackRight = ConvertToLinear(x + 1, y + 1);
                        int nextBackRow = x + 1;

                        int possibleBackLeft2 = ConvertToLinear(x + 2, y - 2);
                        int possibleBackRight2 = ConvertToLinear(x + 2, y + 2);
                        int nextBackRow2 = x + 2;

                        if (possibleBackLeft2 >= 0 && (m_Values[possibleBackLeft2] == GridEntry.Empty) && ((m_Values[possibleBackLeft] == GridEntry.PlayerW) || (m_Values[possibleBackLeft] == GridEntry.PlayerWk)))
                        {
                            CaptureBlackNormal(i, children, possibleBackLeft, possibleBackLeft2, nextBackRow2);
                            captureHappened = true;
                        }
                        if (possibleBackRight2 >= 0 && (m_Values[possibleBackRight2] == GridEntry.Empty) && ((m_Values[possibleBackRight] == GridEntry.PlayerW) || (m_Values[possibleBackRight] == GridEntry.PlayerWk)))
                        {
                            CaptureBlackNormal(i, children, possibleBackRight, possibleBackRight2, nextBackRow2);
                            captureHappened = true;
                        }
                        
                        //Moving one forward is only done if there was no capture in the same step
                        if (!captureHappened && !isRecursive)
                        {
                            if (possibleLeft >= 0 && (m_Values[possibleLeft] == GridEntry.Empty))
                            {
                                MoveBlackNormal(i, children, possibleLeft, nextRow);
                            }
                            if (possibleRight >= 0 && (m_Values[possibleRight] == GridEntry.Empty))
                            {
                                MoveBlackNormal(i, children, possibleRight, nextRow);
                            }
                        }

                        if (isRecursive && children.Count == 0) //We cannot move this piece anymore with captures. Which means this is the state that we hand over to the other player
                        {
                            GridEntry[] newList = getDeepCopy();
                            CheckersBoard child = new CheckersBoard(newList, !TurnForPlayerOne);
                            children.Add(child);
                        }


                    }
                    //   else if (m_Values[i] = GridEntry.PlayerBk)
                    //   {

                    //                    }
                }





               
                return (children);
            }


            private void CaptureBlackNormal(int i, List<CheckersBoard> children, int possibleTargetLocation, int possibleEndLocation, int endingRow)
            {

                GridEntry[] newList = getDeepCopy();
                newList[i] = GridEntry.Empty;
                newList[possibleTargetLocation] = GridEntry.Empty; //Kill the enemy piece
                if (endingRow == 0)
                {
                    newList[possibleEndLocation] = GridEntry.PlayerBk;
                }
                else
                {
                    newList[possibleEndLocation] = GridEntry.PlayerB;
                }

              //  CheckersBoard child = new CheckersBoard(newList, !TurnForPlayerOne);
               // children.Add(child);

                CheckersBoard reccursivechild = new CheckersBoard(newList, TurnForPlayerOne);


                List<CheckersBoard> reccursiveChildern = reccursivechild.getChildrenByMoving(possibleEndLocation, true);

                if (reccursiveChildern.Count == 0)
                {
                    CheckersBoard nonReccursivechild = new CheckersBoard(newList, !TurnForPlayerOne);
                    children.Add(nonReccursivechild);
                }
                else
                {
                    children.AddRange(reccursiveChildern);
                }
            }



            private void CaptureWhiteNormal(int i, List<CheckersBoard> children, int possibleTargetLocation, int possibleEndLocation, int endingRow)
            {

                GridEntry[] newList = getDeepCopy();
                newList[i] = GridEntry.Empty;
                newList[possibleTargetLocation] = GridEntry.Empty; //Kill the enemy piece
                if (endingRow == boardSize - 1)
                {
                    newList[possibleEndLocation] = GridEntry.PlayerWk;
                }
                else
                {
                    newList[possibleEndLocation] = GridEntry.PlayerW;
                }

                CheckersBoard reccursivechild = new CheckersBoard(newList, TurnForPlayerOne);
               

                List<CheckersBoard> reccursiveChildern = reccursivechild.getChildrenByMoving(possibleEndLocation, true);

                if (reccursiveChildern.Count == 0)
                {                   
                    CheckersBoard nonReccursivechild = new CheckersBoard(newList, !TurnForPlayerOne);
                    children.Add(nonReccursivechild);
                }
                else
                {  
                    children.AddRange(reccursiveChildern);                   
                }

            }

            private void MoveBlackNormal(int i, List<CheckersBoard> children, int possibleLocation, int nextRow)
            {
                GridEntry[] newList = getDeepCopy();
                newList[i] = GridEntry.Empty;
                if (nextRow == 0)
                {
                    newList[possibleLocation] = GridEntry.PlayerBk;
                }
                else
                {
                    newList[possibleLocation] = GridEntry.PlayerB;
                }
                CheckersBoard child = new CheckersBoard(newList, !TurnForPlayerOne);
                children.Add(child);
            }

            private void MoveWhite(GridEntry currentType,int i, List<CheckersBoard> children, int possibleLocation, int nextRow)
            {
                GridEntry[] newList = getDeepCopy();
                newList[i] = GridEntry.Empty;
                if (nextRow == boardSize - 1)
                {
                    newList[possibleLocation] = GridEntry.PlayerWk;
                }
                else
                {
                    newList[possibleLocation] = currentType;
                }
                CheckersBoard child = new CheckersBoard(newList, !TurnForPlayerOne);
                children.Add(child);
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
                MiniMax(depth, TurnForPlayerOne, int.MinValue + 1, int.MaxValue - 1, out ret);               
                return ret;
            }

        }
}
