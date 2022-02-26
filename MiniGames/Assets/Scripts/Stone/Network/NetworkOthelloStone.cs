using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkOthelloStone : NetworkOmokStone,IPunObservable
{
    public override void Awake() {
        base.Awake();
        var manager = GameObject.Find("GameManager").GetComponent<OthelloManager>();
        manager.saveStones.Add(this);
    }
    // private void Start() {
    //     SoundManager.instance.PlayClip(EEffactClipType.Put);
    // }
    private void Update() {
        SetImageType();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsReading)
        {
            m_row = (int)stream.ReceiveNext();
            m_col = (int)stream.ReceiveNext();
            stoneType = (EPlayerType)stream.ReceiveNext();
        }
        else if(stream.IsWriting)
        {
            stream.SendNext(m_row);
            stream.SendNext(m_col);
            stream.SendNext(stoneType);
        }
    }
}