using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// manager는 도구일 뿐이야!
public class OthelloManager : SequenceBoardGame
{
    [SerializeField]
    Text whiteNumberText;
    [SerializeField]
    Text blackNumberText;
    [SerializeField]
    GameObject numbering;
    [SerializeField]
    GameObject board;
    protected GameObject _result;
    Transform _transform;
    protected  List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
    public List<Stone> saveStones = new List<Stone>();

    public virtual void Awake()     
    {
        _result = Resources.Load("NetworkNCMBResultBoard") as GameObject;
        _transform = GameObject.Find("Canvas").transform;
        InitBoard(GameVariable.othelloBoardNum);
        CreateNumbering();
        gameKind = ESceneKind.Othello;
    }
    private void Start() {
        SoundManager.instance.ChangeBGM(EBGMClipType.Othello);
        GameView.ShowFade(new GameFadeOption{isFade = false, limitedTime = 1f});
    }
    private void Update() {
        ChangeStoneNumber();
    }
    public void ChangeStoneNumber()
    {
        var w_count = saveStones.Where(e => e.stoneType == EPlayerType.White);
        var b_count = saveStones.Where(e => e.stoneType == EPlayerType.Black);
        whiteNumberText.text = w_count.Count().ToString();
        blackNumberText.text = b_count.Count().ToString();
    
    }
    public override bool AnalyzeBoard(int r, int c, int length = 0)
    {
        // 내가 둔 돌에 사방에 같은 돌이 한개라도 존재한다면 들어감
        // 고로 오델로는 이 방향을 피하면 됨.. 총 8방향이 존재한다
        // 2개인경우만 생각하자;; 
        // 검은돌 2개가 들어갔을때 나와 같은 돌을 만나지 않으면 그냥 반복문을 나가게 된다 
        // 결국 연속성이 깨진 상태이지만 리스트안에 들어가 있어서 생긴 이슈인것 같다
        int[] diraction = new int[checkDir.GetLength(0)];
        List<KeyValuePair<int,int>> value = new List<KeyValuePair<int, int>>();

        while(sequenceQ.Count > 0)
        {
            diraction[sequenceQ.Dequeue()] = 1;
        }
        for(int i = 0; i < diraction.Length; i++)
        {
            if(diraction[i] != 1)
            {
                for (int sr = r + checkDir[i, 0], sc = c + checkDir[i, 1];
                !CheckOverValue(sr, sc);
                sr += checkDir[i, 0], sc += checkDir[i, 1])
                {
                    if (GetBoardValue (sr, sc) == GetNextTurn()) // 1. 나의 돌과 다른 경우
                    {
                        value.Add(new KeyValuePair<int, int>(sr, sc));
                    }
                    else if(GetBoardValue(sr,sc) == GetTurn()) // 나의 돌과 같은 경우 .. 연속성이 깨졌다
                    {
                        // 여기서 경우의 수가 두개임
                        CheckDirectionPosition(value);
                        break;
                    }
                    else // 돌이 없는 경우
                    {
                        value.Clear();
                        break;
                    }
                }
                value.Clear(); // 2개 이상의 돌이 value에 남아 있을 수 있어서 clear를 한다
            }
        }
        if (list.Count <= 0) return false;
        else return true;
    }
    public override bool AnalyzeBoard(int r, int c, int _turn, int length = 0)
    {
        int[] diraction = new int[checkDir.GetLength(0)];
        List<KeyValuePair<int,int>> value = new List<KeyValuePair<int, int>>();

        while(sequenceQ.Count > 0)
        {
            diraction[sequenceQ.Dequeue()] = 1;
        }
        for(int i = 0; i < diraction.Length; i++)
        {
            if(diraction[i] != 1)
            {
                for (int sr = r + checkDir[i, 0], sc = c + checkDir[i, 1];
                !CheckOverValue(sr, sc);
                sr += checkDir[i, 0], sc += checkDir[i, 1])
                {
                    if (GetBoardValue (sr, sc) == GetTurn(_turn)) // 1. 나의 돌과 다른 경우
                    {
                        value.Add(new KeyValuePair<int, int>(sr, sc));
                    }
                    else if(GetBoardValue(sr,sc) == _turn) // 나의 돌과 같은 경우 .. 연속성이 깨졌다
                    {
                        // 여기서 경우의 수가 두개임
                        CheckDirectionPosition(value);
                        break;
                    }
                    else // 돌이 없는 경우
                    {
                        value.Clear();
                        break;
                    }
                }
                value.Clear(); // 2개 이상의 돌이 value에 남아 있을 수 있어서 clear를 한다
            }
        }
        if (list.Count <= 0) return false;
        else return true;
    }
    public bool ConditionGameOver()
    {
        return !CheckTransferTurn(GetTurn()) && !CheckTransferTurn(GetNextTurn());
    }
    public bool CheckTransferTurn(int _turn) // 이건 그냥 보조수단임 // 여기서 만들어야 하는구나~ㅋㅋㅋㅋㅋ 시발
    {
        // overvalue여선 안되며, 돌이 존재하는 위치에서 확인하면 안된다 그리고 row, col의 최대, 최솟값을 구하여 그 둘레부분을 탐색한다
        // 최소,최댓값을 구하기엔 너무 과분하여 그냥 전부 탐색으로 만들었다
        bool isTrue = false;
        for(int i = 0; i < GameVariable.othelloBoardNum; i++)
        {
            for(int j = 0; j < GameVariable.othelloBoardNum; j++)
            {
                if(IsEmpty(i,j))
                {
                    CheckDirection(i,j,_turn);
                    if(AnalyzeBoard(i,j,_turn,0))
                    {
                        ResetList();
                        isTrue = true;
                        break;
                    }
                }
            }
            if(isTrue) break;
        }
        return isTrue;
    }
    void CheckDirectionPosition(List<KeyValuePair<int,int>> _value)
    {
        list.AddRange(_value);
        _value.Clear();
    }
    
    public override void GameOver()
    {
        //string s = string.Format("Victory : {0}",whiteList.Count > blackList.Count ? "White" : whiteList.Count < blackList.Count ? "Black" : "Same");
        //Debug.Log(s);
        base.GameOver();

        var whiteCount = saveStones.Where(w => w.stoneType == EPlayerType.White);
        var blackCount = saveStones.Where(b => b.stoneType == EPlayerType.Black);
        
        if(whiteCount.Count() > blackCount.Count())
            score = (int)EPlayerType.White;
        else 
            score = (int)EPlayerType.Black;

        Instantiate(_result, _transform);
    }
    // 요게 문제
    // 솔직히 이거 왜 만들었을까 이럴거면
    public virtual void ChangeStone(EPlayerType type, int myTurn) // 이게 제일문제네 만들어 놓고 이거 솔직히 말해서 서로 반대라 매니저에서 해야 할 일인데
    {
        if (list.Count <= 0) return;
        /*
         * internal(내부의) <> external(외부의)
         * 1. board change internal(내부의)
         * 2. stone change external(외부의)
         */

        //(turn -1 == (int)EPlayerType.white) ? blackList : whiteList; // 서로 반대로 해야 됬는데;;

        foreach (var _list in list) // 1.이제 내부의 list걸린 모든 배열 값을 변경해 준다 
        {
            SetBoardValue(_list.Key, _list.Value, myTurn);
        }
        
        var enemys = saveStones.Where(e => GetBoardValue(e.m_row,e.m_col) == myTurn);
        
        foreach(var enemy in enemys)
        {
            enemy.stoneType = type == EPlayerType.White ? EPlayerType.White : EPlayerType.Black;
        }
    }
    public void CreateNumbering() // 이건 그냥 내가 pos를 계산해서 넣자,,
    {
        float interval = 0.9f;
        float constValue = 4f;
        float startRow = 3.15f;
        float startCol = -3.15f;
        // row
        for(int i = 0; i < GameVariable.othelloBoardNum; i++)
        {
            GameObject text = Instantiate(numbering,new Vector3(-constValue,startRow,0),Quaternion.identity);
            text.transform.SetParent(board.transform);
            text.GetComponent<Canvas>().sortingLayerName = "Obstacle";
            Text _text = text.transform.GetChild(0).GetComponent<Text>();
            _text.color = Color.white;
            _text.text = i.ToString();
            startRow -= interval;
        }
        // row
        for(int j = 0; j < GameVariable.othelloBoardNum; j++)
        {
            GameObject text = Instantiate(numbering,new Vector3(startCol, constValue,0),Quaternion.identity);
            text.transform.SetParent(board.transform);
            text.GetComponent<Canvas>().sortingLayerName = "Obstacle";
            Text _text = text.transform.GetChild(0).GetComponent<Text>();
            _text.color = Color.white;
            _text.text = j.ToString();
            startCol += interval;
        }
    }
    public void ResetList()
    {
        if(list.Count <= 0) return;
        list.Clear();        
    }
}
