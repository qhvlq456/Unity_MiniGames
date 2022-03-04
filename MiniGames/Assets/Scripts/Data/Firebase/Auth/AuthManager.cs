using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
public class AuthManager : MonoBehaviour
{
    [Header("SingIn UI")]
    [SerializeField]
    InputField inIdInput;
    [SerializeField]
    InputField inPwInput;
    [SerializeField]
    Text playerInfoText;
    [SerializeField]
    Animator inAnim;

    [Header("SingUp UI")]
    [SerializeField]
    InputField upIdInput;
    [SerializeField]
    InputField upPwInput;
    [SerializeField]
    InputField upPwConfirmInput;
    [SerializeField]
    InputField nickNameInput;
    [SerializeField]
    Animator upAnim;

    [SerializeField]
    Text testText;
    EAuthError type;
    bool isAsync = false;
    private void Update() {
        Debug.Log(FirebaseAuth.DefaultInstance.CurrentUser == null ? "current user null" : "current user not null");
    }
    public void OnClickSignInButton() // sign in
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);

        StartCoroutine(WaitAsync());
        if(FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            type = EAuthError.AlreadyEnter;
            return;
        }

        if(string.IsNullOrEmpty(inIdInput.text) || string.IsNullOrEmpty(inPwInput.text))
        {
            type = EAuthError.Empty;
            Debug.Log("Please input id or pw");
            isAsync = true;
            return;
        }

        string _id = inIdInput.text;
        string _pw = inPwInput.text;

        
        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(_id,_pw).ContinueWith(
            task => {
                if(task.IsCanceled)
                {
                    type = EAuthError.Fail;
                    print("Login fail");
                    isAsync = true;
                    return;
                }
                if(task.IsFaulted)
                {
                    type = EAuthError.Fail;
                    print("Login error");
                    isAsync = true;
                    return;
                }
                FirebaseUser user = task.Result;
                type = EAuthError.Success;
                print("Success Login");
                InitAuth();
                isAsync = true;
            }
        );

        inIdInput.text = "";
        inPwInput.text = "";
    }
    public void OnClickSignUpButton() // SignUp
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
        StartCoroutine(WaitAsync());

        if(string.IsNullOrEmpty(upIdInput.text) || string.IsNullOrEmpty(upPwInput.text) 
        || string.IsNullOrEmpty(upPwConfirmInput.text) || string.IsNullOrEmpty(nickNameInput.text))
        {
            type = EAuthError.Empty;
            isAsync = true;
            Debug.LogError("Please check inputField!!");
        }
        else
        {
            if(upPwInput.text != upPwConfirmInput.text)
            {
                type = EAuthError.NotEqual;
                isAsync = true;
                Debug.LogError("Not equal pw and confirm pw");
                return;
            }
            else
            {
                FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(upIdInput.text,upPwInput.text).ContinueWith(
                    task => 
                    {
                        if(task.IsCanceled)
                        {
                            type = EAuthError.NotCreate;
                            print("Login fail");
                            isAsync = true;
                            return;
                        }
                        if(task.IsFaulted)
                        {
                            type = EAuthError.NotCreate;
                            isAsync = true;
                            print("Login error");
                            return;
                        }
                        type = EAuthError.Create;

                        FirebaseUser user = task.Result;
                        Debug.LogFormat("Sign up name = {0} , email = {1} Success",user.DisplayName, user.UserId);
                        DataManager.instance.Create(nickNameInput.text);
                        Debug.Log($"Auth name = {nickNameInput.text}");
                        isAsync = true;
                    }
                );
            }
        }
    }
    public void OnClickInCancelAnim()
    {
        InitAuth();
        StartCoroutine(AuthCloseDelay(inAnim,false));
    }
    public void OnClickUpCancelAnim()
    {
        InitAuth();
        StartCoroutine(AuthCloseDelay(upAnim,false));
    }
    IEnumerator WaitAsync()
    {
        yield return new WaitUntil(() => isAsync);
        CreateAuthAlert();
        isAsync = false;
        InitAuth();
    }
    IEnumerator AuthCloseDelay(Animator anim, bool value)
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);

        anim.SetTrigger("isClose");
        
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length); // close 시간 보장이구나
        anim.gameObject.SetActive(value);
        anim.ResetTrigger("isClose");
        InitAuth();
    }
    void InitAuth()
    {
        upIdInput.text = "";
        upPwInput.text = "";
        upPwConfirmInput.text = "";
        nickNameInput.text = "";
    }
    
    public void CreateAuthAlert()
    {
        string title = type.ToString();
        string body = ErrorKind.authError[type];

        AlertBoxView.ShowBox(title,body);
        type = EAuthError.None;
    }
}
