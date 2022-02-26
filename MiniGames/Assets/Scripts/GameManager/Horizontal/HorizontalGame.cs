using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum GameState 
{   
    intro, 
    start, 
    end 
}
public class HorizontalGame : GameBase
{
    #region Variable
    public GameState gameState {get; protected set; }
    public int life { get; private set; }
    public float time { get; private set; }
    public Text lifeText { protected get; set; }
    public Text timeText { protected get; set; }
    int maxStateIdx;
    protected bool isIntro = true;
    protected int currentStateIdx { get; private set; }
    #endregion
    #region Constructor
    public HorizontalGame()
    {
        life = 3;
        time = 0f;
        maxStateIdx = Enum.GetValues(typeof(GameState)).Length;
        gameState = GameState.intro;
        currentStateIdx = 0;
    }
    #endregion
    public virtual void LifeCycle()
    {
        if (isGameOver) return;
        if (gameState == GameState.intro)
        {
            if(!isIntro) return;
            IntroCycle();
        }
        else if (gameState == GameState.start)
        {
            StartCycle();
        }
        else // end state
        {
            EndCycle();
        }
    }
    public virtual void UpdateLife()
    {
        life--;
    }
    public void UpdateTime()
    {
        time += Time.deltaTime;
    }
    public override void CurrentText()
    {
        base.CurrentText();
        lifeText.text = $"Life : {life}";
        timeText.text = $"Time : {time:N2}"; // 0.00 N2자리 사용가능
    }
    public void GameOver()
    {
        if (life <= 0) IsGameOver();
    }
    public virtual void NextGameState()
    {
        currentStateIdx = (int)gameState;
        currentStateIdx = (currentStateIdx + 1) % maxStateIdx;
        gameState = (GameState)currentStateIdx;
    }
    public virtual void IntroCycle() 
    {
        isIntro = false;
        Invoke("NextGameState",3f);
    }    
    public virtual void StartCycle()
    {
        UpdateTime();
    }
    public virtual void EndCycle() // flappy는 아에 끝나는거고 angry는 다시 첨으로 가는 것;;
    {
        GameOver();
    }        
}



