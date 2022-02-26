using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OmokPlayer : BasePlayer
{
    protected LocalOmokManager GameManager;
    protected float[] xPosition = new float[GameVariable.omokBoardNum];
    protected float[] yPosition= new float[GameVariable.omokBoardNum];
    public virtual void Awake()
    {
        boardNum = GameVariable.omokBoardNum;
        SetPlayer();
        SetStonePosition();
    }
    public virtual void SetPlayer()
    {
        r = c = -1;
        m_turn = 1;
        GameManager = GameObject.Find("GameManager").GetComponent<LocalOmokManager>();
        parent = GameObject.Find("Canvas").transform;
        playerType = (EPlayerType)GameManager.playerStoneNum;
    }
    void Update() // 이 update내용을 bundle로 만들어야 함!! .. 설마 enemy가 상속받아서 이 update가 호출되는건가..?
    {
        if(GameManager.isGameOver) return;
        if(GameManager.GetTurn() != m_turn) return;

        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            IsMouseButtonDown();
            if( r < 0 || c < 0) return;
            if(!GameManager.IsEmpty(r,c))
            {
                AlertPanelView.ShowPanel("이곳에 돌을 둘 수 없습니다..",1f);
                return;
            }

            SelectStone(CreateStone());

            GameManager.SetBoardValue(r,c,m_turn);
            GameManager.CheckDirection(r,c,m_turn);

            if(GameManager.AnalyzeBoard(r,c,GameManager.setOverNum))
            {
                GameManager.GameOver();
            }
            else
            {
                GameManager.ResetLength();
            }

            if(!GameManager.isGameOver)
            {
                GameManager.NextTurn();
            }
        }
    }
    public virtual void SelectStone(GameObject stone)
    {
        OmokStone _stone = stone.GetComponent<OmokStone>();

        _stone.m_row = r;
        _stone.m_col = c;
        _stone.stoneType = playerType;
        _stone.SetImageType();
    }
    public override void SetStonePosition()
    {
        float lastPos = 4f; // 앞 뒤 제외하고 [0].x2 ~ [18].x1 까지 가야함        
        float interval = 0.5f;

        float currentPos = lastPos;

        for (int i = 0; i < boardNum; i++) // 아니 이것만 왜 되는거지?
        {
            yPosition[i] = currentPos;
            currentPos -= interval;
        }

        float startPos = -4f; // 앞 뒤 제외하고 [0].x2 ~ [18].x1 까지 가야함
        currentPos = startPos;

        for (int i = 0; i < boardNum; i++)
        {
            xPosition[i] = currentPos;
            currentPos += interval;
        }
    }
    public override void GetStonePosition(Vector3 mousePosition) // 이거 거꾸로 만들어야 함 ㅋ
    {

        float margin = 0.25f; // 순수 계산한거 수치스럽다
        float lastPos = 4f; // 앞 뒤 제외하고 [0].x2 ~ [18].x1 까지 가야함        
        float interval = 0.5f;

        float currentPos = lastPos;

        for (int i = 0; i < boardNum; i++) // 아니 이것만 왜 되는거지?
        {
            if (currentPos - margin <= mousePosition.y && mousePosition.y <= currentPos + margin)
            {
                putPosition.y = currentPos;
                r = i;
            }
            currentPos -= interval;
        }
        r = r <= -1 ? -1 : r;

        float startPos = -4f; // 앞 뒤 제외하고 [0].x2 ~ [18].x1 까지 가야함
        currentPos = startPos;

        for (int i = 0; i < boardNum; i++)
        {
            if (currentPos - margin <= mousePosition.x && mousePosition.x <= currentPos + margin)
            {
                putPosition.x = currentPos;
                c = i;
            }
            currentPos += interval;
        }
        c = c <= -1 ? -1 : c;
    }
}
