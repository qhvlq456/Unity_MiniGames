using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;

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

    DateTime previousTime;
    PlayerInfo _player = DataManager.instance.player;

    // 하.. id랑 pw를 이렇게 계속 받아서 쓰면 안될 것 같은데
    // 아 싱글톤 필요해 이건 나의 데이터가 계속 이리 되면 안됨;;
    private void Start() {
        SoundManager.instance.ChangeBGM(EBGMClipType.Main);
        previousTime = DateTime.Now;

        nameText.text = "Name : " + _player.nickName;
        coinText.text = $"Coin : {_player.coin}";
        coinTimeText.text = "00:00";

        GameView.ShowFade(new GameFadeOption{
            isFade = false,
            limitedTime = 1f,
        });

        StartCoroutine(CoinRoutine());
    }
    async void OnEnable() {
        await DataManager.instance.SetTimes();
        _player.GetCoinTime();
        DataManager.instance.SetTimesss();
    }
    private void Update() {
        UpdateCoin();
    }
    private void OnDisable() {// save
        Debug.Log("Func OnDisable");
        DataManager.instance.SetTimesss();
    }
    void UpdateCoin()
    {
        if(_player.coin >= _player.maxCoin) return;

        coinText.text = "Coin : " + DataManager.instance.player.coin;
        coinTimeText.text = TimeSpan.FromSeconds((DataManager.instance.player.coinTime - (long)(DateTime.Now.Subtract(previousTime).TotalSeconds)))
        .ToString("mm':'ss");
    }
    IEnumerator CoinRoutine()
    {
        // true일 때 빠져나감
        yield return new WaitUntil(() => _player.coin < _player.maxCoin);

        while(_player.coinTime > 0)
        {
            yield return new WaitForSeconds(1f);
            _player.coinTime--;
            previousTime = DateTime.Now;
        }
        if(_player.coin < _player.maxCoin)
        {
            DataManager.instance.UpdateCoin(_player.addPerCoin);
        }
        _player.coinTime = _player.addCoinPerDelay;

        StartCoroutine(CoinRoutine());
    }
    public void OnClickAddCoinButton() // 파기
    {
        SoundManager.instance.PlayClip(EEffactClipType.CoinButton);

        DataManager.instance.UpdateCoin(GameVariable.addCoin);
    }
    public void OnClickQuitButton()
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
        DataManager.instance.GameQuit();

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
        
        if(DataManager.instance.player.coin <= 0 || DataManager.instance.player.coin < GameVariable.consumCoin)
        {
            AlertBoxView.ShowBox("lack coin","코인이 부족합니다 확인해주세요!");
            Debug.LogError("check coin");
            return;
        }

        DataManager.instance.UpdateCoin(GameVariable.consumCoin * -1);
        GameView.ShowFade(new GameFadeOption{
            isFade = true,
            limitedTime = 1f,
            sceneNum = sceneNum
        });
        UpdateCoin();
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
    private void OnApplicationQuit() {
        DataManager.instance.SetTimesss();
    }
}
