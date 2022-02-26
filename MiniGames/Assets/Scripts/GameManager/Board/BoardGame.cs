using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoardGame : GameBase
{
    int[,] board;
    int arrNum;
    public int turn; // 임시
    public BoardGame()
    {
        isGameOver = false;
        turn = 1;
    }
    public void InitBoard(int boardNum)
    {
        arrNum = boardNum;
        board = new int[arrNum, arrNum];
    }
    public virtual void InitGame()
    {
        Array.Clear(board,0,board.Length);
    }
    public int GetBoardValue(int r, int c) // board pos value값을 리턴하여 logic의 구성하게 돕는다
    {
        return board[r, c];
    }
    public void SetBoardValue(int r,int c, int value)
    {
        board[r, c] = value;
    }
    public virtual void GameStart(){}
    public virtual void GameOver()
    {
        IsGameOver();
    }
    public void NextTurn() // 1인용인 경우 사용하지 않음 그만이라 가상함수로 정의하지 않음!
    {
        turn = 3 - turn; // 2 > 1 > 2 > 1
    }
    public int GetNextTurn()
    {
        return 3 - turn;
    }
    public int GetTurn()
    {
        return turn;
    }
    public int GetTurn(int _turn)
    {
        return 3 - _turn;
    }
    public bool CheckOverValue(int r, int c)
    {
        if (r >= arrNum || r < 0) return true;
        if (c >= arrNum || c < 0) return true;
        return false;
    }
    public void DebugBoard()
    {
        string s = "";
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(0); j++)
            {
                s += board[i, j].ToString() + " ";
            }
            s += "\n";
        }
        Debug.Log(s);
    }
    public void TestBoard(int _turn)
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(0) - 2; j++)
            {
                board[i, j] = _turn;
            }
        }
    }
}
