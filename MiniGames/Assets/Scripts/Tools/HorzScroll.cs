using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HorzScroll : Horizontal
{
    [Header("Stream Speed")]
    [Range(0,20)]
    public float _speed;

    private void Start()
    {        
        speed = _speed;
    }
    void Update()
    {
        LeftMove();
    }


}
