using UnityEngine;
using System;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
public class GameRoomManager : RoomManager
{
    Dictionary<string,string> playerPath = new Dictionary<string,string>()
    {
        {"Omok","Network/OmokPlayer"},
        {"Othello","Network/OthelloPlayer"}
    };

    public override void Awake() {
        base.Awake();
    }    
    private void Start() {
        GameView.ShowFade(new GameFadeOption{isFade = false, limitedTime = 1f});
        SpawnGamePlayer();
    }
    // public void GameOver() 
    // {
    //     PhotonNetwork.AutomaticallySyncScene = false; // client의 Scene의 자유를 준다
    //     // SceneManager.LoadScene("Room");
    // } // 아 player sequence를 맞춰야되 슈바
    void SpawnGamePlayer()
    {
        string spawnPlayerName =  (string)room.CustomProperties["gameKind"];
        PhotonNetwork.Instantiate(playerPath[spawnPlayerName],new Vector3(0,0,0),Quaternion.identity);
    }
    // [PunRPC]
    // public void RpcPeopleCount()
    // {
    //     ++StaticVariable.twoPeople;
    // }
    // IEnumerator RoomSceneLoad()
    // {
    //     AsyncOperation op =  SceneManager.LoadSceneAsync("Room");
    //     op.allowSceneActivation = false;

    //     float timer = 0f;
            
    //     while (!op.isDone)
    //     {
    //         yield return null;
    //         timer += Time.deltaTime;
    //         if(timer >= 0.9f)
    //         {
    //             op.allowSceneActivation = true;
    //             yield break;
    //         }
    //     }
    // }
}
