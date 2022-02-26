using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
public enum EButtonType
{
    Attack,
    Run,
    View
}
public class PlayerButtons : MonoBehaviour,IPointerUpHandler,IPointerDownHandler
{
    public EButtonType buttonType;
    [SerializeField]
    GameObject player;

    Action buttonEvent;
    bool isClick = false;
    private void Awake() {
        switch(buttonType)
        {
            // case EButtonType.Attack : buttonEvent = () => player.transform.GetChild(1).GetComponent<PlayerShooting>().OnClickShoot(); break;
            case EButtonType.Run : buttonEvent = () => player.GetComponent<PlayerMovement>().IsRun(); break;
        }
    }
    private void Update() {
        if(isClick) 
            buttonEvent.Invoke();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isClick = false;

        if(buttonType == EButtonType.Run)
            player.GetComponent<PlayerMovement>().ResetSpeed();    
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isClick = true;
    }

}
