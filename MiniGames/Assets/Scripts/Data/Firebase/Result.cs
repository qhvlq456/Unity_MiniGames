﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    protected Button retryButton;
    [SerializeField]
    protected Button mainButton;

    [Header("Text")]
    [SerializeField]
    protected Text bodyText;
    protected string gameName;
    protected GameBase GameManager;
    public virtual void Awake() {
        GameManager = FindObjectOfType<GameBase>();
        gameName = GameManager.gameKind.ToString(); // 이거 tostring 말고 scenekind에서 내가 커스텀한 걸로 get,set 할 수 있도록
    }
    public virtual void SetContentText(string content)
    {
        Debug.Log($"content = {content}");
        bodyText.text = content;
        content = "";
    }
    public virtual void OnClickRetryButton(){
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
    }    
    public virtual void OnClickMainButton(){
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
    }
    
}
