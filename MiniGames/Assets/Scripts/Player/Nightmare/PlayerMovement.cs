using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    JoyStickValue jValue;
    float startingRunGauge = 50f;
    float consumRun = 0.5f;
    float recoveryRun = 0.4f;
    public float speed = 4f;
    public float runSpeed = 7.5f;
    public float applySpeed;
    public bool isRun = false;
    public Animator animator;
    public Rigidbody rigid;
    Slider runSlider;
    Vector3 movement;
    int floorMask;

    private void Awake() {
        floorMask = LayerMask.GetMask("Floor");
        runSlider = GameObject.Find("RunSlider").GetComponent<Slider>();

        runSlider.maxValue = startingRunGauge;
        runSlider.value = startingRunGauge;

        applySpeed = speed;
    }
    private void Start() {
        StartCoroutine(RecoveryRunGauge());
    }
    private void FixedUpdate() {
        if(NightmareManager.Manager.isGameOver) return;

        if(isRun)
            runSlider.value -= consumRun;

        float h = jValue.jVector.x;
        float v = jValue.jVector.y;

        Animating(h,v);

        if(h == 0 && v == 0)
        {
            LookAtTarget();
            return;
        }
        else
        {
            MoveMent(h,v);
            NightmareManager.Manager.target = null;
        }
    }
    void LookAtTarget()
    {
        float min = 10000;

        float range = 95;

        foreach(var t in NightmareManager.Manager.enemys)
        {
            float distance = (t.transform.position - transform.position).sqrMagnitude;
            
            if(distance > range) continue;

            if(min > distance)
            {
                min = distance;
                NightmareManager.Manager.target = t;
            }
        }
        
        if(NightmareManager.Manager.target != null)
            StartCoroutine(RotationRoutine());
    }
    IEnumerator RotationRoutine()
    {
        Vector3 newPos = NightmareManager.Manager.target.transform.position - transform.position;
        newPos.y = 0;
        Quaternion quaternion = Quaternion.LookRotation(newPos);
        rigid.MoveRotation(quaternion);

        while(transform.rotation != quaternion)
        {
            yield return null;
        }

        var shoot = transform.GetComponentInChildren<PlayerShooting>();
        shoot.Shoot();
    }
    void MoveMent(float h, float v)
    {
        Move(h,v);
        Turning(h,v);
    }
    void Move(float h, float v)
    {        
        // 1. 방향 변경
        movement.Set(h,0,v);
        // 2. 방향에 대한 크기 1로 만들어 크기 결정
        movement = movement.normalized * applySpeed * Time.deltaTime; // 이게 거리니깐.. 그니깐 나의 포지션과 더한 거리니깐 이걸 작게 해야 부드럽게 되지
        // fixedupdate에서 계속 호출 되는건데
        // 3. 크기와 방향이 결정 되었음으로 다음 프레임 이동
        rigid.MovePosition(transform.position + movement);         // velocity의 영향을 받지 않음ㅋㅋ
    }
    public void IsRun()
    {
        if(runSlider.value <= 0) 
        {
            ResetSpeed();
            return;
        }

        isRun = true;
        applySpeed = runSpeed;
    }
    public void ResetSpeed()
    {
        isRun = false;
        applySpeed = speed;
    }
    void Animating(float h, float v)
    {
        bool walking = (h != 0) || (v != 0);
        animator.SetBool("IsWalking",walking);
    }
    void Turning(float h, float v)
    {
        Quaternion quaternion = Quaternion.LookRotation(new Vector3(h,0,v));
        rigid.MoveRotation(quaternion);
    }
    
    IEnumerator RecoveryRunGauge()
    {
        yield return new WaitForSeconds(0.1f);
        runSlider.value += recoveryRun;
        StartCoroutine(RecoveryRunGauge());
    }
}

