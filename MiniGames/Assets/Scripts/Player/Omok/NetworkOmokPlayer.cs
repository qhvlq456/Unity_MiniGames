using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;
using Photon.Realtime;
using utility = Photon.Pun.UtilityScripts;

public class NetworkOmokPlayer : BasePlayer
{
    protected float[] xPosition = new float[GameVariable.omokBoardNum];
    protected float[] yPosition= new float[GameVariable.omokBoardNum];
    public PhotonView pv;
    OmokManager GameManager;
    string stonePath = "Network/NetworkOmokStone";

    void Awake()
    {
        boardNum = GameVariable.omokBoardNum;
        pv = GetComponent<PhotonView>();
        parent = GameObject.Find("Canvas").transform;
        GameManager = GameObject.Find("GameManager").GetComponent<OmokManager>();

        StartCoroutine(WaitPlayerNumbering());
        gameObject.name = "Player " + photonView.OwnerActorNr;
    }
    private void Start() {
    }
    IEnumerator WaitPlayerNumbering()
    {
        while(utility.PlayerNumberingExtensions.GetPlayerNumber(pv.Owner) <= -1)
        {
            yield return null;
        }
        int playerNumber = utility.PlayerNumberingExtensions.GetPlayerNumber(pv.Owner);
        m_turn = playerNumber % 2 == 0 ? 1 : 2; // white and black
        playerType = m_turn == 1 ? EPlayerType.White : EPlayerType.Black;
    }
    void Update()
    {
        if(!pv.IsMine) return;
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

            SelectStone(CreateStone(),r,c,playerType);
            SyncPlayer(r,c);

            GameManager.CheckDirection(r,c,m_turn);

            if(GameManager.AnalyzeBoard(r,c,GameManager.setOverNum)) GameOver();
            else
            {
                GameManager.ResetLength();
            }

            if(!GameManager.isGameOver)
            {
                NextTurn();
            }
        }
    }
    #region CreateStone    
    public override GameObject CreateStone()
    {
        return PhotonNetwork.Instantiate(stonePath,putPosition,Quaternion.identity);
    }
    public void SelectStone(GameObject stone, int row, int col, EPlayerType type)
    {
        NetworkOmokStone _stone = stone.GetComponent<NetworkOmokStone>();
        _stone.stoneType = type;
        _stone.GetComponent<NetworkOmokStone>().m_row = row;
        _stone.GetComponent<NetworkOmokStone>().m_col = col;
    }
    void SyncPlayer(int m_r, int m_c)
    {
        pv.RPC("RpcSyncPlayer",RpcTarget.All,m_r,m_c);
    }
    [PunRPC]
    void RpcSyncPlayer(int m_r, int m_c)
    {
        GameManager.SetBoardValue(m_r,m_c,m_turn);
    }
    #endregion

    #region NextTurn
    public void NextTurn()
    {
        photonView.RPC("RpcNextTurn",RpcTarget.AllViaServer);
    }
    [PunRPC]
    void RpcNextTurn()
    {
        GameManager.NextTurn();
        //Debug.LogError($"manager turn = {GameManager.turn}");
    }
    #endregion

    #region GameStop
    public void GameOver()
    {
        Debug.LogError("player Gameover");
        pv.RPC("RpcGameOver",RpcTarget.AllViaServer);
    }
    [PunRPC]
    void RpcGameOver() // 여기서 한번
    {
        GameManager.GameOver(); // DestroyStones 여기에 넣어서 그냥 manager만 false하여 자동으로 하면 될 것 같은데..?
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
    #endregion
}
