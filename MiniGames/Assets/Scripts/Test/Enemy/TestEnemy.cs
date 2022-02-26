using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestEnemy : MonoBehaviour
{
    [SerializeField]
    Slider slider;
    [SerializeField]
    Transform target;
    public float hp;
    public float damage;

    private void Awake() {
    }
    private void Update() {
        
        // Vector3 newPos = Camera.main.transform.position;
        // slider.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back, 
        // Camera.main.transform.rotation * Vector3.down);
        // 여기서 target은 camera임;; 그니깐 tareget - slider임 
        Transform parent = slider.transform.parent.transform;
        // Vector3 newPos = target.position - parent.position;
        
        // Quaternion quaternion = Quaternion.LookRotation(newPos);
        // slider.transform.parent.transform.rotation = quaternion;
        

        parent.LookAt(Camera.main.transform);

    }
    void Start()
    {
        slider.maxValue = hp;
        slider.value = hp;        
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        slider.value = hp;
        if(hp <= 0) 
        {
            if(TestManager.manager.targets.Contains(gameObject))
            {
                TestManager.manager.targets.Remove(gameObject);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("not contains gameobject");
            }
        }
    }
}
