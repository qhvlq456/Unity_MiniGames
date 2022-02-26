using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Firebase.Database;
using Firebase.Auth;

public class GPGSAuthManager : MonoBehaviour
{
    FirebaseAuth auth;

    [SerializeField]
    Text authStatusText;
    void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .RequestIdToken()
        .RequestEmail()
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        
        // PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder()
        // .RequestIdToken()
        // .RequestEmail()
        // .Build());

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate(); // google play active
    }
    void Start()
    {
        TryGoogleLogin();
    }

    public void TryGoogleLogin()
    {
        if(!Social.localUser.authenticated) // not login status
        {
            Social.localUser.Authenticate(success => 
            {
                if(success)
                {
                    authStatusText.text = "Success Google login";
                    StartCoroutine(TryFirebaseLogin());
                    Debug.Log("Success Google login");
                }
                else
                {
                    authStatusText.text = "Fail Google login";
                    Debug.Log("Fail Google login");
                }
            });
        }
    }

    public void TryGoogleLogout()
    {
        if(Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.SignOut(); // google sign out
            auth.SignOut(); // firebase auth sign out
            authStatusText.text = "Success Sign out";
            Debug.Log("Success Sign out");
        }
    }

    IEnumerator TryFirebaseLogin()
    {
        while(string.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).GetIdToken()))
            yield return null;
        string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();

        Credential credential = GoogleAuthProvider.GetCredential(idToken,null);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => 
        {
            if(task.IsFaulted)
            {
                authStatusText.text = "Firebase Google Sign in fail..";
                Debug.Log("Firebase Google Sign in fail..");
                return;
            }

            if(task.IsCanceled)
            {
                authStatusText.text = "Firebase Google Sign in cancel..";
                Debug.Log("Firebase Google Sign in cancel..");
                return;
            }
            FirebaseUser newUser = task.Result;

            authStatusText.text = "Firebase Google Sign in success";
            Debug.Log("Firebase Google Sign in success");
            Debug.Log($"email = {newUser.Email}");
            
        });
    }
}
