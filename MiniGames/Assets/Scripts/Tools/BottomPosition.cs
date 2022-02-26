using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomPosition : MonoBehaviour
{
    [Header("bottom size")]
    [ContextMenuItem("DefaultValue","SetDefaultValue")]
    [Range(0,30)]    
    public float boxsize;
    [Header("Bottom Number")]
    [Range(2,20)]    
    public int boxNum;

    Vector2 _transform;
    GameObject _bottom;
    void SetDefaultValue()
    {
        boxsize = 5.12f;
        boxNum = 17;
    }
    private void Start()
    {
        SetInit();
        SetStartPosition();
    }    
    void SetInit()
    {
        _bottom = Resources.Load("Angry/Bottom") as GameObject;
        _transform = new Vector2(0, -2.56f);
    }
    void SetStartPosition()
    {
        float leftPos, rightPos;        
        rightPos = boxsize; leftPos = -rightPos;

        Instantiate(_bottom, new Vector2(_transform.x,_transform.y),Quaternion.identity);
        for(int i = 0; i < boxNum / 2; i++)
        {
            Instantiate(_bottom, new Vector2(rightPos, _transform.y), Quaternion.identity);
            Instantiate(_bottom, new Vector2(leftPos, _transform.y), Quaternion.identity);
            rightPos += boxsize;
            leftPos = -rightPos;
        }
    }
}
