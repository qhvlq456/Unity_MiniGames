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
    bool isLoading = true;
    DateTime coinDelayTime; // coinDelay를 더한 시간
    DateTime currentTime; // 현재 시간
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
                    scores.Add(SceneKind.sceneValue[(ESceneKind)i].sceneName,"0/0");
                else if(kind == EGameKind.None || playType == EGamePlayType.None) continue;
                else
                    scores.Add(SceneKind.sceneValue[(ESceneKind)i].sceneName,defaultScore);
            }
        }
        this.nickName = nickName;
        this.lastPlayTime = lastPlayTime;
        this.coinTime = coinTime;
        this.coin = coin;
    }
    public void IsLoading(bool value)
    {
        isLoading = value;
    }
    // id
    public string GetReplacedId(string email)
    {
        string _id = email.Replace('@',' ');
        _id = _id.Replace('.',' ');
        return _id;
    }
    // Set Dictionary Scores
    public void SetScore(string key, object value)
    {
        scores[key] = value;
    }
    public object GetScore(string key)
    {
        return scores[key];
    }
    // Firebase Set Create Id 
    public void CreatePlayer(string email, string nickName)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        this.nickName = nickName;
        this.lastPlayTime = DateTime.Now.ToString();
        this.coinTime = 0;
        this.coin = maxCoin;

        for(int i = 0; i < SceneKind.sceneValue.Count; i++)
        {
            EGameKind kind = SceneKind.sceneValue[(ESceneKind)i].gameOptions.gameKind;
            EGamePlayType playType = SceneKind.sceneValue[(ESceneKind)i].gameOptions.gamePlayType;

            if(kind == EGameKind.BoardGame && playType == EGamePlayType.Multi)
                scores.Add(SceneKind.sceneValue[(ESceneKind)i].sceneName,"0/0");
            else if(kind == EGameKind.None || playType == EGamePlayType.None) continue;
            else
                scores.Add(SceneKind.sceneValue[(ESceneKind)i].sceneName,defaultScore);
        }
        reference.Child(TABLENAME).Child(GetReplacedId(email)).UpdateChildrenAsync(PlayerToDictionary());
    }

    // ToDictionary and FirebasePlayerInfo
    public Dictionary<string,object> PlayerToDictionary()
    {
        Dictionary<string, object> copy = new Dictionary<string, object>();
        // 복사전에 나의 dictionary에 넣기
        scores.Add("nickName",this.nickName);
        scores.Add("lastPlayTime",this.lastPlayTime);
        scores.Add("coinTime",this.coinTime);
        scores.Add("coin",this.coin);

        foreach(var data in scores)
        {
            // UnityEngine.Debug.Log($"key = {data.Key}, value = {data.Value}");
            copy.Add(data.Key,data.Value);
        }
        
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
    void SetPlayerInfo(string key, object value)
    {
        switch(key)
        {
            case "nickName" : nickName = (string)value; break;
            case "lastPlayTime" : lastPlayTime = (string)value; break;
            case "coinTime" : coinTime = (long)value; break;
            case "coin" : coin = (long)value; break;
        }
    }
    public void GetPlayer(string email)
    {
        UnityEngine.Debug.Log("await Start");
        GetFirebaseDatabase(email);
    }
    async void GetFirebaseDatabase(string email) // 그냥 모든 값을 나에게 set한다고 생각하자 근데 update되고 자동으로 해도 되는거잖아;;
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
                    // UnityEngine.Debug.Log($"Key = {data.Key}, Value = {data.Value}");
                    SetPlayerInfo(data.Key,data.Value);
                }
            }
        });
        EnterPlayer(email);
    }

    // Firebase DateTime
    public long CalculatorLastPlayTime() // string -> long으로 변환 왜냐 계산 해야됨;;
    {
        return DateTime.Parse(lastPlayTime).ToBinary();
    }
    public DateTime GetLastPlayDateTime() // 이거 바꿔야 됨!! 이 밑으로 전부~
    {
        return DateTime.FromBinary(CalculatorLastPlayTime());
    }
    // Set CoinTime
    public void SetCoinTime(string email, string columnName){
        long time = addCoinPerDelay - (long)coinDelayTime.Subtract(DateTime.Now).TotalSeconds; // 이거 때문이구나;;
        UpdateFirebaseDatabase<long>(email,columnName,time);
    }
    // Enter Player Time Diff or Set coin
    public void EnterPlayer(string email) // 근데 이건 꼭 해야됨
    {
        long diffTime = (long)DateTime.Now.Subtract(GetLastPlayDateTime()).TotalSeconds;
        // UnityEngine.Debug.Log($"diffTime = {diffTime}");
        long receiveCoin = diffTime / addCoinPerDelay * addPerCoin;
        long nmgTime = diffTime % addCoinPerDelay;

        if(receiveCoin + coin >= maxCoin)
        {
            coin = maxCoin;
        }
        else
        {
            coin += (receiveCoin * addPerCoin);
        }

        SetCoinDelayTime(email, nmgTime);
    }
    void SetCoinDelayTime(string email, long nmgTime)
    {
        long totalTime = coinTime + nmgTime;
        UnityEngine.Debug.Log($"coinTime Time = {coinTime}");
        UnityEngine.Debug.Log($"nmgTime Time = {nmgTime}");
        UnityEngine.Debug.Log($"Total Time = {totalTime}");
        if(totalTime >= addCoinPerDelay) // ex > 25 - 15 // 10 - 50 // ㅁㅊ nmgTime이 coinTime보다 크면 코인을 하나 더 줘야한다 ㅋ
        {
            if(coin + addPerCoin >= maxCoin)
            {
                coin = maxCoin;
                totalTime = addCoinPerDelay;
            }
            else
            {
                coin += (totalTime /= addCoinPerDelay) * addPerCoin;
                totalTime %= addCoinPerDelay;
            }
        }

        coinDelayTime = DateTime.Now.AddSeconds(addCoinPerDelay - totalTime);
        UpdateFirebaseDatabase<long>(email,"coin",coin);
        isLoading = false;
    }
    // UpdateCoin
    public void UpdateCoin(UnityEngine.UI.Text currentPlayTimeText = null, Action callback = null)
    {
        if(isLoading) return;
        if(coin >= maxCoin) 
        {
            ResetCoinDelayTime();
            return;
        }

        currentPlayTimeText.text = TimeSpan.FromSeconds((coinDelayTime - DateTime.Now).TotalSeconds).ToString("mm':'ss");

        if(coinDelayTime.Subtract(DateTime.Now).TotalSeconds <= 0)
        {
            ResetCoinDelayTime();
            if(callback != null)
                callback.Invoke();

            UnityEngine.Debug.Log("SameTime");
        }
    }
    // ResetCoinDelayTime
    public void ResetCoinDelayTime()
    {
        coinDelayTime = DateTime.Now.AddSeconds(addCoinPerDelay);
    }
}
