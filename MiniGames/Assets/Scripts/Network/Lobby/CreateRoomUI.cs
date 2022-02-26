using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoomUI : MonoBehaviourPunCallbacks
{
    [Header("GameButton")]
    [SerializeField]
    Button omokButton;
    [SerializeField]
    Button othelloButton;

    [Header("InputField")]
    [SerializeField]
    InputField roomNameInput;
    
    int type;
    public byte maxPlayer = 2;
    string gameKind;
    public override void OnEnable() {
        base.OnEnable();
        type = (int)ESceneKind.Omok;
        SetButtonColor();
    }
    public void OnClickGameKind(int _type)
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
        type = _type;
        SetButtonColor();
    }
    void SetButtonColor()
    {
        // gameKind = SceneKind.GetSceneName((int)type);
        gameKind = SceneKind.sceneValue[(ESceneKind)type].sceneName;
        
        switch(gameKind)
        {
            case "Omok" : 
            omokButton.image.color = Color.green;
            othelloButton.image.color = Color.white;
            break;
            case "Othello" : 
            omokButton.image.color = Color.white;
            othelloButton.image.color = Color.green;
            break;
            default : 
            omokButton.image.color = Color.white;
            othelloButton.image.color = Color.white;
            break;
        }
    }

    #region Settings open n close
    public void OnClickSettingsClose(bool value)
    {
        StartCoroutine(SettingsCloseDelay(value));
    }
    IEnumerator SettingsCloseDelay(bool value)
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
        
        Animator anim = GetComponentInChildren<Animator>();

        anim.SetTrigger("isClose");
        
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length); // close 시간 보장이구나
        gameObject.SetActive(value);

        anim.ResetTrigger("isClose");
    }
    #endregion

    #region Settings option create Room
    public void OnClickCreateRoomButton()
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);

        if(string.IsNullOrEmpty(roomNameInput.text)) return;
        else if(type <= -1) return;
        
        GameView.ShowFade(new GameFadeOption{isFade = true, limitedTime = 1f, action = () => 
        {
            PhotonNetwork.CreateRoom(roomNameInput.text,
            new RoomOptions 
            {
                MaxPlayers = maxPlayer,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {{"gameKind",gameKind}},
                CustomRoomPropertiesForLobby = new string[1] {"gameKind"} // 엄청난 삽질이다 로비에 룸 커스텀 받으려면 스트링으로 키값 셋팅해놔야함
            }
            , null);
        }
        });
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("Join CreateRoom");
        // 아니 슈발 fade in out 어케해 loadlevel인데.. action으로 퍼가야되나
        if(PhotonNetwork.IsMasterClient) // master client에서만 load scene 사용가능
        {
            Debug.LogError($"Load Scene   gamekind = {type + 1}");
            // 이게 문제긴 하네..
            PhotonNetwork.LoadLevel("Room");
        }
    }
    #endregion
}
