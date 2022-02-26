using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    // construct default value
    const long defaultScore = 0;
    const long defaultCoin = 100;
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
    public void Load()
    {   
        player = new PlayerInfo(auth.CurrentUser.Email);
        
        reference.ValueChanged += OnLoadChanged;
    }
    // update(save)
    public void UpdateColumn<T>(string columnName, T value) // 수정 필요없음 어차피 load에 걸린 체인때문에 자동변경됨!!
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        reference.Child(tableName).Child(player.GetReplaceId()).Child(columnName).SetValueAsync(value);
    }
    public void UpdateCoin(long value)
    {
        long coin = player.coin;
        coin += value;
        
        UpdateColumn<long>(c_Coin,coin);
    }
    public void Create(string _name) // create class => to json => save database .. create class => to dictionary => save database
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        PlayerInfo newPlayer = new PlayerInfo(auth.CurrentUser.Email);
        newPlayer.coin = defaultCoin;
        newPlayer.nickName = _name;

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
        reference.ValueChanged -= OnLoadChanged;
        player = null;
    }
}
