using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    [Header("Intro Delete Time")]
    [Range(0, 300)]
    [ContextMenuItem("DefaultValue", "SetDefaultValue")]    
    public float deleteTime;
    [Header("Intro speed")]
    public float speed;
    public float timer = 0;
    public int originalFontSize = 200;
    
    void SetDefaultValue()
    {
        // maxLocalscale = 5f;
        speed = 200;
        deleteTime = 1f;
    }
    private void OnEnable() {
        gameObject.GetComponent<Text>().fontSize = originalFontSize;
        timer = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= deleteTime)
            SetDisableObject();
        else
            StartLocalScale();
    }
    void SetDisableObject()
    {
        gameObject.SetActive(false);
    }
    public void StartLocalScale()
    {
        gameObject.GetComponent<Text>().fontSize -= (int)(Time.deltaTime * speed); // font깨짐으로 인한 fontsize변경
    }
}
