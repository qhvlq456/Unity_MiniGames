using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SequenceBoardGame : BoardGame
{
     // turn == 1 : white , 2 == black
    // top, bottom, left, right, top-left, top-right, bottom-left, bottom-right
    protected enum ESequenceDirection { T, B, L, R, TL, TR, BL, BR }
    protected Queue<int> sequenceQ;
    public int[,] checkDir = new int[,] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } };
    public SequenceBoardGame()
    {
        GameVariable.sequenceNum = 0;
        sequenceQ = new Queue<int>();
    }
    public bool IsEmpty(int r, int c)
    {
        if (GetBoardValue(r, c) != 0 || GetBoardValue(r,c) == GetTurn()) return false;
        else 
        {
            return true;
        }            
    }
    public void CheckDirection(int r, int c, int _turn)
    {
        // top, bottom, left, right, top-left, top-right, bottom-left, bottom-right
        for (int i = 0; i < checkDir.GetLength(0); i++)
        {
            if (CheckOverValue(r, c) || CheckOverValue(r + checkDir[i, 0], c + checkDir[i, 1])) continue; // out of index range check
            if (GetBoardValue(r + checkDir[i, 0], c + checkDir[i, 1]) == _turn)
                sequenceQ.Enqueue(i);
        }
    }
    public abstract bool AnalyzeBoard(int r, int c, int length);
    public abstract bool AnalyzeBoard(int r, int c,int turn, int length);
}
