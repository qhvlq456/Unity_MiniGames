using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum ECameraView { QuaterView, FPSView }
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    float quaterAngleX = 30f;
    float smoothing = 5f;
    Vector3 offset;

    void Start()
    {
        offset = transform.position - target.position; // 처음엔 무조건 quaterview임
    }

    private void FixedUpdate() {
        QuaterView();        
    }
    
    void QuaterView()
    {
        transform.rotation = Quaternion.Euler(quaterAngleX,transform.rotation.y,transform.rotation.z);
        
        Vector3 newPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position,newPos,smoothing * Time.deltaTime);
    }
}
