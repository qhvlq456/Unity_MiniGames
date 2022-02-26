using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChattingManager : MonoBehaviour
{
    #region Variable
    protected PhotonView pv;
    [SerializeField]
    protected GameObject chattingText;
    [SerializeField]
    protected InputField input;
    [SerializeField]
    GameObject parent;
    const int maxTextNum = 6;
    protected Queue<GameObject> contentsObject = new Queue<GameObject>();
    #endregion
    
    public virtual void Awake() {
        pv = GetComponent<PhotonView>();
    }
    public GameObject InstantiateText(string msg)
    {
        GameObject obj = Instantiate(chattingText,parent.transform);
        obj.GetComponent<Text>().text = msg;

        return obj;
    }
    [PunRPC]
    public void MaxQueue()
    {
        if(contentsObject.Count >= maxTextNum)
        {
            Destroy(contentsObject.Dequeue());
        }
    }
    [PunRPC]
    public void RpcEnQueue(string msg)
    {
        contentsObject.Enqueue(InstantiateText(msg));
    }
    protected void CreateText()
    {
        if(string.IsNullOrEmpty(input.text)) return;

        pv.RPC("MaxQueue",RpcTarget.AllViaServer);
        pv.RPC("RpcEnQueue",RpcTarget.AllViaServer,string.Format("{0} : {1}",PhotonNetwork.NickName,input.text));

        input.text = "";
    }
    public virtual void OnClickEnterButton()
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
        
        CreateText();
    }

}
