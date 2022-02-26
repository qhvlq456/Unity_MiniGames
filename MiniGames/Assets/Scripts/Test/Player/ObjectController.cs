using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [SerializeField]
    JoyStickValue jValue;
    Rigidbody rigid;
    public float speed;
    public float runSpeed;
    public float runGauge;
    public float runConsum;
    public float recoveryRunGaugePerSec;
    const string enemyTag = "Enemy";
    private void Start() {        
        rigid = GetComponent<Rigidbody>();
        
        StartCoroutine(RecoveryRunGauge());
    }
    void FixedUpdate() 
    {
        float h = jValue.jVector.x;
        float v = jValue.jVector.y;

        if(h == 0 && v == 0)
        {
            LookAtTarget();
            return;
        }

        MoveMent(h,v);
        Turning(h,v);
    }
    public void LookAtTarget()
    {
        if(TestManager.manager.target == null) return;

        StartCoroutine(RotationRoutine());
        // Vector3 newPos = TestManager.manager.target.transform.position - transform.position; // 이거 오일러 각이구나
        // newPos.y = 0;
        // Quaternion quaternion = Quaternion.LookRotation(newPos);
        // rigid.MoveRotation(quaternion);
    }
    IEnumerator RotationRoutine()
    {
        Debug.Log("Start Rotation Routine");

        Vector3 newPos = TestManager.manager.target.transform.position - transform.position; // 이거 오일러 각이구나
        newPos.y = 0;
        Quaternion quaternion = Quaternion.LookRotation(newPos);
        rigid.MoveRotation(quaternion);

        while(transform.rotation != quaternion)
        {
            Debug.Log("update quaternion");
            yield return null;
        }

        var shoot = FindObjectOfType<TestShoot>();
        shoot.Shoot();
    }
    private void OnTriggerEnter(Collider other) {
        if(other.tag == enemyTag)
        {
            TestManager.manager.targets.Add(other.gameObject);
        }
    }
    // 여기서 distance연산 해야됨
    private void OnTriggerStay(Collider other) {
        float min = 10000;

        foreach(var t in TestManager.manager.targets)
        {
            float distance = (t.transform.position - transform.position).sqrMagnitude;
            if(min > distance) // 거리는 distance는 크기임
            {
                min = distance;
                TestManager.manager.target = t;
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if(TestManager.manager.targets.Contains(other.gameObject))
        {
            TestManager.manager.targets.Remove(other.gameObject);
        }
        if(TestManager.manager.target == other.gameObject)
        {
            TestManager.manager.target = null;
        }
    }

    void MoveMent(float h, float v)
    {
        // 방향을 정하기 위한 백터
        Vector3 move = new Vector3(h,0,v);
        // 백터를 평준화 시켜 스피드 시간과 곱하기
        move = move.normalized * (speed * runSpeed) * Time.deltaTime;

        rigid.MovePosition(transform.position + move); // 나의 백터와 더해야 하는구나~
    }
    void Turning(float h, float v)
    {
        // joystick의 좌표를 보면 알 수 있지않을까?
        // - + (-1,1), - - (-1,-1), + + (1,1), + - (1,-1)

        // 특정 방향을 쳐다보게 해줌으로써 Object (ie. Player) 의 시선처리를 처리한다.
        // 이때, Vector 값은 target과 현재 위치의 상대위치값 을 할당해야 한다. 
        // 근데 특정 상대위치가 없어서 조이스틱의 방향을 넣었다.

        Quaternion newPos = Quaternion.LookRotation(new Vector3(h,0,v));
        rigid.MoveRotation(newPos);
    }
    // buttons event
    // view 빼고 pressing 되어야 하며, shoot같은 경우 딜레이 필요
    public void Run() // Press
    {
        if(runGauge <= 0) return;

        runSpeed = 2.5f;
        runGauge -= runConsum;
        Debug.Log($"Run Gauge = {runGauge}");
    }
    public void ResetSpeed() // Press Down
    {
        runSpeed = 1;
    }
    IEnumerator TestRoutine()
    {        
        Ray ray = new Ray(transform.position,transform.forward);
        RaycastHit hit;

        for(int i = 1; i <= 360; i++)
        {
            // var eulerPos = transform.rotation.eulerAngles; // 오일러 각을 백터로 반환하는구나~             
            // // Quaternion newPos = Quaternion.Euler(eulerPos.x, i, eulerPos.z);
            // Quaternion newPos = Quaternion.AngleAxis(i,Vector3.up);
            // transform.rotation *= newPos;
            // var pos = Quaternion.Euler(transform.rotation.eulerAngles);
            ray.direction = transform.forward;
            Debug.DrawRay(ray.origin,ray.direction,Color.black,0.5f);

            if(Physics.Raycast(ray,out hit, 30, LayerMask.GetMask("Shootable")))
            {
                if(hit.collider != null)
                {
                    Debug.Log("Hit shootable!!");
                }
            }
            transform.Rotate(new Vector3(0,1,0));
            Debug.Log("rot = " + transform.rotation.eulerAngles);
            yield return new WaitForSeconds(0.25f);
        }
    }
    IEnumerator RecoveryRunGauge()
    {
        float timer = 0f;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        Debug.Log(runGauge);
        if(runGauge < 100)
            runGauge += recoveryRunGaugePerSec;
        StartCoroutine(RecoveryRunGauge());
    }
}
