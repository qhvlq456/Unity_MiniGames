using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이거 지움 안됨 대신 로비 그런것들을 지우는게 좋음
public enum ESceneKind
{
    Logo,
    MainMenu,
    Room,
    Lobby,
    Flappy,
    Angry,
    LocalOmok,
    LocalOthello,
    NightMare,
    Omok,
    Othello
}
public enum ESceneType
{
    Menu,
    Game
}
public enum EGameKind
{
    None,
    Horizontal,
    Nightmare,
    BoardGame
}
public enum EGamePlayType
{
    None,
    Local,
    Multi
}
// string key = dic.FirstOrDefault(x => x.Value == 384).Key ;
public class SceneValue
{
    public GameOptions gameOptions {get; private set;} // gameOptions information 
    public string sceneName {get; private set;}
    public int sceneNum {get; private set;} // primary(고유 값)

    public SceneValue(GameOptions gameOptions,string sceneName, int sceneNum)
    {
        this.gameOptions = gameOptions;
        this.sceneName = sceneName;
        this.sceneNum = sceneNum;
    }
}
public class GameOptions
{
    public ESceneType sceneType {get; private set;}
    public EGameKind gameKind {get; private set;}
    public EGamePlayType gamePlayType {get; private set;}
    public GameOptions(ESceneType sceneType, EGameKind gameKind, EGamePlayType isNetwork)
    {
        this.sceneType = sceneType;
        this.gameKind = gameKind;
        this.gamePlayType = isNetwork;
    }
}
public class SceneKind : MonoBehaviour
{
    public static Dictionary<ESceneKind,SceneValue> sceneValue = new Dictionary<ESceneKind, SceneValue>()
    {
        {ESceneKind.Logo, new SceneValue(new GameOptions(ESceneType.Menu,EGameKind.None,EGamePlayType.None),"Logo",0)},
        {ESceneKind.MainMenu, new SceneValue(new GameOptions(ESceneType.Menu,EGameKind.None,EGamePlayType.None),"MainMenu",1)},
        {ESceneKind.Lobby, new SceneValue(new GameOptions(ESceneType.Menu,EGameKind.None,EGamePlayType.None),"Lobby",2)},
        {ESceneKind.Room, new SceneValue(new GameOptions(ESceneType.Menu,EGameKind.None,EGamePlayType.None),"Room",3)},
        {ESceneKind.Flappy, new SceneValue(new GameOptions(ESceneType.Game,EGameKind.Horizontal,EGamePlayType.Local),"Flappy",4)},
        {ESceneKind.Angry, new SceneValue(new GameOptions(ESceneType.Game,EGameKind.Horizontal,EGamePlayType.Local),"Angry",5)},
        {ESceneKind.NightMare, new SceneValue(new GameOptions(ESceneType.Game,EGameKind.Nightmare,EGamePlayType.None),"NightMare",6)},
        {ESceneKind.LocalOmok, new SceneValue(new GameOptions(ESceneType.Game,EGameKind.BoardGame,EGamePlayType.None),"LocalOmok", 7)},
        {ESceneKind.LocalOthello, new SceneValue(new GameOptions(ESceneType.Game,EGameKind.BoardGame,EGamePlayType.Local),"LocalOthello",8)},
        {ESceneKind.Omok, new SceneValue(new GameOptions(ESceneType.Game,EGameKind.BoardGame,EGamePlayType.Multi),"Omok",9)},
        {ESceneKind.Othello, new SceneValue(new GameOptions(ESceneType.Game,EGameKind.BoardGame,EGamePlayType.Multi),"Othello",10)}
    };
}


