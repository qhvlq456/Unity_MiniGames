using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStick : MonoBehaviour
{
    protected RectTransform rect;
    [SerializeField]
    protected JoyStickValue jValue;
    [SerializeField]
    protected RectTransform handle;
    protected Vector2 touch = Vector2.zero;
    protected float widthHalf;
    public virtual void Start() {
        rect = GetComponent<RectTransform>();
        widthHalf = rect.sizeDelta.x * 0.5f;
    }
    public virtual void RangeHandle()
    {
        if(touch.magnitude > 1)
            touch = touch.normalized;
        jValue.jVector = touch;
    }
    public virtual void InitializeRect()
    {
        jValue.jVector = Vector3.zero;
        handle.anchoredPosition = Vector2.zero;
    }

}
