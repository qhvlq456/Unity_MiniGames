using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChattingText : MonoBehaviour
{    
    Text m_Text;
    void Start()
    {
        m_Text = GetComponent<Text>();
        string target = m_Text.text.Substring(0,PhotonNetwork.NickName.Length); // indexOf로도 확인가능 

        if(target == PhotonNetwork.NickName)
            m_Text.color = Color.yellow;

        Destroy(gameObject,15f);
    }
}
