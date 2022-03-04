using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Firebase.Database;
using System;


[Serializable]
public class FirebasePlayerInfo
{
    public string nickName;
    public string lastPlayTime;
    public long coinTime;
    public long coin;
    string[] keyValue = new string[] // 나중에 안정화 되었을 때 dictionary로 관리하자
    {
        "nickName", "lastPlayTime", "coinTime", "coin"
    };
    Dictionary<string,object> scores = new Dictionary<string, object>();
    // const coin key
    public readonly string ADDCOIN = "addCoin";
    public readonly string ADDCOINPERDELAY = "addCoinPerDelay";
    public readonly string MAXCOIN = "maxCoin";
    public readonly string TABLENAME = "PlayerData";
    public readonly long addCoinPerDelay = 60;
    public readonly long addPerCoin = 10;
    public readonly long maxCoin = 200;
    public readonly long defaultScore = 0;
    public FirebasePlayerInfo(){}
    public FirebasePlayerInfo(string nickName, string lastPlayTime, long coinTime, long coin, Dictionary<string,object> scores = null)
    {
        if(scores != null)
        {
            for(int i = 0; i < SceneKind.sceneValue.Count; i++)
            {
                EGameKind kind = SceneKind.sceneValue[(ESceneKind)i].gameOptions.gameKind;
                EGamePlayType playType = SceneKind.sceneValue[(ESceneKind)i].gameOptions.gamePlayType;

                if(kind == EGameKind.BoardGame && playType == EGamePlayType.Multi)
                    scores[SceneKind.sceneValue[(ESceneKind)i].sceneName] = "0/0"; 
                else if(kind == EGameKind.None || playType == EGamePlayType.None) continue;
                else
                    scores[SceneKind.sceneValue[(ESceneKind)i].sceneName] = 0; 
            }
        }
        this.nickName = nickName;
        this.lastPlayTime = lastPlayTime;
        this.coinTime = coinTime;
        this.coin = coin;
    }
    
    // id
    public string GetReplacedId(string email)
    {
        string _id = email.Replace('@',' ');
        _id = _id.Replace('.',' ');
        return _id;
    }
    // Firebase Set Create Id // 아 이게 애초에 set이구나!!
    public void CreatePlayer(string email, string nickName)
    {
        DatabaseReference reference =  FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child(TABLENAME).Child(GetReplacedId(email)).Child("nickName").SetValueAsync(nickName);
        reference.Child(TABLENAME).Child(GetReplacedId(email)).Child("lastPlayTime").SetValueAsync(DateTime.Now.ToBinary().ToString());
        reference.Child(TABLENAME).Child(GetReplacedId(email)).Child("coinTime").SetValueAsync(addCoinPerDelay);
        reference.Child(TABLENAME).Child(GetReplacedId(email)).Child("coin").SetValueAsync(maxCoin);

        for(int i = 0; i < SceneKind.sceneValue.Count; i++)
        {
            EGameKind kind = SceneKind.sceneValue[(ESceneKind)i].gameOptions.gameKind;
            EGamePlayType playType = SceneKind.sceneValue[(ESceneKind)i].gameOptions.gamePlayType;

            if(kind == EGameKind.BoardGame && playType == EGamePlayType.Multi)
                scores[SceneKind.sceneValue[(ESceneKind)i].sceneName] = "0/0"; 
            else if(kind == EGameKind.None || playType == EGamePlayType.None) continue;
            else
                scores[SceneKind.sceneValue[(ESceneKind)i].sceneName] = 0; 
        }
    }

    // ToDictionary and FirebasePlayerInfo
    public Dictionary<string,object> PlayerToDictionary<TKey,TValue>()
    {
        // 복사전에 나의 dictionary에 넣기
        scores.Add(nickName,this.nickName);
        scores.Add(lastPlayTime,this.lastPlayTime);
        scores.Add("coinTime",this.coinTime);
        scores.Add("coin",this.coin);

        var copy = scores.ToDictionary(entry => entry.Key, entry => entry.Value);
        
        return copy;
    }
    public string PlayerToJson()
    {
        FirebasePlayerInfo newPlayer = new FirebasePlayerInfo(this.nickName,this.lastPlayTime,this.coinTime,this.coin,this.scores);
        return UnityEngine.JsonUtility.ToJson(newPlayer);
    }

    // Firebase Set
    public void UpdateFirebaseDatabase<T>(string email, string columnName, T value)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child(TABLENAME).Child(GetReplacedId(email)).Child(columnName).SetValueAsync(value);
    }
    // Firebase EnterPlayer Get await
    void SetMyPlayerInfo(string key, object value)
    {
        switch(key)
        {
            case "nickName" : nickName = (string)value; break;
            case "lastPlayTime" : lastPlayTime = (string)value; break;
            case "coinTime" : coinTime = (long)value; break;
            case "coin" : coin = (long)value; break;
        }
    }
    public async void GetPlayer(string email)
    {
        await GetFirebaseDatabase(email);
        EnterPlayer(email);
    }
    async Task GetFirebaseDatabase(string email)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(TABLENAME);
        await reference.Child(GetReplacedId(email)).GetValueAsync().ContinueWith(task => 
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
                    SetMyPlayerInfo(data.Key,data.Value);
                }
            }
        });
    }

    // Firebase DateTime
    public DateTime GetLastPlayDateTime()
    {
        return DateTime.FromBinary(Convert.ToInt64(lastPlayTime));
    }
    public long GetLastPlayBinaryTime()
    {
        return Convert.ToInt64(lastPlayTime);
    }

    // Enter Player Time Diff or Set coin
    public void EnterPlayer(string email)
    {
        long diffTime = (long)DateTime.Now.Subtract(GetLastPlayDateTime()).TotalSeconds;
        long receiveCoin = diffTime / addCoinPerDelay * addPerCoin;
        long nmgTime = diffTime % addCoinPerDelay;

        if(coin + receiveCoin >= maxCoin)
        {
            coin = maxCoin;
            coinTime = addCoinPerDelay;
        }
        else
        {
            coin += receiveCoin;
            coinTime -= nmgTime;
        }
        UpdateFirebaseDatabase<long>(email,"coin",coin);
        UpdateFirebaseDatabase<long>(email,"coinTime",coinTime);
    }
}
