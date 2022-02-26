using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ReadyData : MonoBehaviour
{
    #region Variable
    public Photon.Realtime.Player player;
    protected RoomManager roomManager;
    public bool isReady;
    protected string gameName;

    [SerializeField]
    Text playerNameText;
    #endregion
    public virtual void Awake() {
        roomManager = FindObjectOfType<RoomManager>();
        gameName = (string)PhotonNetwork.CurrentRoom.CustomProperties["gameKind"];
    }
    private void Update() {
        SetPlayerText();
    }
    public virtual void SetPlayerText()
    {
        if(player.IsMasterClient)
        {
            playerNameText.text = string.Format("{0} ({1})",player.NickName,"master");
        }
        else 
           playerNameText.text = string.Format("{0}",player.NickName);
        
        if(player.IsLocal)
            playerNameText.color = Color.yellow;
    }
}
