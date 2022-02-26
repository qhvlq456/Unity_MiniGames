using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmokManager : SequenceBoardGame
{
    public int setOverNum = 5;
    protected GameObject _result;
    protected Transform _transform;
    int m_Length = 0;

    public virtual void Awake()
    {
        _result = Resources.Load("NetworkNCMBResultBoard") as GameObject;//Resources.Load("ResultBoard") as GameObject;
        _transform = GameObject.Find("Canvas").transform;
        InitBoard(GameVariable.omokBoardNum);
        gameKind = ESceneKind.Omok;
    }
    private void Start() {
        SoundManager.instance.ChangeBGM(EBGMClipType.Omok);
        GameView.ShowFade(new GameFadeOption{isFade = false, limitedTime = 1f});
    }
    public override bool AnalyzeBoard(int r, int c ,int length)
    {
        if (sequenceQ.Count <= 0) return false;
        
        while (sequenceQ.Count > 0)
        {
            int dir = sequenceQ.Dequeue();
            int _sr,_sc;
            _sr = _sc = 0;

            for (int sr = r, sc = c;
            !CheckOverValue(sr, sc);
            sr += checkDir[dir, 0], sc += checkDir[dir, 1])
            {
                _sr = sr; _sc = sc;
                if(GetBoardValue(sr,sc) != GetTurn())
                {
                    break;
                }
                else // 나와 같은 위치에 있는 돌
                {
                    m_Length++;
                }
            }
            
            if (m_Length >= length) break; // 오목!!
            else ResetLength(); // 재시작

            // 왔던길 되돌아 가기
            for(int row = _sr, col = _sc; !CheckOverValue(row, col); row -= checkDir[dir,0], col -= checkDir[dir,1])
            {
                if(GetBoardValue(row,col) != GetTurn()) 
                {
                    if(m_Length >= length) break;
                    ResetLength();
                }
                else // 나와 같은 위치에 있는 돌
                {
                    m_Length++;
                }
            }
            if (m_Length >= length) break; // 오목!!
            else ResetLength(); // 재시작
        
        }
        if (m_Length >= length) return true;
        else return false;
    }
    public override bool AnalyzeBoard(int r, int c ,int _turn, int length)
    {
        if (sequenceQ.Count <= 0) return false;
        
        while (sequenceQ.Count > 0)
        {
            int dir = sequenceQ.Dequeue();
            int _sr,_sc;
            _sr = _sc = 0;

            for (int sr = r, sc = c;
            !CheckOverValue(sr, sc);
            sr += checkDir[dir, 0], sc += checkDir[dir, 1])
            {
                _sr = sr; _sc = sc;
                if(GetBoardValue(sr,sc) != _turn)
                {
                    break;
                }
                else // 나와 같은 위치에 있는 돌
                {
                    m_Length++;
                }
            }
            
            if (m_Length >= length) break; // 오목!!
            else ResetLength(); // 재시작

            // 왔던길 되돌아 가기
            for(int row = _sr, col = _sc; !CheckOverValue(row, col); row -= checkDir[dir,0], col -= checkDir[dir,1])
            {
                if(GetBoardValue(row,col) != _turn) 
                {
                    if(m_Length >= length) break;
                    ResetLength();
                }
                else // 나와 같은 위치에 있는 돌
                {
                    m_Length++;
                }
            }
            if (m_Length >= length) break; // 오목!!
            else ResetLength(); // 재시작
        
        }
        if (m_Length >= length) return true;
        else return false;
    }
    public override void GameOver()
    {
        // 임시
        if(turn == 1) // 내가 항상 1이니깐.. 하지만 네트워크는 맞음 
        {
            score = (int)EPlayerType.White; // white victory
        }
        else 
        {
            score = (int)EPlayerType.Black; // black victory
        }

        base.GameOver();
        Instantiate(_result, _transform);
    }
    public void ResetLength()
    {
        m_Length = 0;
    }
   
}
