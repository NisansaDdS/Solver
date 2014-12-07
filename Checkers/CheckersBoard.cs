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
                sb.Append(GetBoardString());
                sb.AppendFormat("score={0},ret={1},{2}", m_Score, RecursiveScore, TurnForPlayerOne);
                return sb.ToString();
            }

            public String GetBoardString()
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < boardSize; i++)
                {
                    for (int j = 0; j < boardSize; j++)
                    {
                        int val = ConvertToLinear(i, j);
                        GridEntry v = m_Values[val];
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
                //If either one has run out of pieces, this is a terminal node
                int wCount = 0;
                int bcount = 0;
                foreach (GridEntry v in m_Values)
                {
                    if (v == GridEntry.PlayerB || v == GridEntry.PlayerBk)
                    {
                        bcount++;
                    }
                    else if (v == GridEntry.PlayerW || v == GridEntry.PlayerWk)
                    {
                        wCount++;
                    }
                }

                if (wCount == 0 || bcount == 0)
                {
                    return true;
                }


                //If the current player do not have any leagal moves, this is a terminal node
                CreateChildren();
                if (childrenL.Count == 0)
                {
                    return true;
                }

                return false;
            }


            private List<CheckersBoard> getChildrenByMoving(int x, int y)
            {
                return (getChildrenByMoving(ConvertToLinear(x, y),false));
            }



            public override IEnumerable<GameState> GetChildren()
            {
                if (childrenL == null)
                {
                    CreateChildren();
                }
                

                childrenL.Sort(); 

                for (int i = 0; i < childrenL.Count; i++)
                {
                    yield return childrenL[i];
                }
            }

            private void CreateChildren()
            {
                childrenL = new List<GameState>();
                CheckersGame game = CheckersGame.getInstance();
                for (int i = 0; i < m_Values.Length; i++)
                {
                    if ((TurnForPlayerOne && (m_Values[i] == GridEntry.PlayerW || m_Values[i] == GridEntry.PlayerWk)) || (!TurnForPlayerOne && (m_Values[i] == GridEntry.PlayerB || m_Values[i] == GridEntry.PlayerBk)))
                    {
                        List<CheckersBoard> c = getChildrenByMoving(i, false);
                        for (int j = 0; j < c.Count; j++)
                        {
                            bool backupTurn = c[j].TurnForPlayerOne;
                            c[j]=game.AddToTranspositionTable(c[j]);
                            c[j].TurnForPlayerOne = backupTurn;
                            childrenL.Add(c[j]);                            
                        }
                    }
                }
                

            }


            public override void ComputeScore()
            {
                int ret = 0;
                int kingWeight = 10;
                int pawnWeight = 1;
                for (int i = 0; i < m_Values.Length; i++)
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
                m_Score = ret;
                r_Score = ret;
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
                            CaptureWhite(GridEntry.PlayerW,i, children, possibleLeft, possibleLeft2, nextRow2);
                            captureHappened = true;                            
                        }
                        if (possibleRight2 >= 0 && (m_Values[possibleRight2] == GridEntry.Empty) && ((m_Values[possibleRight] == GridEntry.PlayerB) || (m_Values[possibleRight] == GridEntry.PlayerBk)))
                        {
                            CaptureWhite(GridEntry.PlayerW,i, children, possibleRight, possibleRight2, nextRow2);
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
                            CaptureWhite(GridEntry.PlayerW,i, children, possibleBackLeft, possibleBackLeft2, nextBackRow2);
                            captureHappened = true;
                        }
                        if (possibleBackRight2 >= 0 && (m_Values[possibleBackRight2] == GridEntry.Empty) && ((m_Values[possibleBackRight] == GridEntry.PlayerB) || (m_Values[possibleBackRight] == GridEntry.PlayerBk)))
                        {
                            CaptureWhite(GridEntry.PlayerW,i, children, possibleBackRight, possibleBackRight2, nextBackRow2);
                            captureHappened = true;
                        }


                        //Moving one forward is only done if there was no capture in the same step and if this is not an intemediate recursive step.
                        if (!captureHappened && !isRecursive)
                        {
                            //Console.WriteLine("ooo");
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
                                                               
                        
                        //Back Left
                        for (int j = 1; ; j++)
                        {
                            if (!MoveKing(GridEntry.PlayerWk, i, isRecursive, children, x, y, -j, -j))
                            {
                                break;
                            }
                        }

                        //Back Right
                        for (int j = 1; ; j++)
                        {
                            if (!MoveKing(GridEntry.PlayerWk, i, isRecursive, children, x, y, -j, +j))
                            {
                                break;
                            }
                        }

                        //Front Left
                        for (int j = 1; ; j++)
                        {
                            if (!MoveKing(GridEntry.PlayerWk, i, isRecursive, children, x, y, +j, -j))
                            {
                                break;
                            }
                        }

                        //Front Right
                        for (int j = 1; ; j++)
                        {
                            if (!MoveKing(GridEntry.PlayerWk, i, isRecursive, children, x, y, +j, +j))
                            {
                                break;
                            }
                        }

                        if (isRecursive && children.Count == 0) //We cannot move this piece anymore with captures. Which means this is the state that we hand over to the other player
                        {
                            GridEntry[] newList = getDeepCopy();
                            CheckersBoard child = new CheckersBoard(newList, !TurnForPlayerOne);
                            children.Add(child);
                        }
                        
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
                    else if (m_Values[i] == GridEntry.PlayerBk)
                    {


                        //Back Left
                        for (int j = 1; ; j++)
                        {
                            if (!MoveKing(GridEntry.PlayerBk, i, isRecursive, children, x, y, -j, -j))
                            {
                                break;
                            }
                        }

                        //Back Right
                        for (int j = 1; ; j++)
                        {
                            if (!MoveKing(GridEntry.PlayerBk, i, isRecursive, children, x, y, -j, +j))
                            {
                                break;
                            }
                        }

                        //Front Left
                        for (int j = 1; ; j++)
                        {
                            if (!MoveKing(GridEntry.PlayerBk, i, isRecursive, children, x, y, +j, -j))
                            {
                                break;
                            }
                        }

                        //Front Right
                        for (int j = 1; ; j++)
                        {
                            if (!MoveKing(GridEntry.PlayerBk, i, isRecursive, children, x, y, +j, +j))
                            {
                                break;
                            }
                        }

                        if (isRecursive && children.Count == 0) //We cannot move this piece anymore with captures. Which means this is the state that we hand over to the other player
                        {
                            GridEntry[] newList = getDeepCopy();
                            CheckersBoard child = new CheckersBoard(newList, !TurnForPlayerOne);
                            children.Add(child);
                        }

                    }
                }





               
                return (children);
            }

            private bool MoveKing(GridEntry currentType,int i, Boolean isRecursive, List<CheckersBoard> children, int x, int y, int j,int k)
            {
                int candidate = ConvertToLinear(x + j, y + k);
                int shiftX= j>0 ? 1: -1;
                int shiftY = k > 0 ? 1 : -1;

                //If out of the board, stop
                if (candidate < 0)
                {
                    return false;
                }

                //If blocked by a friendly piece, stop
                if (isSameTeam(currentType, candidate)) 
                {
                    return false;
                }

                //If blocked by an enemy piece,
                if (isDifferentTeam(currentType, candidate))
                {
                    int jumpCandidate = ConvertToLinear(x + j + shiftX, y + k + shiftY);

                    //If jumping position out of the board, stop
                    if (jumpCandidate < 0)
                    {
                        return false;
                    }

                    //If jumping position blocked, stop
                    if (m_Values[jumpCandidate] != GridEntry.Empty)
                    {
                        return false;
                    }

                    CaptureWhite(currentType, i, children, candidate, jumpCandidate, x + j + shiftX);
                    return false; //If captured stop

                }
                else if (!isRecursive)
                {
                    MoveWhite(currentType, i, children, candidate, x + j);                    
                }

                return true;
            }

            private bool isSameTeam(GridEntry currentType, int candidate)
            {
                if (m_Values[candidate] == currentType)
                {
                    return true;
                }
                else if (currentType == GridEntry.PlayerW && m_Values[candidate] == GridEntry.PlayerWk)
                {
                    return true;
                }
                else if (currentType == GridEntry.PlayerWk && m_Values[candidate] == GridEntry.PlayerW)
                {
                    return true;
                }
                else if (currentType == GridEntry.PlayerB && m_Values[candidate] == GridEntry.PlayerBk)
                {
                    return true;
                }
                else if (currentType == GridEntry.PlayerBk && m_Values[candidate] == GridEntry.PlayerB)
                {
                    return true;
                }
                return false;
            }

            private bool isDifferentTeam(GridEntry currentType, int candidate)
            {
                
                if (currentType == GridEntry.PlayerW && m_Values[candidate] == GridEntry.PlayerB)
                {
                    return true;
                }
                else if (currentType == GridEntry.PlayerW && m_Values[candidate] == GridEntry.PlayerBk)
                {
                    return true;
                }
                else if (currentType == GridEntry.PlayerWk && m_Values[candidate] == GridEntry.PlayerB)
                {
                    return true;
                }
                else if (currentType == GridEntry.PlayerWk && m_Values[candidate] == GridEntry.PlayerBk)
                {
                    return true;
                }
                else if (currentType == GridEntry.PlayerB && m_Values[candidate] == GridEntry.PlayerW)
                {
                    return true;
                }
                else if (currentType == GridEntry.PlayerB && m_Values[candidate] == GridEntry.PlayerWk)
                {
                    return true;
                }
                else if (currentType == GridEntry.PlayerBk && m_Values[candidate] == GridEntry.PlayerW)
                {
                    return true;
                }
                else if (currentType == GridEntry.PlayerBk && m_Values[candidate] == GridEntry.PlayerWk)
                {
                    return true;
                }
                return false;
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



            private void CaptureWhite(GridEntry currentType,int i, List<CheckersBoard> children, int possibleTargetLocation, int possibleEndLocation, int endingRow)
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
                    newList[possibleEndLocation] = currentType;
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


            
            

            public GameState FindNextMove1(int depth)
            {
                GameState ret = null;
                //MiniMaxShortVersion(depth, int.MinValue + 1, int.MaxValue - 1, out ret);
            
                MiniMax(depth, TurnForPlayerOne, int.MinValue + 1, int.MaxValue - 1, out ret);               
                return ret;
            }


        


            public override int CompareTo(GameState obj)
            {
                CheckersBoard boardObj = obj as CheckersBoard;
                int retVal = 0;
                if (r_Score > boardObj.r_Score)
                {
                    retVal = 1;
                }
                else if (r_Score < boardObj.r_Score)
                {
                    retVal = -1;
                }
                if (TurnForPlayerOne)
                {
                    retVal *= -1;
                }
                return retVal;
            }

            //Board state is uniquely identified by player positions 
            public override bool Equals(GameState obj)
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
        }
}
