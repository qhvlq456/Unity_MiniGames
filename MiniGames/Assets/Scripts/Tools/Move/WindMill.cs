using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindMill : MonoBehaviour
{
    float speed;    
    void Start()
    {
        speed = 25.6f;
        StartCoroutine(StartWindMill());
    }
    IEnumerator StartWindMill()
    {
        while (true)
        {
            transform.Rotate(Vector3.forward * speed * Time.deltaTime);
            yield return null;
        }
    }

    
}
