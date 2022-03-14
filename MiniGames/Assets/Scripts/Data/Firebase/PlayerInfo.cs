using System.Collections;
using System.Collections.Generic;
using System;
using Firebase.Database;
using System.Threading.Tasks;

// set을 잘함 계속 저장된 데이터를 꺼내고 저장 할 수 있음....
// set은 update임
[Serializable]
public class PlayerInfo
{
    public string uid;
    public string nickName;
    public long coin;
    public readonly string tableName = "PlayerData";
    Dictionary<string,object> playerData = new Dictionary<string, object>();
    public PlayerInfo(){}
    public PlayerInfo(string uid)
    {
        this.uid = uid;
    }
    public PlayerInfo(string uid, Dictionary<string,object> dictionary)
    {
        this.uid = uid;
        playerData = new Dictionary<string, object>(dictionary);
        playerData.Add("uid",uid);
    }
    public void SetPlayerData(string key, object value)
    {
        playerData[key] = value;
    }
    public object GetPlayerData(string key)
    {
        return playerData[key];
    }
    public async Task LoadData()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(tableName);
        await reference.Child(uid).GetValueAsync().ContinueWith(task => 
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
                    if(playerData.ContainsKey(data.Key))
                        playerData[data.Key] = data.Value;
                    else
                        playerData.Add(data.Key, data.Value);
                }
            }
        });

        nickName = (string)playerData["nickName"];
        coin = (long)playerData["coin"];
    }
    public async Task SaveData()
    {
        await FirebaseDatabase.DefaultInstance.RootReference.Child(tableName).Child(uid).UpdateChildrenAsync(ToDictionary());
        await LoadData();

        nickName = (string)playerData["nickName"];
        coin = (long)playerData["coin"];
    }
    public async Task ParticalSaveData<T>(string key, T value)
    {
        await FirebaseDatabase.DefaultInstance.RootReference.Child(tableName).Child(uid).Child(key).SetValueAsync(value);
        await LoadData();

        nickName = (string)playerData["nickName"];
        coin = (long)playerData["coin"];
    }
    public Dictionary<string,object> ToDictionary()
    {
        Dictionary<string,object> copy = new Dictionary<string, object>(playerData);

        return copy;
    }
}
