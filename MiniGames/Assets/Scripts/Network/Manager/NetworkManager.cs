using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

// 이걸 싱글톤으로 바꿔야 되는건가..?
public class NetworkManager : MonoBehaviourPunCallbacks
{
    // RoomInfo : photon realtime의 방정보 기능
    #region Variable
    GameObject canvas;

    [Header("LobbyPanel")]
    GameObject lobbyPanel;
    [SerializeField]
    GameObject roomSettingsUI;
    Text nickNameText;
    Text currentRoomInfoText;
    Text currentConnectionStatusText;
    Button exitButton;
    public Button createRoomButton;

    [Header("RoomList")]
    [SerializeField]
    Transform roomParent;
    Dictionary<string,GameObject> m_Dic = new Dictionary<string, GameObject>();
    int maxRoomCount = 10;
    [SerializeField]
    GameObject roomPrefabs;
    [SerializeField]
    CanvasGroup canvasGroup;
    #endregion
    private void Awake() {
        PhotonNetwork.ConnectUsingSettings(); // master server 연결

        canvas = GameObject.Find("Canvas");
        lobbyPanel = GameObject.Find("LobbyPanel");
        nickNameText = lobbyPanel.transform.Find("NickNameText").GetComponent<Text>();
        currentConnectionStatusText = lobbyPanel.transform.Find("ConnectionStatusText").GetComponent<Text>();        
        currentRoomInfoText = lobbyPanel.transform.Find("CurrentRoomInfo").GetComponent<Text>();
        createRoomButton = lobbyPanel.transform.Find("SelectOptionPanel").Find("CreateRoomBtn").GetComponent<Button>(); /// 이거 테스트만 하고 수정하자 너무 의존성이 강함;;
        exitButton = lobbyPanel.transform.Find("ExitBtn").GetComponent<Button>();

        canvasGroup.interactable = true;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = GameVariable.gameVersion;

        EnterPlayer();
        OnClickCreateRoomUI();
        // 룸 안의 모든 접속한 클라이언트에 대해 이 레벨 로드를 유니티가 직접 하는 것이 아닌 Photon 이 하도록 하였습니다.
        // 즉, 룸에 접속시 자동으로 해당 씬으로 이동함
    }
    private void Start() {
        SoundManager.instance.ChangeBGM(EBGMClipType.Lobby);
        GameView.ShowFade(new GameFadeOption{isFade = false,limitedTime = 1f});
    }
    private void Update() {
        currentConnectionStatusText.text = PhotonNetwork.NetworkClientState.ToString();
        currentRoomInfoText.text = $"{m_Dic.Count} / {maxRoomCount}";
    }

    void EnterPlayer()
    {
        PhotonNetwork.LocalPlayer.NickName = DataManager.instance.firebasePlayer.nickName;//NCMBDataManager.instance.GetName(); // DataController.dataController.player.name;
        nickNameText.text = $"Your name : {PhotonNetwork.LocalPlayer.NickName}";
        canvasGroup.interactable = false;
    }
    #region JoinedLobby
    public override void OnJoinedLobby()
    {
        Debug.LogError("This is master lobby");
    }
    #endregion            

    #region Join Button
    public void JoinRandomRoom()
    {        
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
        PhotonNetwork.JoinRandomRoom();
    }
    #endregion
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected is master server");
        canvasGroup.interactable = true;
        PhotonNetwork.JoinLobby();
    }
    #region Update List Room  / Photon override All server in update information
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int listCount = roomList.Count;
        
        for(int i = 0; i < listCount; i++)
        {
            GameObject tempRoom = null;

            if(roomList[i].RemovedFromList)
            {
                m_Dic.TryGetValue(roomList[i].Name,out tempRoom);
                Destroy(tempRoom);
                m_Dic.Remove(roomList[i].Name);
            }
            else
            {
                if(m_Dic.ContainsKey(roomList[i].Name)) // update
                {
                    m_Dic.TryGetValue(roomList[i].Name,out tempRoom);
                    m_Dic[roomList[i].Name].GetComponent<RoomData>().roomInfo = roomList[i];
                }
                else // add
                {
                    GameObject createRoom = Instantiate(roomPrefabs,roomParent);
                    createRoom.GetComponent<RoomData>().roomInfo = roomList[i];
                    m_Dic.Add(roomList[i].Name,createRoom);
                }
            }
        }
    }    
    #endregion
    public void OnClickCreateRoomUI()
    {
        createRoomButton.onClick.AddListener(() => {
            SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);

            roomSettingsUI.SetActive(true);
        });
    }
    #region Disconnect status
    public void Disconnect()
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
        PhotonNetwork.Disconnect(); // 아에 master server까지 disconnected 하는거구나
    }
    public override void OnDisconnected(DisconnectCause cause) // master server 연결 실패시 호출되는 callback함수
    {
        canvasGroup.interactable = true;
        GameView.ShowFade(new GameFadeOption{isFade = true, limitedTime = 1f,sceneNum = SceneKind.sceneValue[ESceneKind.MainMenu].sceneNum});
        Debug.LogError("Disconnected");
        // currentConnectionStatusText.text = $"Offline : Connection Disabled {cause.ToString()} - Try reconnectind...";
        // PhotonNetwork.ConnectUsingSettings();
    }

    #endregion
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print(message);
        currentConnectionStatusText.text = "There is no empty room, Please Create Room...";
    }
    
}
