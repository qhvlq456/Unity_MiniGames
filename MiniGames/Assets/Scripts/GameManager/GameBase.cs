using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameBase : MonoBehaviour
{
    public int score;
    public Text scoreText { protected get; set; }
    public ESceneKind gameKind;
    public bool isGameOver { get; protected set; }
    public GameBase()
    {
        isGameOver = false;
        score = 0;
    }
    public virtual void UpdateScore(int _score)
    {
        score += _score;
    }
    public virtual void CurrentText()
    {
        scoreText.text = $"Score : {score}";
    }
    public void IsGameOver()
    {
        isGameOver = true;
    }
}

