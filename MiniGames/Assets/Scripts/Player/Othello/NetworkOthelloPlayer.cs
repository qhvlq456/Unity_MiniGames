using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using utility = Photon.Pun.UtilityScripts;
public class NetworkOthelloPlayer : BasePlayer
{
    protected float[] xPosition = new float[GameVariable.othelloBoardNum];
    protected float[] yPosition = new float[GameVariable.othelloBoardNum];
    public PhotonView pv;
    OthelloManager GameManager;
    string stonePath = "Network/NetworkOthelloStone";
    
    void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<OthelloManager>(); // 이걸 못 찾을 일이 없을텐데;;
        r = c = -1;
        boardNum = GameVariable.othelloBoardNum;
        parent = GameObject.Find("Canvas").transform;
        pv = GetComponent<PhotonView>();
        SetStonePosition();

        StartCoroutine(WaitPlayerNumbering()); // numbering이 시간이 걸림으로 시간 보장
        gameObject.name += " " + photonView.OwnerActorNr;

        if(PhotonNetwork.IsMasterClient && pv.IsMine)
            InitStone();
        
    }
    private void Start() {
        GameManager.ChangeStoneNumber();
        GameManager.DebugBoard();
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
        if(GameManager.isGameOver) return;
        if(GameManager.GetTurn() != m_turn) return;
        if(!pv.IsMine) return;        

        if (Input.GetMouseButtonDown(0))
        {
            IsMouseButtonDown();
            if( r < 0 || c < 0) return;
            if (!GameManager.IsEmpty(r,c))
            {
                AlertPanelView.ShowPanel(ErrorKind.boardGameError[EBoardGameError.Fail],1f);
                Debug.LogError($"{playerType} is click empty location");
                return;
            }
            else
            {
                GameManager.CheckDirection(r,c,m_turn);
                if (!GameManager.AnalyzeBoard(r,c)) return;
                else
                {
                    SelectStone(CreateStone(),r,c,playerType);
                    SyncPlayer(r,c,m_turn);
                    GameManager.ChangeStone(playerType,m_turn);
                    GameManager.ResetList();

                    OtherChangeStone(r,c,m_turn,(int)playerType);

                    NextTurn();
                }
            }

            if(GameManager.ConditionGameOver())
            {
                GameOver();
                return;
            }

            if (!GameManager.CheckTransferTurn(GameManager.GetTurn(m_turn))) // 내 생각엔 여기서 엄청많이 호출하고 있다.. 각자 local을 돌리면서 로직 구현
            {
                pv.RPC("RpcTransferNextTurn",RpcTarget.AllViaServer);
                Debug.Log($"state = {GameManager.CheckTransferTurn(GameManager.GetTurn())}");
                Debug.Log("Don't put Stone therefore change turn");
                return;
            }
        }
    }
    public virtual void InitStone() // 이건 그냥 마스터 클라이언트만 하도록 하자;;
    {                                 //   0         1         2        3
        int[,] initStone = new int[,] {{ 3, 3 }, { 3, 4 }, { 4, 4 } ,{ 4, 3 }}; // row & col -> mousePos // 흑, 백, 흑, 백
        
        for (int i = 0; i < 4; i++)
        {
            putPosition = new Vector2(xPosition[initStone[i,1]],yPosition[initStone[i,0]]);
            EPlayerType type = i % 2 == 0 ? EPlayerType.Black : EPlayerType.White; 
            SelectStone(CreateStone(),initStone[i, 0], initStone[i,1],type);
            SyncPlayer(initStone[i, 0],initStone[i, 1],(int)type);
        }
    }
    [PunRPC]
    void RpcTransferNextTurn()
    {
        AlertPanelView.ShowPanel(ErrorKind.boardGameError[EBoardGameError.NextTurn],1f);
        GameManager.NextTurn();
    }
    #region CreateStone
    public override GameObject CreateStone()
    {
        return PhotonNetwork.Instantiate(stonePath,putPosition,Quaternion.identity);
    }
    public void SelectStone(GameObject stone, int row, int col, EPlayerType type)
    {
        NetworkOthelloStone _stone = stone.GetComponent<NetworkOthelloStone>();
        _stone.stoneType = type;
        _stone.GetComponent<NetworkOthelloStone>().m_row = row;
        _stone.GetComponent<NetworkOthelloStone>().m_col = col;
    }
    #region ChangeStone Bundle
    public void OtherChangeStone(int m_r, int m_c, int _turn, int type)
    {
        pv.RPC("RpcChangeStone",RpcTarget.Others,m_r, m_c, _turn , type);
    }
    [PunRPC]
    void RpcChangeStone(int m_r, int m_c, int _turn, int type)
    {
        GameManager.CheckDirection(m_r,m_c,_turn);
        GameManager.AnalyzeBoard(m_r,m_c);
        GameManager.ChangeStone((EPlayerType)type,_turn);
        GameManager.ResetList();
    }    
    #endregion
    void SyncPlayer(int m_r, int m_c, int _turn)
    {
        pv.RPC("RpcSyncPlayer",RpcTarget.All,m_r,m_c,_turn);
    }
    [PunRPC]
    void RpcSyncPlayer(int m_r, int m_c, int _turn)
    {
        GameManager.SetBoardValue(m_r,m_c,_turn);
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
    #endregion

    #region Get Set Othello Position
    public override void GetStonePosition(Vector3 mousePosition)
    {
        float lastPos = 3.7f;
        float startPos = -3.7f;
        float interval = 0.925f;
        float currentPos = lastPos; // row

        for (int i = 0; i < GameVariable.othelloBoardNum; i++) // 아니 이것만 왜 되는거지?
        {
            if (currentPos > mousePosition.y && mousePosition.y >= currentPos - interval)
            {
                r = i;
                putPosition.y = yPosition[r];
            }
            currentPos -= interval;
        }
        r = r <= -1 ? -1 : r;

        currentPos = startPos; // col

        for (int i = 0; i < GameVariable.othelloBoardNum; i++)
        {
            if (currentPos <= mousePosition.x && mousePosition.x < currentPos + interval)
            {
                c = i;
                putPosition.x = xPosition[c];
            }
            currentPos += interval;
        }
        c = c <= -1 ? -1 : c;
    }
    public override void SetStonePosition()
    {
        // ypos 
        float lastPos = 3.6f; // 앞 뒤 제외하고 [0].x2 ~ [18].x1 까지 가야함        
        float interval = 0.9f;

        float currentPos = lastPos;

        for (int i = 0; i < GameVariable.othelloBoardNum; i++) // 아니 이것만 왜 되는거지?
        {            
            yPosition[i] = currentPos - (interval / 2);
            currentPos -= interval;
        } 
        // xpos
        float startPos = -3.6f; // 앞 뒤 제외하고 [0].x2 ~ [18].x1 까지 가야함

        currentPos = startPos;

        for (int i = 0; i < GameVariable.othelloBoardNum; i++)
        {
            xPosition[i] = currentPos + (interval/2);
            currentPos += interval;
        }
    }    
    #endregion
}
