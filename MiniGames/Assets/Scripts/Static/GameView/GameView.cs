using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameFadeOption
{
    public bool isFade = false;
    public float limitedTime = 1f;
    public int sceneNum = -1;
    public Action action = null;
}
public class GameView : MonoBehaviour
{
    static GameObject intro;
    static GameObject fade;

    public static void ShowIntro(float wait, Action action = null, params string[] message)
    {        
        if(intro == null)
        {
            intro = Resources.Load("GameViewUI/Intro") as GameObject;
        }
        GameView gameView = Instantiate(intro).GetComponent<GameView>(); // 이건 진짜 전환의 발상이다
        StartIntro(gameView,wait,action,message);
    }
    public static void ShowFade(GameFadeOption fadeOption = null)
    {
        if(fade == null)
        {
            fade = Resources.Load("GameViewUI/Fade") as GameObject;
        }
        GameView gameView = Instantiate(fade).GetComponent<GameView>(); // 이건 진짜 전환의 발상이다
        gameView.Fade(gameView.transform,fadeOption);
    
    }
    void Fade(Transform parent, GameFadeOption option = null)
    {
        Fade _fade = parent.transform.GetComponentInChildren<Fade>();

        bool isFade = option.isFade;
        float limitedTime = option.limitedTime == 0 ? 1 : option.limitedTime;
        int sceneNum = option.sceneNum >= 0 ? option.sceneNum : -1;
        Action action = option.action;

        _fade.InitImage(isFade);
        _fade.StartFade(isFade,limitedTime);

        if(isFade)
        {
            _fade.NextScene(limitedTime,sceneNum, action); // 여기에 isNetwork매개변수로 넣어야됨
        }
        else
        {
            Destroy(gameObject,limitedTime);
        }
    }
    static void StartIntro(GameView gameView,float wait ,Action action ,params string[] message)
    {
        gameView.StartCoroutine(gameView.Intro(wait,action,message));
    }

    IEnumerator Intro(float wait, Action action , params string[] message) // 굳이 queue로 던져서 해야되나.. // Queue<string> introMessage
    {
        Intro intro = gameObject.GetComponentInChildren<Intro>();

        Destroy(gameObject,wait * message.Length + 0.5f); // 0.5f는 파괴되기전에  action이 존재한다면 action을 실행하기 위해 놥둔다

        for(int i = 0; i < message.Length; i++)
        {
            intro.gameObject.SetActive(true);
            Text text = intro.GetComponent<Text>();
            text.text = message[i];
            yield return new WaitForSeconds(wait);
        }
        Debug.Log("Intro escape coroutine");

        if(action != null) 
        {
            action.Invoke();
            Debug.Log("Action is not null");
        }
        else
        {
            Debug.Log("Action is null");
        }
    }
    
}
