using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AngryGameManager : HorizontalGame
{
    AngryObstacle obstacle;
    ObjectCamera _objectCamera;
    
    Transform startBallpos;

    GameObject ball_Prefabs, _ball;

    float leftTime;
    public int _birdCount, tempScore;
    float[] gameStateArr = new float[] 
    {GameVariable.introTime, GameVariable.startTime, GameVariable.endTime };
    public void Awake() {
        gameKind = ESceneKind.Angry;
        
        startBallpos = GameObject.Find("CatapultPivot").gameObject.transform;
        scoreText = GameObject.Find("score_Text").GetComponent<Text>();
        timeText = GameObject.Find("time_Text").GetComponent<Text>();
        lifeText = GameObject.Find("life_Text").GetComponent<Text>();
        _objectCamera = GameObject.Find("Main Camera").GetComponent<ObjectCamera>();

        obstacle = GetComponent<AngryObstacle>();
        
        ball_Prefabs = Resources.Load("Angry/Ball") as GameObject;

        _birdCount = GameVariable.birdCount;
        _ball = null;
    }
    private void Start() {
        SoundManager.instance.ChangeBGM(EBGMClipType.Angry);
        GameView.ShowFade(new GameFadeOption{isFade = false, limitedTime = 1f});
    }
    void Update()
    {        
        LifeCycle();
        CurrentText();
        NextGameState();
    }

    public override void IntroCycle()
    {
        base.IntroCycle();
        // GameView.ShowIntro(new GameIntroOption{increase = false, startNum = 3, introInterval = 1, introCount = 3});
        GameView.ShowIntro(1f,null,"3","2","1","Go");
        _objectCamera.SetInit();
        ObstacleBundle();
        CreateBall();
    }
    public override void EndCycle()
    {
        base.EndCycle();
        if (isGameOver) CreateResult();
        else
        {
            InitGame();
            DeleteBall();
        }
    }
    public void AddScore(int _score)
    {
        _birdCount--;
        if (_birdCount <= 0)
        {
            SoundManager.instance.PlayClip(EEffactClipType.AddScore);
            UpdateScore(tempScore);
            Debug.Log($"angry score = {score}");
        }
        else
            tempScore += _score * (_birdCount + 1);
    }
    void InitGame()
    {
        if (leftTime <= 0)
        {
            if (_birdCount > 0) UpdateLife();
        }
        isIntro = true;
    }
    public override void NextGameState()
    {
        leftTime += Time.deltaTime;
        if (leftTime < gameStateArr[currentStateIdx]) return;
        base.NextGameState();
        leftTime = 0f;
    }
    public void OnPassStart()
    {
        base.NextGameState();
        leftTime = 0f;
    }
    void CreateBall()
    {
        if (_ball != null) return; // 이미 존재한다면            
        _ball = Instantiate(ball_Prefabs, startBallpos);
    }
    public void DeleteBall()
    {
        if (_ball == null) return; // ball을 찾지못하면 리턴 예외처리
        Destroy(_ball);
        _ball = null;
    }
    void ObstacleBundle()
    {
        obstacle.StartObstacle();
        _birdCount = GameVariable.birdCount;
        tempScore = 0;
    }
    void CreateResult()
    {
        Instantiate((GameObject)Resources.Load("LocalResultBoard"),GameObject.Find("Canvas").transform);
    }

    public override void UpdateScore(int _score)
    {
        base.UpdateScore(_score);
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
        Debug.Log(anim.GetCurrentAnimatorStateInfo(0).length);

        anim.SetBool("isBounce",false);
    }
}
