using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkResult : Result
{
    public override void Awake() {
        base.Awake();
        Photon.Pun.PhotonNetwork.AutomaticallySyncScene = false;
    }
    void Start()
    {
        retryButton.gameObject.SetActive(false);
        BoardGameResult();
    }
    
    public void BoardGameResult() // 매개변수에 나의 player type이 들어가던가 하여야 구별하면서 보여줄텐데 ㅠㅠ;
    {
        int victory = GameManager.score;
        var players = FindObjectsOfType<BasePlayer>();
        BasePlayer m_Player = null;

        foreach(var p in players)
        {
            if(p.photonView.IsMine)
            {
                m_Player = p;
                break;
            }
        }
        string record = (string)DataManager.instance.player.GetScore(gameName);
        string[] newRecord = record.Split(new char[] {'/'}); // front victory, back defeat

        if((int)m_Player.playerType == victory)
        {
            SoundManager.instance.PlayClip(EEffactClipType.Victory);

            SetContentText("Victory");

            long value = int.Parse(newRecord[0]);
            value++;
            newRecord[0] = value.ToString();
        }
        else
        {
            SoundManager.instance.PlayClip(EEffactClipType.Lose);

            SetContentText("Defeat");

            long value = int.Parse(newRecord[1]);
            value++;
            newRecord[1] = value.ToString();
        }
        record = newRecord[0] + '/' + newRecord[1];
        DataManager.instance.UpdateColumn<string>(gameName,record);
    }
    public override void OnClickMainButton()
    {
        base.OnClickMainButton();

        GameView.ShowFade(new GameFadeOption
        {
            isFade = true, limitedTime = 1f,
            sceneNum = SceneKind.sceneValue[ESceneKind.Room].sceneNum
        });
    }
}
