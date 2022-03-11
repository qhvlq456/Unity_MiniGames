using Firebase.Database;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class TestFirebaseInfo : PlayerInfo
{
    public GameData gameData;
    public TimeData timeData;
    bool isLoading = true;
    public readonly string ADDCOIN = "addCoin";
    public readonly string ADDCOINPERDELAY = "addCoinPerDelay";
    public readonly string MAXCOIN = "maxCoin";
    public readonly string TABLENAME = "PlayerData";
    public readonly long addCoinPerDelay = 60;
    public readonly long addPerCoin = 10;
    public readonly long maxCoin = 200;
    public readonly long defaultScore = 0;
    public TestFirebaseInfo() : base(){}
    public TestFirebaseInfo(string id) : base(id)
    {
        this.id = id;
    }
    public TestFirebaseInfo(string id, string nickName, long coin) : base(id, nickName, coin)
    {
        this.id = id;

        timeData = new TimeData();
        gameData = new GameData(GetReplaceId());

        this.nickName = nickName;
        this.coin = coin;
    }
    public void IsLoading(bool value)
    {
        isLoading = value;
    }
    public void Create()
    {
        gameData.Create(ToDictionary());
    }
    public void AllSave()
    {
        Create();
    }
    public void Update<T>(string key, T value)
    {
        gameData.Save<T>(key,value);
    }
    public async void Load() // 이게 async가 되는구나
    {
        Dictionary<string,object> dic = await gameData.Load();

        await Task.Run(() => 
        {
            timeData.lastPlayTime = (string)dic["lastPlayTime"];
            timeData.coinTime = (long)dic["coinTime"];
        });

        gameData.Save<long>("coin",coin);
        isLoading = false;
    }

    
    public override Dictionary<string,object> ToDictionary()
    {   
        SetScore<string>("nickName",nickName);
        SetScore<long>("coin",coin);
        SetScore<long>("coinTime",timeData.coinTime);

        Dictionary<string,object> dic = new Dictionary<string, object>(base.ToDictionary());

        return dic;
    }
    public override string PlayerToJson()
    {
        TestFirebaseInfo newPlayer = new TestFirebaseInfo(this.id,this.nickName, this.coin);
        return UnityEngine.JsonUtility.ToJson(newPlayer);
    }
}
