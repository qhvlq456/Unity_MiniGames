using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "JoyStick")]
public class JoyStickValue : ScriptableObject
{
    public Vector3 jVector;
    public Quaternion jQuaternion;
}
