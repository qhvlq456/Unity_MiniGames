using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class TestButtons : MonoBehaviour,IPointerUpHandler,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{
    public EButtonType buttonType;
    [SerializeField]
    ObjectController player;
    Action buttonEvent;
    bool isClick = false;
    private void Update() {
        if(isClick) 
            buttonEvent.Invoke();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isClick = false;
        if(buttonType == EButtonType.Run)
            player.ResetSpeed();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isClick = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch(buttonType)
        {
            case EButtonType.Attack : buttonEvent = () => player.transform.GetChild(1).GetComponent<TestShoot>().Shoot(); break;
            case EButtonType.Run : buttonEvent = () => player.Run(); break;
            case EButtonType.View : break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonEvent = null;
    }

}
