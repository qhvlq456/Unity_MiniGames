using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Firebase.Auth;

public class AppMenu : MonoBehaviour
{
    static int DummyNum = 10;
    static string packageFile = "FirebaseGames_Email(Success)_Portfolio(base).unitypackage";
    static bool isRobby;
    static bool isSettings;
    [MenuItem("FirebaseGames/Export BackUp", false, 0)]
    static void action01()
    {
        string[] exportpaths = new string[]
        {
            "Assets/Animations",
            "Assets/Audio",
            "Assets/Editor",
            "Assets/Editor Default Resources",
            "Assets/ExternalDependencyManager",
            "Assets/Firebase",
            "Assets/Font",
            "Assets/Materials",
            "Assets/Models",
            "Assets/Parse",
            "Assets/Photon",
            "Assets/Plugins",
            "Assets/Prefabs",
            "Assets/Resources",
            "Assets/Scenes",
            "Assets/Scripts",
            "Assets/Sprites",
            "Assets/TextMesh Pro",
            "Assets/Textures",
            "ProjectSettings/TagManager.asset",
            "ProjectSettings/InputManager.asset"

        };

        AssetDatabase.ExportPackage(exportpaths, packageFile, ExportPackageOptions.Interactive
            | ExportPackageOptions.Recurse |
            ExportPackageOptions.IncludeDependencies | ExportPackageOptions.IncludeLibraryAssets);

        print("Backup Export Complete!");
    }

    [MenuItem("FirebaseGames/Improt BackUp", false, 1)]
    static void action02()
    {
        AssetDatabase.ImportPackage(packageFile, true);
        //CheckMark("BoardGame2D/Improt BackUp");

    }
    [MenuItem("FirebaseGames/Logout",false,11)]
    static void FirebaseLogout()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        auth.SignOut();
    }
    [MenuItem("PlayerPrefs/Delete all", false, 1)]
    static void PlayerPrefsDeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
    [MenuItem("PlayerPrefs/CreatePlayer", false, 2)]
    static void CreatePlayer()
    {
        string s = "";
        for (int i = 0; i < DummyNum - 1; i++)
            s += i.ToString() + '/';
        s += (DummyNum - 1).ToString();
        PlayerPrefs.SetString("Player", s);
    }
    [MenuItem("PlayerPrefs/Create DummyNum Data", false, 3)]
    static void CreateDummyNumData()
    {
        Debug.Log("Preparing function");
        // for (int i = 0; i < DummyNum; i++)
        // {
        //     // PlayerInfo newPlayer = new PlayerInfo(i.ToString(),100);
        // }
    }
    [MenuItem("PlayerPrefs/DebugPlayerString", false, 4)]
    static void DebugPlayerString()
    {
        Debug.Log(PlayerPrefs.GetString("Player", "Player Null"));
    }
    [MenuItem("Create Prefabs/Alert Create", false, 0)]
    static void CreateAlert()
    {
        GameObject alert = Resources.Load("AlertUI") as GameObject;
        Transform transform = GameObject.Find("Canvas").transform;
        Instantiate(alert,transform);
    }
    // [MenuItem("BoardGame2D/All Stone Destroy", false, 11)]
    // static void AllStoneDestroy()
    // {
    //     var stones = FindObjectsOfType<ConcaveStone>();
    //     OmokManager manager = GameObject.Find("GameManager").GetComponent<OmokManager>();

    //     foreach(var stone in stones)
    //     {
    //         manager.SetBoardValue(stone.m_row,stone.m_col,0);
    //         Destroy(stone.gameObject);
    //     }
    // }
    // [MenuItem("Lobby/All Panel true", false, 2)]
    // static void AllPanelOn()
    // {
    //     GameObject parent = GameObject.Find("Canvas");

    //     parent.transform.Find(disconnectPanel).gameObject.SetActive(true);
    //     parent.transform.Find(RobbyPanel).gameObject.SetActive(true);
    //     parent.transform.Find(Settings).gameObject.SetActive(true);

    //     isRobby = isDisconnect  = isSettings = true;
    // } 
    // [MenuItem("Lobby/All Panel false", false, 3)]
    // static void AllPanelOff()
    // {
    //     GameObject parent = GameObject.Find("Canvas");

    //     parent.transform.Find(disconnectPanel).gameObject.SetActive(false);
    //     parent.transform.Find(RobbyPanel).gameObject.SetActive(false);
    //     parent.transform.Find(Settings).gameObject.SetActive(false);

    //     isRobby = isDisconnect  = isSettings = false;
    // } 
    // [MenuItem("Lobby/Init Panel",false, 4)]
    // static void InitPanel()
    // {
    //     GameObject parent = GameObject.Find("Canvas");

    //     parent.transform.Find(disconnectPanel).gameObject.SetActive(true);
    //     parent.transform.Find(RobbyPanel).gameObject.SetActive(false);
    //     parent.transform.Find(Settings).gameObject.SetActive(false);

    //     isDisconnect = true;
    //     isRobby = isSettings = false;
    // }

    // [MenuItem("Lobby/RobbyPanel Switch", false, 15)]
    // static void SwtichRobbyPanel()
    // {        
    //     GameObject parent = GameObject.Find("Canvas");
    //     isRobby = !isRobby;
    //     parent.transform.Find(RobbyPanel).gameObject.SetActive(isRobby);

    //     CheckMark("Lobby/RobbyPanel Switch");
    // }
    // [MenuItem("Lobby/DisConnect Switch", false, 16)]
    // static void SwtichDisConnectPanel()
    // {
    //     GameObject parent = GameObject.Find("Canvas");
    //     isDisconnect = !isDisconnect;
    //     parent.transform.Find(disconnectPanel).gameObject.SetActive(isDisconnect);

    //     CheckMark("Lobby/DisConnect Switch");
    // }

    // [MenuItem("Lobby/Settings Switch",false, 18)]
    // static void SwtichSettingsPanel()
    // {
    //     GameObject parent = GameObject.Find("Canvas");
    //     isSettings = !isSettings;
    //     parent.transform.Find(Settings).gameObject.SetActive(isSettings);

    //     CheckMark("Lobby/Settings Switch");
    // }    
    // static void CheckMark(string path)
    // {
    //     var @check = Menu.GetChecked(path);
    //     Menu.SetChecked(path,!@check); // 아 이미 false인 상황에서 클릭을 할거니깐 반대로 저장해야 되는구나 ㅋ
    // }
}

