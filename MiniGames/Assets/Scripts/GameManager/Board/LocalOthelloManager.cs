using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalOthelloManager : OthelloManager
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
    public List<KeyValuePair<int,int>> enemyLocationList = new List<KeyValuePair<int, int>>();
    public override void Awake() {
        base.Awake();
        _result = Resources.Load("LocalResultBoard") as GameObject;
        gameMenu = GameObject.Find("GameMenu");
        gameKind = ESceneKind.LocalOthello;
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
    public void EnemySearchStone() // base??
    {
        for(int i = 0; i < GameVariable.othelloBoardNum; i++)
        {
            for(int j = 0; j < GameVariable.othelloBoardNum; j++)
            {
                if(IsEmpty(i,j))
                {
                    CheckDirection(i,j,GetTurn());
                    if(AnalyzeBoard(i,j,GetTurn()))
                    {
                        enemyLocationList.Add(new KeyValuePair<int, int>(i,j));
                    }
                }
            }
        }
    }
    public KeyValuePair<int,int> randomLocation() // 하... 너무 대충 만들었는뎈ㅋㅋ 또 탐색하는 거나 만들까;;
    {
        int random = Random.Range(0,enemyLocationList.Count);
        return enemyLocationList[random];
    }
    // 새로운 루틴
    // 1. 적이 돌을 둘수 있는지 탐색을 진행한다
    // 2. 돌을 둘 수 있는 리스트에서 제일 많이 변경 할 수 있는 리스트 인덱스를 다른 배열의 인덱스로 넣는다
    // 3. 랜덤값을 돌려 순위에 당첨된 리스트 값을 좌표로 리턴킨다

    public KeyValuePair<int,int> MuchChangeStoneLocation()
    {
        List<KeyValuePair<int,int>> rankList = new List<KeyValuePair<int, int>>(); // count, enemyLocationList index
        int index = 0;
        for(int i = 0; i < GameVariable.othelloBoardNum; i++)
        {
            for(int j = 0; j < GameVariable.othelloBoardNum; j++)
            {
                if(IsEmpty(i,j))
                {
                    CheckDirection(i,j,GetTurn());
                    if(AnalyzeBoard(i,j,GetTurn()))
                    {
                        enemyLocationList.Add(new KeyValuePair<int, int>(i,j));
                        rankList.Add(new KeyValuePair<int, int>(list.Count,index));
                        index++;
                        ResetList();
                    }
                }
            }
        }
        rankList.Sort((a,b) => a.Key > b.Key ? -1 : 1); // key기준으로 내림차순 -1이 내림차순이였네...

        int min = Random.Range(0,rankList.Count); // 난이도 조절
        int max = Random.Range(min,rankList.Count); // 난이도 조절

        int random = Random.Range(min,max); // 몇번째로 큰수를 정하기 위한 인덱스
        //Debug.Log($"{random} 번째로 큰 값");
        // 0 1 2 3 // 근데 중요한거 내가 필요한것은 인덱스일 뿐         
        // 2 3 1 5 << 이걸 탐색해서 만약 랜덤이 4가 나온다면 4번째로 큰수 1이 나와야 되는건데,, 

        /*
            index 0 1 2 3 4
            key   2 3 1 5 4
            value 0 1 2 3 4

            index 0 1 2 3 4
            key   5 4 3 2 1
            value 3 4 1 0 2
        */
        KeyValuePair<int,int> returnValue = enemyLocationList[rankList[random].Value];//new KeyValuePair<int, int>();
        // foreach(var value in rankList)
        // {
        //     Debug.Log($"enemy list value r = {enemyLocationList[value.Value].Key}, c = {enemyLocationList[value.Value].Value}, count = {value.Key}");
        //     // if(value.Value == 0)
        //     // {
        //     //     returnValue = enemyLocationList[value.Value];
        //     //     //break;
        //     // }
        // }
        // enemyLocationList[rankList[0].Value];
        enemyLocationList.Clear();
        CheckDirection(returnValue.Key,returnValue.Value,GetTurn()); // 아 이거 또 하는게 너 무 싫 다 
        AnalyzeBoard(returnValue.Key,returnValue.Value); // 아 이거 또 하는게 너 무 싫 다 
        //Debug.Log($"index r = {returnValue.Key}, c = {returnValue.Value}");
        return returnValue;
        // 이건 또
    }
    
}
