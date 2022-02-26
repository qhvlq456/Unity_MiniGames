using UnityEngine;

public class ReturnHorizontal : Horizontal
{
    delegate void startType(); // 시작 방향을 위한 델리게이트
    startType type;
    public float x1 { private get; set; }
    public float x2 { private get; set; }
    public float coolTime { private get; set; }
    float leftTime;
    public void SetInit()
    {
        leftTime = 0f;
        InitPosition();
    }
    public void Move()
    {
        type();
    }
    public void SetReturnType(int _type)
    {
        switch (_type)
        {
            case 0:
                type = RightReturnMove;
                break;
            case 1:
                type = LeftReturnMove;
                break;
            default:
                type = RightReturnMove;
                break;
        }
    }
    public void LeftReturnMove()
    {
        leftTime += Time.deltaTime;
        if (1 - leftTime / coolTime <= 0.5f)
            RightMove();
        else
            LeftMove();
    }
    public void RightReturnMove()
    {
        leftTime += Time.deltaTime;
        if (1 - leftTime / coolTime >= 0.5f)
            RightMove();
        else
        {
            LeftMove();
        }
    }
    void InitPosition()
    {
        gameObject.transform.position = new Vector3(x1, transform.position.y, transform.position.z);
    }
}

