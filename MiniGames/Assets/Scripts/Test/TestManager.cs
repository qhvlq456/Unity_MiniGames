using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TestManager : MonoBehaviour
{
    public static TestManager manager = null;
    public GameObject target;
    public List<GameObject> targets = new List<GameObject>();

    private void Awake() {
        manager = this;
    }
}
