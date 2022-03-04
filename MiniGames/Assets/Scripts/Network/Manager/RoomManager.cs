using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using Firebase.Database;

public class RoomManager : MonoBehaviourPunCallbacks
{
    protected Room room;
    [SerializeField]
    Text roomNameText;
    [SerializeField]    
    GameObject readyData;
    [SerializeField]
    Transform readyDataParent;

    public List<ReadyData> readyList = new List<ReadyData>();
    public virtual void Awake() {
        room = PhotonNetwork.CurrentRoom;
        PlayerEnter();
    }
    private void Start() {
        SoundManager.instance.ChangeBGM(EBGMClipType.Lobby);
        roomNameText.text = $"Room Name : {room.Name}";
        GameView.ShowFade(new GameFadeOption{isFade = false, limitedTime = 1f});
    }
    public override void OnPlayerEnteredRoom(Player newPlayer) // 나자신은 포함 안되는구나 ㅎ
    {
        PlayerEnter();
        room.IsVisible = room.MaxPlayers < room.PlayerCount;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerLeft(otherPlayer.NickName);
        room.IsVisible = room.MaxPlayers > room.PlayerCount;
    }
    public override void OnLeftRoom() // 이미 접속했던 플레이어가 나가는 순간 실행되는 콜백 메소드(나 자신이 떠나는 경우에만 실행됨)
    {
        // SceneManager.LoadScene("Lobby");
        PhotonNetwork.Disconnect();

        GameView.ShowFade(new GameFadeOption{isFade = true, limitedTime = 1f, sceneNum = SceneKind.sceneValue[ESceneKind.Lobby].sceneNum});
        Debug.LogError("leftRoom");
    }
    #region Enter and Left Player
    public virtual void PlayerEnter()
    {
        foreach(var player in PhotonNetwork.PlayerList)
        {
            var count = readyList.Count(r => r.player.NickName == player.NickName);
            if(count > 0) continue;

            ReadyData _readyUI = 
            Instantiate(readyData,readyDataParent).GetComponent<ReadyData>();
            readyList.Add(_readyUI);
            _readyUI.player = player;

            DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(DataManager.instance.TABLENAME);
            reference.GetValueAsync().ContinueWith(task => {
                if(task.IsFaulted)
                {
                    Debug.LogError("Fail set load data");
                    return;
                }

                if(task.IsCanceled)
                {
                    Debug.LogError("Cancel set load data");
                }

                if(task.IsCompleted)
                {
                    if(!(_readyUI is RoomReadyData)) return;

                    RoomReadyData roomReadyData = (RoomReadyData)_readyUI;
                    string gameName = (string)PhotonNetwork.CurrentRoom.CustomProperties["gameKind"];

                    DataSnapshot snapshot = task.Result;
                    foreach(var data in snapshot.Children)
                    {
                        IDictionary userInfo = (IDictionary)data.Value;
                        if(player.NickName == (string)userInfo["nickName"])
                        {
                            string res = (string)userInfo[gameName];
                            roomReadyData.SetRecord(res);
                        }
                    }
                }
            });
        }
    }
    public virtual void PlayerLeft(string name)
    {
        foreach(var ready in readyList)
        {
            if(ready.player.NickName == name)
            {
                readyList.Remove(ready);
                Destroy(ready.gameObject);
                break;
            }
        }
    }
    #endregion
    #region Player Ready
    public void ReadyPlayer(string name,bool value)
    {
        photonView.RPC("RpcReadyPlayer",RpcTarget.AllViaServer,name,value);
    }
    [PunRPC]
    void RpcReadyPlayer(string name, bool value)
    {
        foreach(var ready in readyList)
        {
            if(ready.player.NickName == name)
            {
                ready.isReady = value;
            }
        }
    }
    #endregion
    
    public void OnClickLeaveRoom() 
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
        PhotonNetwork.LeaveRoom();
    }
    public virtual void GameSceneLoad()
    {
        photonView.RPC("RpcFade",RpcTarget.AllViaServer);
    }
    [PunRPC]
    void RpcFade()
    {
        // 이건 도무지 이해를 못하겠다 왜 밖에 쪽에 하면 안되는지 이유좀 알고 싶다....
        GameView.ShowFade(new GameFadeOption{isFade = true, limitedTime = 1f,action = ()=> {
            PhotonNetwork.AutomaticallySyncScene = true;

            if(PhotonNetwork.IsMasterClient)
                PhotonNetwork.LoadLevel((string)room.CustomProperties["gameKind"]); // 아 그냥 string으로 할걸 그랫나 불편하네..
            }});
    }

}
