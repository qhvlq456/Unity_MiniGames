using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class AngryBird : MonoBehaviour
{
    AngryGameManager GameManager;
    Animator anim;
    SpriteRenderer _renderer;
    [Header("Bird DisappearTime")]
    [Range(0, 10)]
    [ContextMenuItem("Default Value", "SetDefaultValue")]
    public float disAppeartime;
    float leftTime;
    bool isAppear;
    void SetDefaultValue()
    {
        disAppeartime = 3f;
    }
    void Start()
    {
        SetInit();
    }
    private void Update()
    {
        DisAppearBird();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!isAppear) return;
        if (other.collider.gameObject == transform.GetChild(0).gameObject) return;
        anim.SetTrigger("SetDie");
        isAppear = false;
        GameManager.AddScore(10);
    }

    void SetInit()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<AngryGameManager>();
        anim = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        isAppear = true;
    }

    void DisAppearBird()
    {
        if (isAppear) return;
        leftTime += Time.deltaTime;
        if (1 - leftTime / disAppeartime >= 0f)
        {
            _renderer.color = new Color(1, 1, 1, 1 - leftTime / disAppeartime);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
