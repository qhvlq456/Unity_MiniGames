using UnityEngine;

public class ObjectCamera : ReturnHorizontal
{
    AngryGameManager GameManager;
    Vector2 obstaclePos;
    float startPos;
    public GameObject _ball;
    void Start()
    {
        SetCamera();
    }

    void Update()
    {
        if(GameManager.isGameOver) return;
        
        if (GameManager.gameState == GameState.intro)
        {
            Move();
        }
        else if (GameManager.gameState == GameState.start)
        {
            MoveToBall();
        }
        else // gameState.end
        {
            MoveToObstacle();
        }
    }

    void SetCamera()
    {        
        coolTime = GameVariable.introTime; // 3second
        speed = 20.6f;
        GameManager = GameObject.Find("GameManager").GetComponent<AngryGameManager>();
        _ball = null;

        x1 = startPos = transform.position.x;
        x2 = 20.25f;
        obstaclePos = new Vector2(18.8f, 0); // 이건 다르게 생각해봐야 겠다
        SetInit();
        SetReturnType(0);
        Invoke("InitPosition", GameVariable.introTime); // set InitPosition!!
    }

    void MoveToBall()
    {
        if (_ball == null) return;

        if (_ball.transform.position.x >= startPos &&
            _ball.GetComponent<Rigidbody2D>().velocity.sqrMagnitude > 0)
            transform.position = new Vector3(_ball.transform.position.x,
                transform.position.y, transform.position.z); // vector3 값에 vector2를 넣게되면 z축 값도 들어감ㅋ
    }

    void MoveToObstacle()
    {
        float distance = Vector3.Distance(transform.position, obstaclePos);
        if (distance >= 0)
        {
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(obstaclePos.x, transform.position.y, transform.position.z), Time.deltaTime);
        }
    }
}
