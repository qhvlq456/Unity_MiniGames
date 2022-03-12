using UnityEngine;
using UnityEngine.UI;

public class LogoManager : MonoBehaviour
{
    Text titleText, subtitleText, versionText;
    public void Awake() {
        titleText = GameObject.Find("TitleText").GetComponent<Text>();
        subtitleText = GameObject.Find("SubTitleText").GetComponent<Text>();
        versionText = GameObject.Find("VersionText").GetComponent<Text>();

        subtitleText.text = $"Press On your key";
    }
    private void Start() {
        SoundManager.instance.ChangeBGM(EBGMClipType.Logo);
        titleText.text = $"{Application.productName}"; 
        versionText.text = $"{Application.version}";
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
