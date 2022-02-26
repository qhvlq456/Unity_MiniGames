using UnityEngine;
using UnityEngine.UI;

public class Horizontal : MonoBehaviour
{
    public float speed { private get; set; }
    public void LeftMove()
    {
        transform.Translate(new Vector2(-1 * Time.deltaTime * speed, 0));
    }
    public void RightMove()
    {
        transform.Translate(new Vector2(Time.deltaTime * speed, 0));
    }
}

