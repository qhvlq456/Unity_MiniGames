using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameChattingManager : ChattingManager
{
    [SerializeField]
    GameObject chatting;
    [SerializeField]
    Button chattingButton;
    public override void Awake() {
        base.Awake();
    }
    public void OnClickCloseChatting()
    {
        StartCoroutine(WaitForCloseDelay());
    }
    public override void OnClickEnterButton()
    {
        base.OnClickEnterButton();
        
        pv.RPC("RpcSetOthersChatting",RpcTarget.Others);
    }
    [PunRPC]
    public void RpcSetOthersChatting()
    {
        StartCoroutine(CheckMessage());
    }
    IEnumerator CheckMessage()
    {
        while(chattingButton.gameObject.activeSelf)
        {
            chattingButton.image.color = Color.Lerp(Color.white,Color.green,Mathf.PingPong(Time.time * 2,1));
            yield return null;
        }
        
        chattingButton.image.color = Color.white;
    }
    IEnumerator WaitForCloseDelay()
    {
        chattingButton.interactable = false;
        
        Animator anim = chatting.GetComponent<Animator>();
        anim.SetTrigger("isClose");

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        chatting.SetActive(false);
        anim.ResetTrigger("isClose");

        chattingButton.interactable = true;
    }  
}
