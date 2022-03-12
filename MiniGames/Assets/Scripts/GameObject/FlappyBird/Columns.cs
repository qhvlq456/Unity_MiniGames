using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Columns : MonoBehaviour
{
    [Header("Add Score Range")]
    [Range(0,100)]
    public int score;
    BoxCollider2D _box;
    FlappyGameManager GameManager;
    bool isDelay = false;
    void Start()
    {
        _box = GetComponent<BoxCollider2D>();
        _box.isTrigger = true;
        GameManager = GameObject.Find("GameManager").GetComponent<FlappyGameManager>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(GameManager.isGameOver) return;
        if(isDelay) return;

        if (other.GetComponent<FlappyBird>() != null) 
        {
            isDelay = true;
            GameManager.UpdateScore(score);
            Invoke("UpdateScoreDelay", 0.4f);
        }
    }
    void UpdateScoreDelay()
    {
        isDelay = false;
    }
}
