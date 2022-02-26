using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LocalOmokManager : OmokManager
{
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject enemy;
    EnemyOmokPlayer _enemy;
    OmokPlayer _player;
    GameObject gameMenu;
    public int playerStoneNum;
    public int enemyStoneNum;
    public List<KeyValuePair<string,string>> priorityList = new List<KeyValuePair<string, string>>()
    { // 1 = i
        new KeyValuePair<string, string>("arrNum","i"), // top,,0
        new KeyValuePair<string, string>("0","i"), // bottom,,1
        new KeyValuePair<string, string>("i","arrNum"), // left,,2
        new KeyValuePair<string, string>("i","0"), // right,,3
        new KeyValuePair<string, string>("arrNum","arrNum - i"), // top-left,,4 .. 4
        new KeyValuePair<string, string>("arrNum - i","arrNum"), // top-left,,5
        new KeyValuePair<string, string>("arrNum - i","0"), // top-right,,6 .. 5
        new KeyValuePair<string, string>("arrNum","arrNum - i"), // top-right,,7 
        new KeyValuePair<string, string>("0","i"), // bottom-left,,8..6
        new KeyValuePair<string, string>("arrNum - i","arrNum"), // bottom-left,,9
        new KeyValuePair<string, string>("0","arrNum - i"), // bottom-right,,10.. 7
        new KeyValuePair<string, string>("i","0"), // bottom-right,,11
    };
    public override void Awake() {
        base.Awake();
        gameMenu = GameObject.Find("GameMenu");
        gameKind = ESceneKind.LocalOmok;
    }
    public void OnClickGameStartButton()
    {   
        playerStoneNum = gameMenu.transform.Find("StoneUI").GetComponent<StoneUI>().selectStone;
        enemyStoneNum = playerStoneNum == (int)EPlayerType.White ? (int)EPlayerType.Black : (int)EPlayerType.White;

        _player = Instantiate(player,new Vector3(0,0,0),Quaternion.identity).GetComponent<OmokPlayer>();
        _enemy = Instantiate(enemy,new Vector3(0,0,0),Quaternion.identity).GetComponent<EnemyOmokPlayer>();
        gameMenu.SetActive(false);
    }
    public void OnClickBackButton()
    {
         GameView.ShowFade(new GameFadeOption {
            isFade = true,
            limitedTime = 1f,
            sceneNum = SceneKind.sceneValue[ESceneKind.MainMenu].sceneNum
        });
    }
    public override void GameOver()
    {
        if(turn == 1)
        {
            score = playerStoneNum;
        }
        else
        {
            score = enemyStoneNum;
        }
        Debug.Log("GameOver");
        IsGameOver();
        _result = Resources.Load("LocalResultBoard") as GameObject;
        Instantiate(_result,_transform);
    }
    public void EnemyStart(int playType)
    {
        if(playType <= 0)
        {
            Debug.Log($"Enemy IsInit");
            InitPutStone(out _enemy.r,out _enemy.c, out _enemy.direction);
        }
        else if(playType <= 4) // 50퍼
        {
            Debug.Log($"Enemy IsAttack");
            SelectHaviour(ref _enemy.r,ref _enemy.c);
        }
        else
        {
            Debug.Log($"Enemy IsDefend");
            SelectHaviour(ref _player.r,ref _player.c,true);
        }
    }
    void InitPutStone(out int r, out int c,out int direction ) // player 방향중 하나 찾아서 두기 // 이건 무조건 enemy임
    {
        // init direction
        // player nearly stone
        var stone = FindObjectOfType<OmokStone>();
        direction = Random.Range(0,checkDir.GetLength(0));
        

        r = stone.m_row + checkDir[direction,0];
        c = stone.m_col + checkDir[direction,1];

        while ((r < 0 || r >= GameVariable.omokBoardNum) || (c < 0 || c >= GameVariable.omokBoardNum))
        {
            direction = (direction + 1) % checkDir.GetLength(0);

            r = stone.m_row + checkDir[direction,0];
            c = stone.m_col + checkDir[direction,1];
            Debug.Log($"init r = {r}, c = {c}");
        }
    }   
    public bool PrioritySearchStone()
    {
        int arrNum = GameVariable.omokBoardNum - 1;        
        int kind = 2; // 2==player   
       
        while(kind > 0)
        {
            int _turn = kind == 2 ? GetNextTurn() : GetTurn();
            int index = 0, dir = 0;
            int _r = 0,_c = 0;
            foreach(var list in priorityList)
            {
                for(int i = 0; i <= arrNum; i++)
                {
                    switch(list.Key)
                    {
                        case "0" : _r = 0; break;
                        case "i" : _r = i; break;
                        case "arrNum" : _r = arrNum; break;
                        case "arrNum - i" : _r = arrNum - i; break;
                        default : _r = -1; break;
                    }
                    switch(list.Value)
                    {
                        case "0" : _c = 0; break;
                        case "i" : _c = i; break;
                        case "arrNum" : _c = arrNum; break;
                        case "arrNum - i" : _c = arrNum - i; break;
                        default : _c = -1; break;
                    }
                    string s = dir == 0 ? "top" : dir == 1 ? "bottom" : dir == 2 ? "left" : dir == 3 ? "right" : dir == 4 ? "top-right"
                    : dir == 5 ? "top-left" : 
                    dir == 6 ? "bottom-left" : 
                    dir == 7 ? "bottom-right" : "Fail";

                    if(SequenceDangerousStone(_r,_c,ref _enemy.r, ref _enemy.c,_turn,dir))
                    {
                        Debug.Log($"5 stone search {(_turn != GetTurn() ? "Player turn " : "enemy turn")}  {s}");
                        return true;
                    }
                    else if(SequenceWarningStone(_r,_c,ref _enemy.r, ref _enemy.c,_turn,dir))
                    {
                        Debug.Log($"3 stone search {(_turn != GetTurn() ? "Player turn " : "enemy turn")}  {s}");
                        return true;
                    }
                }
                index++;
                dir++;
                if(index >= 4 && index <= 5) dir = 4;
                else if(index >= 6 && index <= 7) dir = 5;
                else if(index >= 8 && index <= 9) dir = 6;
                else if(index >= 10 && index <= 11) dir = 7;
            }
            kind--;
        }
        return false;
    }
    bool SequenceDangerousStone(int _sr,int _sc,ref int receiveR, ref int receiveC, int turn,int direction)
    {
        // 사방에 적 돌이 없고 비어있다면 막아야 하는 로직을 새로 생성해야 된다 우선순위는 제일 높다
        // 4칸도 막아진다 고로 3칸만 찾음된다
        int saveR = -1, saveC = -1;
        int startR = 0, startC = 0;        
        List<KeyValuePair<int,int>> saveList = new List<KeyValuePair<int, int>>();

        bool isFind = false;
        int count = 0, sequence = 0;
        for(int sr = _sr, sc = _sc; !CheckOverValue(sr,sc); 
        sr += checkDir[direction,0], sc += checkDir[direction,1])
        {
            if(GetBoardValue(sr,sc) == 0)
                saveList.Add(new KeyValuePair<int, int>(sr,sc));
            if(count == 0)
            {
                startR = sr; startC = sc;
            }
            if(count >= setOverNum)
            {
                foreach(var save in saveList)
                {
                    SetBoardValue(save.Key,save.Value,turn);

                    for(int i = startR, j = startC; !CheckOverValue(i,j);
                    i += checkDir[direction,0], j += checkDir[direction,1])
                    {
                        if(GetBoardValue(i,j) == turn)
                        {
                            sequence++;
                            if(sequence >= setOverNum)
                            {
                                Debug.Log("True");
                                Debug.Log($"true saveR = {save.Key}, saveC = {save.Value}, turn = {turn}");
                                saveR = save.Key;
                                saveC = save.Value;
                                isFind = true;
                            }
                        }
                        else sequence = 0;
                    }
                    SetBoardValue(save.Key,save.Value,0);
                    if(isFind)  // 이것도 나중에 바꾸자
                    {
                        break;
                    }
                }
                count = 0;
                saveList.Clear();
            }            
            count++;
        }
        if(isFind)
        {
            receiveR = saveR;
            receiveC = saveC;
            return true;
        }
        else return false;
    }
    bool SequenceWarningStone (int _sr,int _sc, ref int receiveR, ref int receiveC,int turn,int direction)
    {
        int sequence = 0;
        int backR = _sr ,backC = _sc, frontR = 0, frontC = 0;

        for(int sr = _sr, sc = _sc; 
        !CheckOverValue(sr,sc);
        sr += checkDir[direction,0], sc += checkDir[direction,1])
        {
            if(GetBoardValue(sr,sc) == turn)
            {
                sequence++; // 내가 포함이 되어지지 않아서 이렇구나!!
                if(sequence >= 3) // 앞뒤중 하나라도 막혀있음 false로 전환되어야 함
                {
                    backR = sr - (checkDir[direction,0] * sequence);
                    backC = sc - (checkDir[direction,1] * sequence);
                    frontR = sr + checkDir[direction,0];
                    frontC = sc + checkDir[direction,1];
                    
                    if(!CheckOverValue(backR ,backC) && !CheckOverValue(frontR,frontC))
                    {
                        if(IsEmpty(backR,backC) && IsEmpty(frontR,frontC))
                        {
                            Debug.Log($"true backR = {backR}, backC = {backC}, turn = {turn}");
                            Debug.Log($"true frontR = {frontR}, frontC = {frontC}, turn = {turn}");
                            Debug.Log("2.True");
                            receiveR = frontR;
                            receiveC = frontC;
                            Debug.Log($"true saveR = {frontR}, saveC = {frontC}, turn = {turn}");
                            return true;
                        }
                    }
                }
            }
            else sequence = 0;
        }
        return false;
    }
    void SelectHaviour(ref int _r, ref int _c, bool isPlayer = false)
    {
        var stones = FindObjectsOfType<OmokStone>().ToList();
        List<int> storageDirections = new List<int>();

        int defendDirection = 0;
        if(isPlayer) defendDirection = Random.Range(0,checkDir.GetLength(0)); //defend

        for(int i = 0; i < checkDir.GetLength(0);i++) //방향 엄선
        {
            int tempR = _r; int tempC = _c;

            tempR += checkDir[i,0];
            tempC += checkDir[i,1];

            // 음수와 오목 max값을 걸러낸다
            if(!CheckOverValue(tempR,tempC)) 
                storageDirections.Add(i);
        }

        Queue<int> removeQueue = new Queue<int>();

        for(int i = 0; i < storageDirections.Count; i++) // 동일한 값 제외
        {
            int index = storageDirections[i];
            
            foreach(var stone in stones)
            {
                if(stone.m_row  == _r + checkDir[index,0]  // 이게 문제인데;;
                && stone.m_col == _c + checkDir[index,1])
                {
                    removeQueue.Enqueue(index); // 값을 너야 되는데 index를 넣어서 생긴 이슈같음..
                }
            }
        }
        while (removeQueue.Count > 0)
        {
            int index = removeQueue.Dequeue();
            storageDirections.Remove(index);
        }

        if(storageDirections.Count > 0)
        {
            if(isPlayer) // defend
            {
                if(!storageDirections.Contains(defendDirection)) // 방향을 엄선 // defend
                {
                    defendDirection = storageDirections[Random.Range(0,storageDirections.Count)];
                }
                _enemy.r = _r + checkDir[defendDirection,0];
                _enemy.c = _c + checkDir[defendDirection,1];
                Debug.Log($"defend r = {_enemy.r}, c = {_enemy.c}"); // enemy r,c가 바뀌어야 하는데 player의 r,c가 바뀌었다 이건 캐치 못했다
            }
            else
            {
                if(!storageDirections.Contains(_enemy.direction)) // 방향을 엄선 // attack
                {
                    _enemy.direction = storageDirections[Random.Range(0,storageDirections.Count)];
                }
                _r = _r + checkDir[_enemy.direction,0];
                _c = _c + checkDir[_enemy.direction,1];
            }
        }
        else 
        {
            Debug.Log("ReStart!!");
            int rnd = Random.Range(1,10);
            EnemyStart(rnd);
        }
    }
}
