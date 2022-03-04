using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Auth;
public class DataManager : MonoBehaviour
{
    public static DataManager instance = null;
    public PlayerInfo player;
    public FirebaseAuth auth;
    DatabaseReference reference;
    public readonly string tableName = "PlayerData";
    public readonly string c_Id = "Id";
    public readonly string c_Nick = "nickName";
    public readonly string c_Coin = "coin";
    public readonly string c_LastPlayTime = "lastPlayTime";
    public readonly string c_CoinTime = "coinTime";
    // construct default value
    const long defaultScore = 0;

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

        reference = FirebaseDatabase.DefaultInstance.GetReference(tableName);
        auth = FirebaseAuth.DefaultInstance;
    }
    // load
    void OnLoadChanged(object sender, ValueChangedEventArgs args) // 어차피 모든 데이터 load되면 좋잖아;;
    {
        Debug.Log("Call OnLoadChanged!!");
        string id = player.GetReplaceId();

        player.nickName = args.Snapshot.Child(id).Child(c_Nick).GetValue(true).ToString();
        player.coin = (long)args.Snapshot.Child(id).Child(c_Coin).GetValue(true);

        for(int i = 0; i < SceneKind.sceneValue.Count; i++)
        {
            if(SceneKind.sceneValue[(ESceneKind)i].gameOptions.gameKind == EGameKind.None) continue;

            player.SetScore(SceneKind.sceneValue[(ESceneKind)i].sceneName,
            args.Snapshot.Child(id).Child(SceneKind.sceneValue[(ESceneKind)i].sceneName).GetValue(true));
        }
    }
    public async Task SetTimes()
    {
        await reference.Child(player.GetReplaceId()).GetValueAsync().ContinueWith(task => 
        {
            if(task.IsFaulted)
            {
                return;
            }
            if(task.IsCanceled)
            {
                return;
            }
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach(var data in snapshot.Children)
                {
                    if(data.Key == (string)c_LastPlayTime)
                    {
                        player.lastPlayTime = (string)data.Value;
                    }
                    if(data.Key == (string)c_CoinTime)
                    {
                        player.coinTime = (long)data.Value;
                    }
                }
            }
            Debug.Log("end find times");
        });
    }
    public async void Load() // 여기서 코인 진행
    {   
        player = new PlayerInfo(auth.CurrentUser.Email);
        reference.ValueChanged += OnLoadChanged;

        await SetTimes();
        player.GetCoinTime();
        UnityEngine.Debug.Log("end");
    }
    // update(save)
    public void UpdateColumn<T>(string columnName, T value) // 수정 필요없음 어차피 load에 걸린 체인때문에 자동변경됨!!
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        reference.Child(tableName).Child(player.GetReplaceId()).Child(columnName).SetValueAsync(value);
    }
    public void SetTimesss()
    {
        UpdateColumn<long>(c_CoinTime,player.coinTime);
        player.SetLastPlayTime();
        UpdateColumn<string>(c_LastPlayTime,player.lastPlayTime);
        player.GetCoinTime();
    }
    
    public void UpdateCoin(long value) // 파기 
    {
        if(player.coin + value >= player.maxCoin)
        {
            player.coin = player.maxCoin;
        }
        else
            player.coin += value;
        
        UpdateColumn<long>(c_Coin,player.coin);
    }
    
    public void Create(string _name) // create class => to json => save database .. create class => to dictionary => save database
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        PlayerInfo newPlayer = new PlayerInfo(auth.CurrentUser.Email);
        newPlayer.coin = newPlayer.maxCoin;
        newPlayer.nickName = _name;
        newPlayer.SetLastPlayTime();
        newPlayer.coinTime = newPlayer.addCoinPerDelay;

        for(int i = 0; i < SceneKind.sceneValue.Count; i++)
        {
            EGameKind kind = SceneKind.sceneValue[(ESceneKind)i].gameOptions.gameKind;
            EGamePlayType playType = SceneKind.sceneValue[(ESceneKind)i].gameOptions.gamePlayType;

            if(kind == EGameKind.BoardGame && playType == EGamePlayType.Multi)
                newPlayer.SetScore(SceneKind.sceneValue[(ESceneKind)i].sceneName,"0/0");
            else if(kind == EGameKind.None) continue;
            else
                newPlayer.SetScore(SceneKind.sceneValue[(ESceneKind)i].sceneName,0);
        }
        reference.Child(tableName).Child(newPlayer.GetReplaceId()).UpdateChildrenAsync(newPlayer.ToDictionary());
    }
    // logout
    public void Logout()
    {
        GameQuit();
        auth.SignOut();
    }
    public void GameQuit()
    {
        SetTimesss();
        reference.ValueChanged -= OnLoadChanged;
        player = null;
    }
}
