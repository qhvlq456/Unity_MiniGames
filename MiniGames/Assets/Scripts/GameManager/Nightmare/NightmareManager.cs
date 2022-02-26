using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightmareManager : GameBase
{
    static NightmareManager manager;
    public static NightmareManager Manager
    {
        get
        {
            if(manager == null)
            {
                manager = FindObjectOfType<NightmareManager>();

                if(manager == null)
                {
                    GameObject container = new GameObject("GameManager");
                    manager = container.AddComponent<NightmareManager>();
                }
            }
            return manager;
        }
    }

    [Header("UI Text")]
    [SerializeField]    
    Text timeText;
    [SerializeField]
    Text stageText;
    Image[] playerLife;
    [Header("GameState")]
    [SerializeField]
    GameObject introText;
    [SerializeField]
    GameObject resultPanel;
    // enemys
    public List<GameObject> enemys;
    public GameObject target;
    public int[] enemyKills;
    // game
    public GameState gameState;
    SpawnManager spawnManager;
    float timer = 0f;
    float limtedTime = 30f;
    public int currentStage = 1;
    int maxStage = 3;
    public int life = 2;
    public bool isIntro = true;
    private void Awake() {
        gameState = GameState.intro;
        gameKind = ESceneKind.NightMare;

        GameObject lifeImage = GameObject.Find("LifeUI");

        scoreText = GameObject.Find("Score Text").GetComponent<Text>();
        spawnManager = GetComponent<SpawnManager>();

        enemyKills = new int[3];

        playerLife = new Image[lifeImage.transform.childCount];
        for(int i = 0; i < lifeImage.transform.childCount; i++)
        {
            playerLife[i] = lifeImage.transform.GetChild(i).GetComponent<Image>();
        }
    }
    private void Start() {
        SoundManager.instance.ChangeBGM(EBGMClipType.Nightmare);
        GameView.ShowFade(new GameFadeOption{isFade = false, limitedTime = 1f});
        StartCoroutine(GameLogic());
    }
    void UpdateTimer()
    {
        if(timer <= limtedTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            gameState = GameState.end;
        }

        timeText.text = string.Format("Time : {0:N2}",timer);
    }
    public void AddScore(int score)
    {
        SoundManager.instance.PlayClip(EEffactClipType.AddScore);
        UpdateScore(score);
        StartCoroutine(WaitForAnim(scoreText.GetComponent<Animator>(),"isBounce"));
        

        CurrentText();
    }
    public void UpdateLife()
    {
        StartCoroutine(WaitForAnim(playerLife[life].gameObject.GetComponent<Animator>(),"isDisappear",() => life--));
    }
    IEnumerator WaitForAnim(Animator anim, string animName, System.Action action = null)
    {        
        anim.SetBool(animName,true);

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        if(action != null)
            action.Invoke();

        anim.SetBool(animName,false);
    }
    public void GameOver() // 그냥 여따 callback넣어도 되지않나??
    {
        IsGameOver();
        InitializeGame();
        Instantiate((GameObject)Resources.Load("LocalResultBoard"),GameObject.Find("Canvas").transform);
    }
    public void EnemyKills(int enemyIndex)
    {
        enemyKills[enemyIndex]++;
    }
    public void RemoveEnemys(GameObject enemy)
    {
        if(enemys.Contains(enemy))
        {
            enemys.Remove(enemy);
        }
        else
        {
            Debug.LogError("Not find enemy object!");
        }
    }
    void InitializeGame()
    {
        timer = 0;
        foreach(var obj in enemys)
        {
            Animator enemyAnim =  obj.GetComponent<Animator>();
            enemyAnim.SetTrigger("Dead");
            Destroy(obj,2f);
        }

        enemys.Clear();
    }
    IEnumerator GameLogic()
    {
        while(maxStage >= currentStage && life >= 0)
        {
            if(gameState == GameState.intro)
            {
                if(isIntro)
                {
                    GameView.ShowIntro(1f, IntroOptions ,$"Stage : {currentStage}","Start"); // 결국은 이것도 함수구나
                    isIntro = false;
                }
            }
            else if(gameState == GameState.start)
            {
                UpdateTimer();
            }
            else
            {
                if(!isIntro)
                {
                    GameView.ShowIntro(1.5f, EndOptions ,$"End");
                }
                yield return new WaitForSeconds(1.5f);
            }
            yield return null;
        }

        GameOver();
    }
    void IntroOptions()
    {
        stageText.text = $"Stage : {currentStage}";
        StartCoroutine(WaitForAnim(stageText.gameObject.GetComponent<Animator>(),"isBounce"));

        spawnManager.enabled = true;
        gameState = GameState.start;
        Debug.Log("IntroOptions");
    }
    void EndOptions()
    {
        InitializeGame();
        target = null;
        gameState = GameState.intro;
        currentStage++;
        spawnManager.enabled = false;
        isIntro = true;
        Debug.Log("EndOptions");
    }
}
