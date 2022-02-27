using System.Collections;
using System.Collections.Generic;
using System;

// set을 잘함 계속 저장된 데이터를 꺼내고 저장 할 수 있음....
// set은 update임
[Serializable]
public class PlayerInfo
{
    public string uid;
    public string nickName;
    public long coin;
    Dictionary<string,object> scores = new Dictionary<string, object>();
    public void SetScore(string key, object value)
    {
        scores[key] = value;
    }
    public object GetScore(string key)
    {
        return scores[key];
    }
    public PlayerInfo(string uid)
    {
        this.uid = uid;
    }
    public Dictionary<string,object> ToDictionary()
    {
        scores.Add("uid",uid);
        scores.Add("nickName",nickName);
        scores.Add("coin",coin);
        return scores;
    }
    public string PlayerToJson()
    {
        PlayerInfo info = new PlayerInfo(this.uid);
        info.coin = this.coin;
        info.nickName = this.nickName;
        
        return UnityEngine.JsonUtility.ToJson(info);
    }
    
}
