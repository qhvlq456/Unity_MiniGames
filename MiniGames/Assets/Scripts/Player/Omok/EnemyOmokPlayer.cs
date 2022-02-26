using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOmokPlayer : OmokPlayer
{
    public int direction;
    public int playType;    
    public override void Awake()
    {
        base.Awake();
    }
    private void Start() {
        StartCoroutine(EnemyRoutine());
    }
    public override void SetPlayer()
    {
        base.SetPlayer();
        playerType = (EPlayerType)GameManager.enemyStoneNum;
        m_turn = 2;
        direction = -1;
    }
    private void Update() { // 상속 받았기 때문에 재정의 한것 근데 왜 override를 안써도 되는건지는 의문
    }
    IEnumerator EnemyRoutine()
    {
        if(GameManager.isGameOver)
        {
            yield break;
        }

        while(GameManager.GetTurn() != m_turn)
        {
            yield return null;
        }        

        yield return new WaitForSeconds(1f);

        float waiting = Random.Range(0.4f,0.7f);
        yield return new WaitForSeconds(waiting);

        
        waiting += Random.Range(0.3f,0.7f);
        AlertPanelView.ShowPanel("상대방이 돌을 두는 중입니다..",waiting);

        //float waiting = UnityEngine.Random.Range(min,max);
    //     AlertPanelUI alert = CreateAlertUI().GetComponent<AlertPanelUI>();
    //     alert.type = kind;
    //     alert.SetString(waiting);
    //     return UnityEngine.Random.Range(waiting + 0.3f,waiting + 0.7f);

        yield return new WaitForSeconds(waiting);

        
        if(!GameManager.PrioritySearchStone())
        {
            var m_stone = FindObjectsOfType<OmokStone>(); // 임시
            if(m_stone.Length <= 1) GameManager.EnemyStart(0);
            else 
            {
                int rnd = Random.Range(1,10);
                GameManager.EnemyStart(rnd);
            }
        }

        putPosition = new Vector3(xPosition[c],yPosition[r],0);

        SelectStone(CreateStone());

        GameManager.SetBoardValue(r,c,m_turn);
        GameManager.CheckDirection(r,c,m_turn);
        
        if(GameManager.AnalyzeBoard(r,c,5))
        {
            Debug.Log($"analyze r = {r}, c = {c}, turn = {m_turn}");
            GameManager.GameOver();
        }
        else
        {
            GameManager.ResetLength();
        }

        if(!GameManager.isGameOver)
        {
            GameManager.NextTurn();
            StartCoroutine(EnemyRoutine());
        }
    }
}
