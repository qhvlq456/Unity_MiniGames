using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameVariable : MonoBehaviour
{
    public static string gameVersion = "1.4.0";
    public static string logoTitle = "Mini Games";
    
    // Flappy Bird
    public static float blinkTime = 3f;
    public static float destroyTime = 2.5f;
    // Angry Bird
    public static float startTime = 10f;
    public static float endTime = 3.5f;
    public static int birdCount = 3;
    // common
    public static float introTime = 3f;
    public static float speed = 13.6f;
    public static float fadeTime = 2.0f;
    public static int consumCoin = 20;
    public static int addCoin = 10;
    
    // board game
    public static int sequenceNum = 0;
    public static int omokBoardNum = 17;
    public static int othelloBoardNum = 8;
    public static int sequneceNum = 0;
}
