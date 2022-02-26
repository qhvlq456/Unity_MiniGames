using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

// nightmare, local boardgame, horizontal game
public class LocalResult : Result
{    
    void Start()
    {
        RendererButton();
        switch(gameName)
        {
            case "Flappy" : 
            case "Angry" :
                HorizontalGameResult();
                break;
            case "LocalOmok" :
            case "LocalOthello" : 
                BoardGameResult();
                break;
            case "NightMare" :
                NightMareResult();
                break;
        }
    }

    void RendererButton()
    {
        if (DataManager.instance.player.coin < GameVariable.consumCoin)
        {
            retryButton.interactable = false;
            Image sprite = retryButton.GetComponent<Image>();
            sprite.color = new Color(1, 1, 1, 0.5f);
        }

    }
    void HorizontalGameResult()
    {
        List<KeyValuePair<string,object>> list = new List<KeyValuePair<string, object>>();

        if(GameManager.score > (long)DataManager.instance.player.GetScore(gameName))
        {
            Victory(list);
        }
        else
        {
            Lose(list);
        }
    }
    public void BoardGameResult() // 매개변수에 나의 player type이 들어가던가 하여야 구별하면서 보여줄텐데 ㅠㅠ;
    {
        int victory = GameManager.score;
        Debug.Log($"{(EPlayerType)victory}");
        // interface구현도 생각해 봐야 할 것 같은데..
        var players = FindObjectsOfType<BasePlayer>();
        
        BasePlayer m_Player = null;

        foreach(var p in players)
        {
            if(p.m_turn == 1) // 무조건 먼저 시작하게 하기위해 player는 turn이 1이 고정이다
            {
                m_Player = p;
                break;
            }
        }

        string record = (string)DataManager.instance.player.GetScore(gameName);
        string[] newRecord = record.Split(new char[] {'/'}); // front victory, back defeat

        if((int)m_Player.playerType == victory) // 이거 더 줄일 수 있다 .. 근데 로직을 변경해야 되넼ㅋㅋ
        {
            long value = long.Parse(newRecord[0]);
            value++;
            newRecord[0] = value.ToString();
            record = newRecord[0] + '/' + newRecord[1];
            Victory(null,record,"Victory");
        }
        else
        {
            long value = long.Parse(newRecord[1]);
            value++;
            newRecord[1] = value.ToString();
            record = newRecord[0] + '/' + newRecord[1];
            Lose(null,true,record,"Lose");
        }
    }
    public void NightMareResult()
    {
        // nightmare는 내 스코어만 받아와야함 즉, update전에 내 score를 받아와야함ㅋㅋ
        string[] enemyNames = new string[] {"Bunny", "Bear", "Hellephant"};

        string body = "";

        for(int i = 0; i < NightmareManager.Manager.enemyKills.Length; i++)
        {
            body += string.Format("{0} : {1}\n",enemyNames[i],NightmareManager.Manager.enemyKills[i]);
        }

        if(GameManager.score > (long)DataManager.instance.player.GetScore(gameName))
        {
            body += $"Total = {NightmareManager.Manager.score} \n My Best Score {NightmareManager.Manager.score}";
            Victory(null,null,body);
        }
        else
        {
            long value = (long)DataManager.instance.player.GetScore(gameName);
            body += $"Total = {NightmareManager.Manager.score} \n My Best Score {value}";
            Lose(null,false,null,body);
        }
    }
    void Victory(List<KeyValuePair<string,object>> list = null, string record = null, string body = null) // 무조건 update필요
    {
        // # Effect Sound
        SoundManager.instance.PlayClip(EEffactClipType.Victory);
        // 1. update
        switch(gameName)
        {
            case "Flappy" : 
            case "Angry" :
            case "NightMare" :
                DataManager.instance.UpdateColumn<long>(gameName,GameManager.score);
                break;
            case "LocalOmok" :
            case "LocalOthello" : 
                DataManager.instance.UpdateColumn<string>(gameName,record);
                break;
        }
        // 2. appear alert text
        if(body != null)
            SetContentText(body);
        else
            RankSort(gameName);
    }
    // board 게임을 제외한 다른게임들은 update 불필요
    void Lose(List<KeyValuePair<string,object>> list = null ,bool isBoard = false, string record = null, string body = null)
    {
        // # Effect Sound
        SoundManager.instance.PlayClip(EEffactClipType.Lose);
        // 1. update # condition : gamekind = boardgame
        if(isBoard) // 2. appear alert text
            DataManager.instance.UpdateColumn<string>(gameName,record);
        
        if(body != null)
            SetContentText(body);
        else
            RankSort(gameName);
    }
    public override void OnClickRetryButton()
    {
        base.OnClickRetryButton();

        if (DataManager.instance.player.coin >= GameVariable.consumCoin)
        {
            DataManager.instance.UpdateCoin(-1 * GameVariable.consumCoin);

            GameView.ShowFade(new GameFadeOption{isFade = true, limitedTime = 1f,
            sceneNum = SceneKind.sceneValue[GameManager.gameKind].sceneNum
            });
            // setactive false해도 fade때문에 안되네... 이걸 해결해야 함!!
        }
    }
    public override void OnClickMainButton()
    {
        base.OnClickMainButton();
        
        GameView.ShowFade(new GameFadeOption{isFade = true, limitedTime = 1f, sceneNum = SceneKind.sceneValue[ESceneKind.MainMenu].sceneNum});
    }
    // Ranking
    public void RankSort(string columnName)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(DataManager.instance.tableName);

        reference.OrderByChild(columnName).GetValueAsync().ContinueWith(task => 
        {
            if(task.IsFaulted)
            {
                Debug.LogError("Load Fail");
                return;
            }
            if(task.IsCanceled)
            {
                Debug.LogError("Load Cancel");
                return;
            }
            if(task.IsCompleted)
            {
                string result = "";
                DataSnapshot snapshot = task.Result;
                List<KeyValuePair<string,object>> list = new List<KeyValuePair<string, object>>();

                foreach(var data in snapshot.Children)
                {
                    IDictionary userInfo = (IDictionary)data.Value;
                    list.Add(new KeyValuePair<string, object>((string)userInfo[DataManager.instance.c_Nick],userInfo[columnName]));
                }
                list.Sort((a,b) => (long)a.Value > (long)b.Value ? -1 : 1);

                for(int i = 0; i < list.Count; i++)
                {
                    result += string.Format("Name : {0}, Score : {1}\n",list[i].Key, list[i].Value);
                }
                SetContentText(result);
            }
        });
    }
}
