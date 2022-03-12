using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Firebase.Auth;
using Firebase.Database;
public class AuthManager : MonoBehaviour
{
    [Header("Firebase Connection UI")]
    FirebaseAuth auth;
    [SerializeField]
    InputField nickNameInput;
    [Header("NickNameUI")]
    [SerializeField]
    GameObject nickNameUI;
    [Header("LogoManagerButton Create")]
    [SerializeField]
    GameObject screenOverlay;
    [SerializeField]
    Animator inAnim;
    EAuthError type;
    bool isAsync = false;
    private void Awake() {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .RequestIdToken()
        .RequestEmail()
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate(); // Play Games Active(활성화)

        auth = FirebaseAuth.DefaultInstance;
    }
    private void Start() {
        TryGoogleLogin();
    }
    private void Update() {
        Debug.Log(FirebaseAuth.DefaultInstance.CurrentUser == null ? "current user null" : "current user not null");
    }
    public void TryGoogleLogin()
    {
        if(!Social.localUser.authenticated) // 로그인 상태가 아니라면
        {
            Social.localUser.Authenticate(success => 
            {
                string status = "";
                if(success)
                {
                    status = "Success google play login";
                    Debug.Log(status);
                    StartCoroutine(TryFirebaseLogin()); // try firebase login
                }
                else
                {
                    status = "Fail google play login";
                    Debug.Log(status);
                }
            });
        }
    }
    // FirebaseAuth를 통해 FirebaseUser를 얻고난 후 User의 UID를 활용할 것이다
    IEnumerator TryFirebaseLogin()
    {
        while(string.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).GetIdToken()))
            yield return null;
        string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();

        Credential credential = GoogleAuthProvider.GetCredential(idToken,null);
        
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => 
        {
            string status = "";

            if(task.IsFaulted)
            {
                status = "Firebase google login is Fail";
                Debug.Log(status);
                return;
            }

            if(task.IsCanceled)
            {
                status = "Firebase google login is cancel";
                Debug.Log(status);
                return;
            }
            status = "Firebase google login is success";
            Debug.Log(status);
            // 처음 들어온 유저인지, 이미 존재하는 유저인지 확인
            CreateOrLoadData();
        });
    }
    public void CreateOrLoadData()
    {
        FirebaseDatabase.DefaultInstance.GetReference(DataManager.instance.tableName).GetValueAsync().ContinueWith(task => 
        {
            if(task.IsFaulted)
            {
                return;
            }
            if(task.IsCanceled)
            {
                return;
            }
            if(task.IsCompleted)
            {
                bool isExist = false;
                DataSnapshot snapshot = task.Result;
                
                foreach(var data in snapshot.Children)
                {
                    IDictionary value = (IDictionary)data.Value;
                    if((string)value["uid"] == auth.CurrentUser.UserId) // 여기서 나와 같은 uid를 찾는다
                    {
                        isExist = true;
                        break;
                    }
                }
                if(!isExist) // 처음 들어온 유저라면 닉네임 창을 표시함
                {
                    nickNameUI.SetActive(true);
                }
                else
                {
                    DataManager.instance.Load();
                    Destroy(screenOverlay); // 이 overlay가 파괴되기 전까지 진입 기타등등 아무것도 하지못한다
                }
            }
        });
    }
    public async void OnClickConfirmNickName() // nickname중복 확인
    {
        if(string.IsNullOrEmpty(nickNameInput.text)) 
        {
            type = EAuthError.Empty;
            CreateAuthAlert();
            return;
        }

        StartCoroutine(WaitAsync());
        await FirebaseDatabase.DefaultInstance.GetReference(DataManager.instance.tableName).GetValueAsync().ContinueWith(task =>
        {
            if(task.IsFaulted)
            {
                return;
            }
            if(task.IsCanceled)
            {
                return;
            }
            if(task.IsCompleted)
            {
                bool isOverlap = false;
                DataSnapshot snapshot = task.Result;
                foreach(var data in snapshot.Children)
                {
                    IDictionary value = (IDictionary)data.Value;
                    if((string)value[DataManager.instance.c_Nick] == nickNameInput.text)
                    {
                        Debug.LogError($"Same nick = {value[DataManager.instance.c_Nick]}, current nick = {nickNameInput.text}");
                        nickNameInput.text = "";
                        isOverlap = true;
                        type = EAuthError.Exist;
                        break;
                    }
                }

                if(!isOverlap)
                {
                    type = EAuthError.Success;
                    DataManager.instance.Create(nickNameInput.text);
                    nickNameUI.SetActive(false);
                    CreateOrLoadData(); // database를 생성하고 다시 load하기 위해 호출
                }
                isAsync = true;
            }
        });
    }
    public void OnClickInCancelAnim()
    {
        StartCoroutine(AuthCloseDelay(inAnim,false));
    }
    IEnumerator WaitAsync()
    {
        yield return new WaitUntil(() => isAsync);
        CreateAuthAlert();
        //isAsync = true;
        isAsync = false;
    }
    IEnumerator AuthCloseDelay(Animator anim, bool value)
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);

        anim.SetTrigger("isClose");
        
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length); // close 시간 보장이구나
        anim.gameObject.SetActive(value);
        anim.ResetTrigger("isClose");
    }
    
    public void CreateAuthAlert()
    {
        string title = type.ToString();
        string body = ErrorKind.authError[type];

        AlertBoxView.ShowBox(title,body);
        type = EAuthError.None;
    }
}
