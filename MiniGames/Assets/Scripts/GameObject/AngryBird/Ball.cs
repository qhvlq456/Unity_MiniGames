using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    AngryGameManager GameManager;
    public GameObject _lineRenderer { private get; set; }
    GameObject _line;
    bool _isShowline;
    public float max_Length { private get; set; }
    [Header("Physics Force Range")]
    [Range(0,300)]
    public float _force; 
    bool isClick;
    Ray _ray;
    SpringJoint2D _spring;
    Rigidbody2D _rigid;
    public Vector3 pivotPos { private get; set; }
    Vector3 prevVelocity;
    void Start()
    {
        gameObject.name = "Ball";        
        max_Length = 3f;        
        _isShowline = true;
        isClick = false;

        _lineRenderer = Resources.Load("Angry/Line") as GameObject;

        GameManager = GameObject.Find("GameManager").GetComponent<AngryGameManager>();
        pivotPos = GameObject.Find("CatapultPivot").transform.position;

        _rigid = GetComponent<Rigidbody2D>();
        _spring = GetComponent<SpringJoint2D>();
        _ray = new Ray(pivotPos, Vector3.zero);

        ObjectCamera _objectCamera = GameObject.Find("Main Camera").GetComponent<ObjectCamera>();
        _objectCamera._ball = gameObject;

        _isShowline = true;

        _line = Instantiate(_lineRenderer, new Vector2(0f, 0f), Quaternion.identity);
    }

    void Update()
    {
        if (GameManager.gameState != GameState.start) return;        

        if(Input.GetMouseButtonDown(0))
            IsButtonDown();
        if (Input.GetMouseButton(0))
            IsButton();
        if (Input.GetMouseButtonUp(0))
            IsButtonUp();

        OnDestroySpring();
        DestroyBall();
    }
    void DestroyBall()
    {
        if (_rigid.velocity.sqrMagnitude <= 0f && _spring == null)
            EndObject();
    }
    public void IsButtonDown()
    {
        if (isClick) return;
        _rigid.velocity = Vector2.zero;
        _rigid.AddForce(Vector2.up * _force);
    }
    public void EndObject()
    {
        GameManager.OnPassStart();
        Destroy(gameObject);
    }
    public void IsButton()
    {
        if (isClick) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 disVec = mousePos - pivotPos; // distance 구하기
        if (disVec.sqrMagnitude > max_Length * max_Length)
        {
            _ray.direction = disVec; // 방향 설정
                                     // 1. Debug.DrawRay(ray.origin, ray.direction, Color.red);
            mousePos = _ray.GetPoint(max_Length); // ray를 따라 /distance/의 거리에 지점을 반환합니다. /distance/가 매개변수임, ray거리 반환
                                                  // 2. Debug.DrawRay(ray.origin, ray.direction, Color.red);

            // # debug의 두길이의 방향은 같다 결국은 disvec의 방향을 알고싶은것 vector라 방향 값을 가지고 있기 때문에
            // # mousePos = ray.GetPoint(maxLength) 이 부분은 ray의 방향과 max를 mousePos에 전해주기 위해 그런듯..?
        }
        transform.position = mousePos; // 난 등신인가?
        UpdateLine();
        
    }    
    public void IsButtonUp()
    {
        if (isClick) return;

        // SoundManager.instance.PlayClip(EEffactClipType.Angry);

        isClick = true;
        _rigid.bodyType = RigidbodyType2D.Dynamic; // 이런 타입도 enum으로 존재하는구나

        _isShowline = false;

        Destroy(_line);
        
        // DeleteLine();
    }
    void OnDestroySpring()
    {
        if (_spring != null)
        {
            if (prevVelocity.sqrMagnitude > _rigid.velocity.sqrMagnitude)
            {
                _rigid.mass = 4;
                Destroy(_spring);// component만 없앨수 있는거구나
                _rigid.velocity = prevVelocity; // 전 속력을 내가 갖는다
            }
            if (isClick) prevVelocity = _rigid.velocity;
        }
    }
    void UpdateLine()
    {
        if (!_isShowline) return;
        _line.transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(1, transform.position);
        _line.transform.GetChild(1).GetComponent<LineRenderer>().SetPosition(1, transform.position);
    }

    void DeleteLine()
    {
        _isShowline = false;

        Destroy(_line);
    }

    // void InitLine()
    // {
    //     _isShowline = true;

    //     _line = Instantiate(_lineRenderer, new Vector2(0f, 0f), Quaternion.identity);
    // }
}
