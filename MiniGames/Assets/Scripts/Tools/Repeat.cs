using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class Repeat : MonoBehaviour
{
    // 부모한테 주면안됨 특히 자식이 하나 있을경우 // 근데 부모는 꼭 필요
    // 그러면 default값이 안됨;    
    public delegate void StartRepeat();
    public enum direction { left, right }
    direction dir;
    StartRepeat startrepeat;
    Vector2 setStartpos;
    public int length;
    public float boxSize { get; set; } // 제일 첫 빠따
    int backgroundNum; // default value = 2;
    
    void Start()
    {
        boxSize = GetComponent<BoxCollider2D>().size.x;
        SetInit();
        SetDirection(0);
    }

    void Update()
    {
        startrepeat();
    }    

    public void SetInit() // 1-2. start set start
    {
        if (transform.parent) SetChildNum(transform.parent.gameObject);
    }

    void Left() // Func Mount alpha
    {
        if (transform.position.x < -boxSize * length)
        {
            transform.position = (Vector2)transform.position + setStartpos;
        }

    }
    void Right() // Func Mount alpha2
    {
        if (transform.position.x > boxSize * length)
        {
            transform.position = (Vector2)transform.position + setStartpos;
        }
    }

    public void SetDirection(int dir) // 1
    {
        switch (dir)
        {
            case (int)direction.left:
                {
                    this.dir = direction.left;
                    setStartpos = new Vector2(boxSize * backgroundNum, 0);
                }
                break;
            case (int)direction.right:
                {
                    this.dir = direction.right;
                    setStartpos = new Vector2(-1 * boxSize * backgroundNum, 0);
                }
                break;
        }
        SetFuncMount();
    }
    public void SetFuncMount() // 3
    {
        switch (dir)
        {
            case direction.left: startrepeat = Left; break;
            case direction.right: startrepeat = Right; break;
        }
    }
    void SetChildNum(GameObject obj) // 1-3
    {
        backgroundNum = obj.transform.childCount; // 결국 2개 있을때 boxsize로 가야하기 때문에 -1을 해야함             
        InitSetPosition(obj);
    }

    void InitSetPosition(GameObject parent) // 1-4 End start set
    {
        GameObject obj;
        float accumulatePos = 0f;
        int dir = this.dir == direction.left ? dir = 1 : dir = -1;
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            obj = parent.transform.GetChild(i).gameObject;
            obj.transform.position = new Vector2(accumulatePos * dir, transform.position.y);
            accumulatePos += boxSize;
        }
    }
}

