using System.Collections;
using Firebase.Database;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;

// set을 잘함 계속 저장된 데이터를 꺼내고 저장 할 수 있음....
// set은 update임
[Serializable]
public class PlayerInfo
{
    public string id {set; private get;}
    public string nickName;
    public string lastPlayTime;
    public long coinTime;
    public long coin;
    // coin balance
    public readonly long addPerCoin = 10;
    public readonly long addCoinPerDelay = 60;
    public readonly long maxCoin = 200;
    Dictionary<string,object> scores = new Dictionary<string, object>();
    public string GetOriginId()
    {
        return id;
    }
    public string GetReplaceId()
    {
        string _id = id.Replace('@',' ');
        _id = _id.Replace('.',' ');

        return _id;
    }
    public void SetScore(string key, object value)
    {
        scores[key] = value;
    }
    public object GetScore(string key)
    {
        return scores[key];
    }
    public PlayerInfo(string id)
    {
        // maxCoinTime = (maxCoin / addPerCoin) * addCoinPerDelay;
        this.id = id;
    }
    public Dictionary<string,object> ToDictionary()
    {
        scores.Add("nickName",nickName);
        scores.Add("coin",coin);
        scores.Add("lastPlayTime",lastPlayTime);
        scores.Add("coinTime",coinTime);

        return scores;
    }
    public string PlayerToJson()
    {
        PlayerInfo info = new PlayerInfo(this.id);
        info.coin = this.coin;
        info.nickName = this.nickName;
        info.lastPlayTime = this.lastPlayTime;
        info.coinTime = this.coinTime;

        return UnityEngine.JsonUtility.ToJson(info);
    }

    // Time Default
    public void SetLastPlayTime()
    {
        lastPlayTime = DateTime.Now.ToBinary().ToString();
    }
    public DateTime GetLastPlayTime()
    {
        return DateTime.FromBinary(Convert.ToInt64(lastPlayTime));
    }
    // coinTime은 
    // 어차피 그냥 set임 coinTime은 addPerCoinDelay값에서 계속 낮아지는 거임 그 상태로 Set하면 됨..
    public void GetCoinTime() // 먼저 coinTime을 가져왔다 생각하는 거임
    {
        UnityEngine.Debug.Log("getcoin");
        long diffTime = (long)DateTime.Now.Subtract(GetLastPlayTime()).TotalSeconds; // 들어온 시간과 나머지 시간의 차이
        long receiveCoin = diffTime / addCoinPerDelay * addPerCoin; //  코인 변경
        long nmgTime = diffTime % addCoinPerDelay; // 코인 변경 후 남는 시간

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
    }
    
}
