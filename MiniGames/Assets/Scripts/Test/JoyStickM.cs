using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStickM : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    RectTransform rect;
    [SerializeField]
    RectTransform handle;
    Vector2 touch = Vector2.zero; // (0,0) , (1,0)
    [SerializeField]
    JoyStickValue jValue;

    float widthHalf;
    private void Start() {
        rect = GetComponent<RectTransform>();
        widthHalf = rect.sizeDelta.x * 0.5f;
    }
    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log($"rect anchoredPosition = {rect.anchoredPosition}");
        // Debug.Log($"Drag Touch position = {touch}");
        Debug.Log((Vector2)rect.position); // (Vector2)rect.position // worldspace상 pivot의 위치이구나 ㅎ
        touch = (eventData.position - (Vector2)rect.position) / widthHalf; // 이것 자체가 잘 못이 있어서 발생하는 이슈임

        if(touch.magnitude > 1)
            touch = touch.normalized;
        jValue.jVector = touch;

        handle.anchoredPosition = touch * widthHalf;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        jValue.jVector = Vector3.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}
