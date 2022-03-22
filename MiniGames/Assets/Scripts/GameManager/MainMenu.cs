using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [Header("Main UI")]
    [SerializeField]
    Text nameText;
    [SerializeField]
    Text coinText;
    [SerializeField]
    Text coinTimeText;
    [SerializeField]
    GameObject optionsUI;
    PlayerInfo player;

    // 하.. id랑 pw를 이렇게 계속 받아서 쓰면 안될 것 같은데
    // 아 싱글톤 필요해 이건 나의 데이터가 계속 이리 되면 안됨;;
    async void Awake() {
        player = DataManager.instance.player;
        await DataManager.instance.LoadCoin();
    }
    private void Start() {
        SoundManager.instance.ChangeBGM(EBGMClipType.Main);
        // DataManager.instance.Load(); // 이게 중요함..
        EnterMainMenu();
        GameView.ShowFade(new GameFadeOption{
            isFade = false,
            limitedTime = 1f,
        });
    }
    private void Update() {
        if(DataManager.instance.maxCoin <= player.coin)
        {
            EnterMainMenu(); // 방법이 없음..ㅋㅋ
            DataManager.instance.ResetCoinTime();
            return;
        }
        
        if(DataManager.instance.LeftCoinTime() <= 0)
        {
            DataManager.instance.UpdateCoin(DataManager.instance.addPerCoin);
            DataManager.instance.ResetCoinTime();
        }
        EnterMainMenu(); // 방법이 없음..ㅋㅋ
    }
    async void OnDisable() {
        await DataManager.instance.SetTimes();
    }
    void EnterMainMenu()
    {
        nameText.text = "Name : " + player.nickName;
        coinText.text = "Coin : " + player.coin;
        coinTimeText.text = "CoinTime : " + DataManager.instance.VisibleCoinTime();
    }
    public void OnClickAddCoinButton()
    {
        SoundManager.instance.PlayClip(EEffactClipType.CoinButton);

        DataManager.instance.UpdateCoin(DataManager.instance.consumCoin);
    }
    public async void OnClickQuitButton()
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
        await DataManager.instance.GameQuit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else 
            Application.Quit();
        #endif
    }
    public void OnClickLogoutButton()
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);

        AlertBoxView.ShowBox("Logout","정말 로그아웃 하시겟습니까?",
        new AlertBoxOption
        {
            okButtonTitle = "Ok",
            cancelButtonTitle = "Cancel",
            okButtonAction = () => {
                DataManager.instance.Logout();

                GameView.ShowFade(new GameFadeOption{
                    isFade = true,
                    limitedTime = 1f,
                    sceneNum = SceneKind.sceneValue[ESceneKind.Logo].sceneNum
                });
            },
            cancelButtonAction = null
        });
    }
    public void OnClickMoveScene(int sceneNum) // button click event 공통된걸 하나로 빼 놔야겠다 animation, sound라든가
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
        
        if(player.coin <= 0 || player.coin < DataManager.instance.consumCoin)
        {
            AlertBoxView.ShowBox("lack coin","코인이 부족합니다 확인해주세요!");
            Debug.LogError("check coin");
            return;
        }
        DataManager.instance.UpdateCoin(-1 * DataManager.instance.consumCoin);
        GameView.ShowFade(new GameFadeOption{
            isFade = true,
            limitedTime = 1f,
            sceneNum = sceneNum
        });
        EnterMainMenu();
    }
    public void OnClickSelectGameStyle(int sceneNum)
    {   
        AlertBoxView.ShowBox(((ESceneKind)sceneNum).ToString(),$"{((ESceneKind)sceneNum).ToString()}은 local과 multi를 지원 합니다 선택해주세요",
        new AlertBoxOption
        {
            okButtonTitle = "Local",
            cancelButtonTitle = "Multi",
            okButtonAction = () => OnClickMoveScene(sceneNum),
            cancelButtonAction = () => OnClickMoveScene(SceneKind.sceneValue[ESceneKind.Lobby].sceneNum)
        });
    }
    public void OnClickOptionsButton()
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
        optionsUI.SetActive(true);
    }
}
