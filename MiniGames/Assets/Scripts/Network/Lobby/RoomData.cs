using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomData : MonoBehaviourPun
{    
    [SerializeField]
    Text titleText;
    [SerializeField]
    Button button;
    public RoomInfo roomInfo {private get; set;}
    string roomKind;
    private void Awake() {
        OnClickEnterRoom();
    }   
    private void Start() {
        SetRoomKind();
    }
    private void Update()
    {
        titleText.text = 
        $"{roomInfo.Name} \n GameKind = {roomKind} \n {roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
    }
    void SetRoomKind()
    {
        roomKind = (string)roomInfo.CustomProperties["gameKind"];
    }
    void EnterRoom()
    {
        PhotonNetwork.JoinRoom(roomInfo.Name);
    }
    public void OnClickEnterRoom() // 와 이건 callback의 callback이냐
    {
        // button.onClick.AddListener(() => {
        //     PhotonNetwork.JoinRoom(roomInfo.Name);
        // });
        button.onClick.AddListener(() => 
        {
            SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
            
            GameView.ShowFade(new GameFadeOption{isFade = true, limitedTime = 1f, action = () => 
                {
                    PhotonNetwork.JoinRoom(roomInfo.Name);
                    
                }
            });
        });
    }
}
