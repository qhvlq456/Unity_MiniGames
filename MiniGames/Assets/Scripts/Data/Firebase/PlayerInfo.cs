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
    public string id;
    public string nickName;
    public string lastPlayTime;
    public long coin;
    Dictionary<string,object> scores = new Dictionary<string, object>();
    public PlayerInfo(){}
    public PlayerInfo(string id)
    {
        this.id = id;
    }
    public PlayerInfo(string id, string nickName, long coin)
    {
        this.id = id;
        this.nickName = nickName;
        this.coin = coin;
    }
    public string GetReplaceId()
    {
        string _id = id.Replace('@',' ');
        _id = _id.Replace('.',' ');

        return _id;
    }
    public void SetScore<T>(string key, T value)
    {
        scores[key] = value;
    }
    public object GetScore<T>(string key)
    {
        return scores[key];
    }
    public virtual Dictionary<string,object> ToDictionary()
    {
        scores.Add("nickName",nickName);
        scores.Add("coin",coin);

        return scores;
    }
    public virtual string PlayerToJson()
    {
        PlayerInfo info = new PlayerInfo(this.id, this.nickName, this.coin);

        return UnityEngine.JsonUtility.ToJson(info);
    }
    
}
