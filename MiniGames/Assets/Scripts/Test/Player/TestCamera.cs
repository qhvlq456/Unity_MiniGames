using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
    public GameObject player;
    Vector3 offset;
    public float speed;
    public float distance;
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        FollowObject();
    }
    
    void FollowObject()
    {
        Vector3 newPos = player.transform.position + offset;
        // y축 빼고 움직여야 함
        transform.position = Vector3.Lerp(transform.position,newPos,Time.deltaTime * speed);
    }
}
