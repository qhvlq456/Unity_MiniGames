using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Fade : MonoBehaviour
{
    Image image;
    float timer;
    private void Awake() {
        image = GetComponent<Image>();
    }    
    public void InitImage(bool isFade)
    {
        if(isFade) // 점점 어두워지게
        {
            image.color = Color.clear;
        }
        else
        {
            image.color = Color.black;
        }
    }
    public void StartFade(bool isFade ,float limitedTime)
    {
        StartCoroutine(StartCoroutine(isFade,limitedTime));
    }
    IEnumerator StartCoroutine(bool isFade, float limitedTime)
    {
        timer = 0;
        while(timer <= limitedTime)
        {
            timer += Time.deltaTime;

            if(isFade) // 점점 어두워지게
            {
                image.color = Color.Lerp(Color.clear,Color.black,timer);
            }
            else
            {
                image.color = Color.Lerp(Color.black,Color.clear,timer);
            }
            yield return null;
        }
    }
    IEnumerator NextSceneRoutine(float limitedTime,int sceneNum, Action action)
    {
        yield return new WaitForSeconds(limitedTime);

        if(action != null)
        {
            Debug.LogError("Action Call");
            action();
        }
        else
        {
            SceneManager.LoadScene(sceneNum);
        }
    }
    public void NextScene(float limitedTime, int sceneNum, Action action)
    {
        StartCoroutine(NextSceneRoutine(limitedTime,sceneNum,action));
        // 원인 destroy가 sceneload보다 훨씬 빠르게 진행되는 문제 였다 어차피 씬이 바뀌면 오브젝트가 파괴되니 굳이 Destory할 필요가 없다
    }
}
