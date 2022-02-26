using UnityEngine.EventSystems;
using UnityEngine;
public class TransformStick : JoyStick, IPointerUpHandler, IPointerDownHandler, IDragHandler
{

    public void OnDrag(PointerEventData eventData)
    {
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,eventData.position,eventData.pressEventCamera,out touch))
        {
            touch.x = (touch.x / rect.sizeDelta.x);
            touch.y = (touch.y / rect.sizeDelta.y);
        
            // 이 수식은 pivot에 따라 달라진다 
            touch = new Vector2(touch.x * 2 - 1, touch.y * 2 - 1);
            touch = touch.magnitude > 1 ? touch.normalized : touch;
            handle.anchoredPosition = new Vector2(touch.x * rect.sizeDelta.x / 2, touch.y * rect.sizeDelta.y / 2);
            jValue.jVector = touch;
        }
        // touch = (eventData.position - rect.anchoredPosition) / widthHalf;
        // RangeHandle();
        // handle.anchoredPosition = touch * widthHalf;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // InitializeRect();
        handle.anchoredPosition = Vector2.zero;
        touch = Vector2.zero;
        jValue.jVector = Vector3.zero;
    }
}
