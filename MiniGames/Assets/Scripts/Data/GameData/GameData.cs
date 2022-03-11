using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using System.Threading.Tasks;
using System;

public class GameData : IData
{
    public string id;
    public readonly string TABLENAME = "PlayerData";
    public GameData(){}
    public GameData(string id){this.id = id;}
    public GameData(string id, string tableName){this.id = id; this.TABLENAME = tableName;}
    public void Create(string json) // all data save equal
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child(TABLENAME).Child(id).SetRawJsonValueAsync(json);
    }

    public void Create(Dictionary<string, object> dic) // all data save equal
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child(TABLENAME).Child(id).UpdateChildrenAsync(dic);
    }

    public async Task<Dictionary<string,object>> Load(Action callback = null)
    {
        Dictionary<string,object> dic = new Dictionary<string, object>();

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(TABLENAME);
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
                    // UnityEngine.Debug.Log($"Key = {data.Key}, Value = {data.Value}");
                    dic.Add(data.Key,data.Value);
                }
            }
        });

        if(callback != null)
            callback.Invoke();

        return dic;
    }

    public void Save<T>(string key, T value) // partical data save
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child(TABLENAME).Child(id).Child(key).SetValueAsync(value);
    }
}
