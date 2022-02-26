using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkOmokStone : Stone
{
    protected PhotonView pv;
    public virtual void Awake() {
        pv = GetComponent<PhotonView>();
        _renderer = GetComponent<SpriteRenderer>();
    }
    private void Start() {
        SyncStone();
        SoundManager.instance.PlayClip(EEffactClipType.Put);
    }
    
    public void SyncStone()
    {
        pv.RPC("RpcSyncStone",RpcTarget.AllViaServer,m_row,m_col,(int)stoneType);
    }
    [PunRPC]
    public void RpcSyncStone(int r, int c, int type)
    {
        m_row = r;
        m_col = c;
        stoneType = (EPlayerType)type;
        SetImageType();
    }
}
