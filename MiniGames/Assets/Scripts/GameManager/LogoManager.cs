using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;

public class LogoManager : MonoBehaviour
{
    [Header("SignIn UI")]
    [SerializeField]
    GameObject signInUI;
    [Header("SignUp UI")]
    [SerializeField]
    GameObject signUpUI;

    Text titleText, subtitleText, versionText;
    public void Awake() {
        titleText = GameObject.Find("Title_Text").GetComponent<Text>();
        subtitleText = GameObject.Find("SubTitle_Text").GetComponent<Text>();
        versionText = GameObject.Find("Version_Text").GetComponent<Text>();

        titleText.text = $"{GameVariable.logoTitle}";
        subtitleText.text = $"Press On your key";
        versionText.text = $"{GameVariable.gameVersion}";
    }
    private void Start() {
        SoundManager.instance.ChangeBGM(EBGMClipType.Logo);
    }
    public void OnClickSignInButton()
    {   
        AllActiveFalse(signInUI);
    }
    public void OnClickSignUpButton()
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
        
        AllActiveFalse(signUpUI);
    }
    void AllActiveFalse(GameObject panel)
    {
        signInUI.SetActive(false);
        signUpUI.SetActive(false);

        panel.SetActive(true);
    }
    void Update()
    {
        if(signInUI.activeSelf || signUpUI.activeSelf) return;
        
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
            
            if(FirebaseAuth.DefaultInstance.CurrentUser == null)
            {
                OnClickSignInButton();
            }
            else
            {
                DataManager.instance.Load();
                GameView.ShowFade(new GameFadeOption{
                    isFade = true,
                    limitedTime = 1f,
                    sceneNum = SceneKind.sceneValue[ESceneKind.MainMenu].sceneNum
                });
            }
        }
    }
}
