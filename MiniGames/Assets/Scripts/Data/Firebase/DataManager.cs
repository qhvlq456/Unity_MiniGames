using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System;
public class DataManager : MonoBehaviour
{
    public static DataManager instance = null;
    public FirebasePlayerInfo firebasePlayer;
    public FirebaseAuth auth;
    DatabaseReference reference;
    public readonly string TABLENAME = "PlayerData";
    // construct default value

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

        reference = FirebaseDatabase.DefaultInstance.GetReference(TABLENAME);
        auth = FirebaseAuth.DefaultInstance;
    }
    // load
    public void Load()
    {
        firebasePlayer = new FirebasePlayerInfo();
        reference.ValueChanged += OnLoadChanged;
    }
    void OnLoadChanged(object sender, ValueChangedEventArgs args) // 자주 변화하는 값에 대해 이벤트 핸들러를 건다 // 그니깐 진짜 안에 있지 않아도 하나라도 update하면 그냥 call되는거임
    {
        Debug.Log("Call OnLoadChanged!!");
        string id = firebasePlayer.GetReplacedId(auth.CurrentUser.Email);
        
        firebasePlayer.nickName = (string)args.Snapshot.Child(id).Child("nickName").GetValue(true);
        firebasePlayer.coin = (long)args.Snapshot.Child(id).Child("coin").GetValue(true);
        firebasePlayer.coinTime = (long)args.Snapshot.Child(id).Child("coinTime").GetValue(true);
    }
    // create
    public void Create(string nickName)
    {
        FirebasePlayerInfo newPlayer = new FirebasePlayerInfo();
        newPlayer.CreatePlayer(auth.CurrentUser.Email, nickName);
        // 이 다음 load를 진행
    }
    public void UpdateCoin(long value)
    {
        if(firebasePlayer.coin + value >= firebasePlayer.maxCoin)
        {
            firebasePlayer.coin = firebasePlayer.maxCoin;
        }
        else
        {
            firebasePlayer.coin = firebasePlayer.coin + value;
        }
        firebasePlayer.UpdateFirebaseDatabase<long>(auth.CurrentUser.Email,"coin",firebasePlayer.coin);
    }
    public void SetTimes() // 이거 해결해야 함 null reference
    {
        firebasePlayer.SetCoinTime(auth.CurrentUser.Email,"coinTime");
        firebasePlayer.UpdateFirebaseDatabase<string>(auth.CurrentUser.Email,"lastPlayTime",DateTime.Now.ToString());
        firebasePlayer.IsLoading(true);
    }
    // sign out
    public void Logout()
    {
        Quit();
        auth.SignOut();
    }
    public void Quit() // 여기서 save작업 완료 해야함
    {
        SetTimes();
        reference.ValueChanged -= OnLoadChanged;
        firebasePlayer = null;
        // 아 어차피 반환값이 task니깐 await가 가능한거구나 // 이것도 그냥 update만 하고 가는거네
    }
    
}
