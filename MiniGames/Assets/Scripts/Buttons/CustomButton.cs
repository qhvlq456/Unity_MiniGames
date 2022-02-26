using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CustomButton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler
{
    Button button;
    AudioSource buttonAudio;
    Vector2 originSize;
    private void Awake() {
        button = GetComponent<Button>();
        buttonAudio = GetComponent<AudioSource>();
    }
    void Start()
    {        
        originSize = transform.localScale;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(eventData.selectedObject == gameObject)
            button.transform.localScale = originSize * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(eventData.selectedObject == gameObject)
            button.transform.localScale = originSize;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.selectedObject == gameObject)
            buttonAudio.Play();
    }
    public void DeleteSound(float time)
    {
        Destroy(gameObject, time);
    }
}
