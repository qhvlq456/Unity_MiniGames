using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using System;
using System.Threading.Tasks;

[Serializable]
public class TestPlayer
{
    public string id;
    public string nickName;
    public long coin;

    Dictionary<string,object> playerData = new Dictionary<string, object>();
    public readonly string tableName = "PlayerData";
    public TestPlayer(){}
    public TestPlayer(string id)
    {
        GetReplaceId(id);
    }
    public TestPlayer(string id, Dictionary<string, object> dictionary)
    {
        GetReplaceId(id);
        playerData = new Dictionary<string, object>(dictionary); // call by value
    }
    void GetReplaceId(string id)
    {
        string str = id.Replace('@',' ');
        str = str.Replace('.',' ');
        this.id = str;
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
        await reference.Child(id).GetValueAsync().ContinueWith(task => 
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
                        playerData.Add(data.Key,data.Value);
                }
            }
        });

        nickName = (string)playerData["nickName"];
        coin =  (long)playerData["coin"];
    }
    public async Task SaveData()
    {
        await FirebaseDatabase.DefaultInstance.RootReference.Child(tableName).Child(id).UpdateChildrenAsync(playerData);
        await LoadData();

        nickName = (string)playerData["nickName"];
        coin =  (long)playerData["coin"];
    }
    public async Task ParticalSaveData<T>(string key, T value)
    {
        await FirebaseDatabase.DefaultInstance.RootReference.Child(tableName).Child(id).Child(key).SetValueAsync(value);
        await LoadData();

        nickName = (string)playerData["nickName"];
        coin =  (long)playerData["coin"];
    }
    public Dictionary<string,object> ToDictionary()
    {
        Dictionary<string,object> copy = new Dictionary<string, object>(playerData);

        return copy;
    }
}
