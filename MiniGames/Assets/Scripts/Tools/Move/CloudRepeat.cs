using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudRepeat : MonoBehaviour
{
    [SerializeField]
    GameObject[] cloud;
    float startXPos;
    private void Awake() {
        startXPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        CheckOverScreen();
    }
    void CheckOverScreen()
    {
        foreach(var o in cloud)
        {
            if(Camera.main.WorldToViewportPoint(o.transform.position).x < 0f)
            {
                o.transform.position = new Vector3(startXPos,o.transform.position.y,o.transform.position.z);
            }
        }
    }
}
