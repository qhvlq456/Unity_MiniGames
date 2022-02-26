using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButtlet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        // if(other.tag == "Enemy")
        Debug.Log("Collider!!!");
    }
    public void DeleteTime(float time = 3f)
    {
        Destroy(gameObject,time);
    }
}
