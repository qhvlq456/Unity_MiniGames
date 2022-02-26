using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlappyGameManager : HorizontalGame
{        
    public void Awake() {
        gameKind = ESceneKind.Flappy;

        scoreText = GameObject.Find("score_Text").GetComponent<Text>();
        timeText = GameObject.Find("time_Text").GetComponent<Text>();
        lifeText = GameObject.Find("life_Text").GetComponent<Text>();
    }
    private void Start() {
        SoundManager.instance.ChangeBGM(EBGMClipType.Flappy);
        GameView.ShowFade(new GameFadeOption{isFade = false, limitedTime = 1f});
    }
    void Update()
    {
        LifeCycle();
        CurrentText();
    }
    public override void IntroCycle()
    {
        GameView.ShowIntro(1f,null,"3","2","1","Go");
        base.IntroCycle();
    }
    public override void EndCycle()
    {
        NextGameState();
        base.EndCycle(); // ResultUI
        Instantiate((GameObject)Resources.Load("LocalResultBoard"),GameObject.Find("Canvas").transform);
    }
    public override void UpdateScore(int _score)
    {
        base.UpdateScore(_score);
        SoundManager.instance.PlayClip(EEffactClipType.AddScore);
        Animator anim = scoreText.gameObject.GetComponent<Animator>();
        StartCoroutine(WaitBounceAnim(anim));
    }

    public override void UpdateLife()
    {
        base.UpdateLife();
        Animator anim = lifeText.gameObject.GetComponent<Animator>();
        StartCoroutine(WaitBounceAnim(anim));
    }
    IEnumerator WaitBounceAnim(Animator anim)
    {
        anim.SetBool("isBounce",true);

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        anim.SetBool("isBounce",false);
    }


}
