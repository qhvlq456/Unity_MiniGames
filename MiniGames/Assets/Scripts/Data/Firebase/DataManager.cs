using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
public class DataManager : MonoBehaviour
{
    public static DataManager instance = null;
    public TestPlayer player;
    public DateTime frontTime;
    public FirebaseAuth auth;
    public readonly string TABLENAME = "PlayerData";
    public readonly string ADDCOIN = "addCoin";
    public readonly string ADDCOINPERDELAY = "addCoinPerDelay";
    public readonly string MAXCOIN = "maxCoin";
    public readonly long addCoinPerDelay = 60;
    public readonly long addPerCoin = 10;
    public readonly long maxCoin = 200;
    public readonly long defaultScore = 0;
    
    // construct default value
    private void Awake() {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        auth = FirebaseAuth.DefaultInstance;
    }
    // load
    public async void Load()
    {
        player = new TestPlayer(auth.CurrentUser.Email);
        await player.LoadData();

        // await LoadCoin();
    }
    public async Task LoadCoin()
    {
        DateTime lastPlayTime = DateTime.Parse((string)player.GetPlayerData("lastPlayTime"));
        long diffTime = (long)DateTime.Now.Subtract(lastPlayTime).TotalSeconds;

        long totalTime = diffTime + (long)player.GetPlayerData("coinTime");

        long addCoin = player.coin + (totalTime / addCoinPerDelay * addPerCoin);

        Debug.Log($"diffTime = {diffTime}");
        Debug.Log($"coinTime = {(long)player.GetPlayerData("coinTime")}");
        Debug.Log($"player coin = {player.coin}");
        Debug.Log($"totalTime = {totalTime}");
        Debug.Log($"sumCoin = {addCoin}");

        if(addCoin >= maxCoin)
        {
            player.coin = maxCoin;
            frontTime = DateTime.Now.AddSeconds(addCoinPerDelay);
        }
        else
        {
            player.coin += (totalTime / addCoinPerDelay * addPerCoin);
            if(player.coin >= maxCoin)
            {
                player.coin = maxCoin;
                frontTime = DateTime.Now.AddSeconds(addCoinPerDelay);
            }
            else
            {
                frontTime = DateTime.Now.AddSeconds(addCoinPerDelay - (totalTime % addCoinPerDelay));
            }
        }

        await player.ParticalSaveData<long>("coin",player.coin);
        await player.ParticalSaveData<string>("lastPlayTime",DateTime.Now.ToString());
    }
    public string VisibleCoinTime()
    {
        return TimeSpan.FromSeconds(LeftCoinTime()).ToString("mm':'ss");
    }
    public long LeftCoinTime()
    {
        return (long)frontTime.Subtract(DateTime.Now).TotalSeconds;
    }
    public void ResetCoinTime()
    {
        frontTime = DateTime.Now.AddSeconds(addCoinPerDelay);
    }
    // create
    public async void Create(string nickName)
    {
        TestPlayer newPlayer = new TestPlayer(auth.CurrentUser.Email,SetValue(nickName));
        await newPlayer.SaveData();
        // 이 다음 load를 진행
    }
    public async Task SetTimes() // 이거 해결해야 함 null reference // 저장하고 나가야됨;;ㅋㅋ
    {
        player.SetPlayerData("lastPlayTime",DateTime.Now.ToString());

        long diffTime = LeftCoinTime();
        if(player.coin >= maxCoin)
        {
            player.SetPlayerData("coinTime",0);
        }
        else
        {
            if(diffTime >= 0)
            {
                player.SetPlayerData("coinTime",addCoinPerDelay - diffTime);
            }
            else
            {
                player.SetPlayerData("coinTime",addCoinPerDelay);
            }
        }

        await player.SaveData();
    }
    // sign out
    public void Logout()
    {
        Quit();
        auth.SignOut();
    }
    public async void Quit() // 여기서 save작업 완료 해야함
    {
        await SetTimes();
        player = null;
        // testTime = null;
    }
    public async void UpdateCoin(long value)
    {
        player.coin += value;
        
        await player.ParticalSaveData<long>("coin",player.coin);
    }
    // test
    public Dictionary<string,object> SetValue(string nickName = null)
    {
        Dictionary<string,object> gameData = new Dictionary<string, object>();

        gameData.Add("nickName", nickName == null ? "" : nickName);
        gameData.Add("lastPlayTime",DateTime.Now.ToString());
        gameData.Add("coinTime",0);
        gameData.Add("coin",maxCoin);

        for(int i = 0; i < SceneKind.sceneValue.Count; i++)
        {
            EGameKind kind = SceneKind.sceneValue[(ESceneKind)i].gameOptions.gameKind;
            EGamePlayType playType = SceneKind.sceneValue[(ESceneKind)i].gameOptions.gamePlayType;
            if(kind == EGameKind.BoardGame && playType == EGamePlayType.Multi)
                gameData.Add(SceneKind.sceneValue[(ESceneKind)i].sceneName,"0/0");
            else if(kind == EGameKind.None || playType == EGamePlayType.None) continue;
            else
                gameData.Add(SceneKind.sceneValue[(ESceneKind)i].sceneName,defaultScore);
        }

        return gameData;
    }
}
