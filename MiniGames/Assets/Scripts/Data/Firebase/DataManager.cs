using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

using Auth = Firebase.Auth.FirebaseAuth;
public class DataManager : MonoBehaviour
{
    public static DataManager instance = null;
    public PlayerInfo player = null;
    public CalculatorTime time;
    public readonly string tableName = "PlayerData";
    public readonly string c_Nick = "nickName";
    public readonly string c_Coin = "coin";
    public readonly string c_CoinTime = "coinTime";
    public readonly string c_LastTime = "lastPlayTime";
    public readonly long addCoinPerDelay = 60;
    public readonly long addPerCoin = 10;
    public readonly long maxCoin = 200;
    // construct default value
    public readonly long defaultScore = 0;
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
    }
    public async void Load()
    {   
        string uid = Auth.DefaultInstance.CurrentUser.UserId;
        if(player.uid == "" || player.uid == null)
        {
            player = new PlayerInfo(uid, SetValue());
            time = new CalculatorTime();
        }
        await player.LoadData();
        await SetLoadedCoin();
    }
    public Dictionary<string,object> SetValue(string nickName = null)
    {
        Dictionary<string,object> newDictionary = new Dictionary<string, object>();

        newDictionary.Add(c_Nick, nickName == null ? "" : nickName);
        newDictionary.Add(c_LastTime,DateTime.Now.ToString());
        newDictionary.Add(c_CoinTime,0);
        newDictionary.Add(c_Coin,maxCoin);

        for(int i = 0; i < SceneKind.sceneValue.Count; i++)
        {
            EGameKind kind = SceneKind.sceneValue[(ESceneKind)i].gameOptions.gameKind;
            EGamePlayType playType = SceneKind.sceneValue[(ESceneKind)i].gameOptions.gamePlayType;

            if(kind == EGameKind.BoardGame && playType == EGamePlayType.Multi)
                newDictionary.Add(SceneKind.sceneValue[(ESceneKind)i].sceneName,"0/0");
            else if(kind == EGameKind.None || playType == EGamePlayType.None) continue;
            else
                newDictionary.Add(SceneKind.sceneValue[(ESceneKind)i].sceneName,0);
        }

        return newDictionary;
    }
    public async Task SetLoadedCoin()
    {
        long lastPlayTime =  CalculatorTime.GetSecondTime(CalculatorTime.GetDateTime((string)player.GetPlayerData(c_LastTime)));
        long diffTime = (long)DateTime.Now.TimeOfDay.TotalSeconds - lastPlayTime;
        long totalTime = diffTime + (long)player.GetPlayerData(c_CoinTime);

        long sumCoin = player.coin + (totalTime / addCoinPerDelay * addPerCoin);
        Debug.Log($"lastPlayTime = {diffTime}");
        Debug.Log($"coinTime = {(long)player.GetPlayerData("coinTime")}");
        Debug.Log($"player coin = {player.coin}");
        Debug.Log($"totalTime = {totalTime}");
        Debug.Log($"sumCoin = {sumCoin}");

        if(sumCoin >= maxCoin)
        {
            player.coin = maxCoin;
            time.AddFrontTime(addCoinPerDelay);
        }
        else
        {
            player.coin += (totalTime / addCoinPerDelay * addPerCoin);
            if(player.coin >= maxCoin)
            {
                player.coin = maxCoin;
                time.AddFrontTime(addCoinPerDelay);
            }
            else
            {
                time.AddFrontTime(addCoinPerDelay - (totalTime % addCoinPerDelay));
            }
        }

        await player.ParticalSaveData<long>(c_Coin,player.coin);
        await player.ParticalSaveData<string>(c_LastTime,DateTime.Now.ToString());
    }
    public async void Create(string nickName)
    {
        PlayerInfo newPlayer = new PlayerInfo(Auth.DefaultInstance.CurrentUser.UserId, SetValue(nickName));
        await newPlayer.SaveData();
        // 이 다음 load를 진행
    }
    public async Task SetTimes() // 여기가 문제임;;
    {
        player.SetPlayerData(c_LastTime,DateTime.Now.ToString());

        if(player.coin >= maxCoin)
        {
            player.SetPlayerData(c_CoinTime,0);
        }
        else
        {
            if(time.DiffSecondTime() >= 0)
            {
                player.SetPlayerData(c_CoinTime,addCoinPerDelay - time.DiffSecondTime());
            }
            else
            {
                player.SetPlayerData(c_CoinTime,addCoinPerDelay);
            }
        }

        await player.SaveData();
    }
    // update(save)
    public async void UpdateColumn<T>(string columnName, T value) // 수정 필요없음 어차피 load에 걸린 체인때문에 자동변경됨!!
    {
        await player.ParticalSaveData<T>(columnName, value);
    }
    public void UpdateCoin(long value)
    {
        long coin = player.coin;
        coin += value;
        
        Task.Run(async () =>  await player.ParticalSaveData<long>(c_Coin, coin));
    }
    // logout
    public async void Logout()
    {
        await GameQuit();
        Auth.DefaultInstance.SignOut();
        GooglePlayGames.PlayGamesPlatform.Instance.SignOut();
    }
    public async Task GameQuit()
    {
        await SetTimes();
        time = null;
        player = null;
    }
    void OnApplicationQuit() {
        Task.Run(async() => await GameQuit());
    }
}
