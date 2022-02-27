using UnityEngine;
using UnityEngine.UI;

public class LogoManager : MonoBehaviour
{
    Text titleText, subtitleText, versionText;
    public void Awake() {
        titleText = GameObject.Find("TitleText").GetComponent<Text>();
        subtitleText = GameObject.Find("SubTitleText").GetComponent<Text>();
        versionText = GameObject.Find("VersionText").GetComponent<Text>();

        titleText.text = $"{GameVariable.logoTitle}";
        subtitleText.text = $"Press On your key";
        versionText.text = $"{GameVariable.gameVersion}";
    }
    private void Start() {
        SoundManager.instance.ChangeBGM(EBGMClipType.Logo);
    }
    public void OnClickMainMenu()
    {   
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
        GameView.ShowFade(new GameFadeOption{
            isFade = true,
            limitedTime = 1f,
            sceneNum = SceneKind.sceneValue[ESceneKind.MainMenu].sceneNum
        });
    }
}
