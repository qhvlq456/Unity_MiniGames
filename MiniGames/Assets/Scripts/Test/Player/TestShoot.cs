using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShoot : MonoBehaviour
{
    [SerializeField]
    JoyStickValue jValue;
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    public GameObject test;
    public float range;
    public float delay;
    public float shootTime;
    public float effectTime;
    int mask;
    private void Awake() {
        mask = LayerMask.GetMask("Shootable");
        gunLine = GetComponent<LineRenderer>();
        gunParticles = GetComponent<ParticleSystem>();
        shootTime = 0f;
        delay = 0.25f;
        effectTime = 0.15f;
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        shootTime += Time.deltaTime;

        if(shootTime >= delay * effectTime && gunLine.gameObject.activeSelf)
        {
            // Debug.Log($"Effect time = {delay * effectTime}");
            EffectOff();
        }
    }
    /*
        1. 가만히 있을때 즉, 백터 크기가 zero이거나 velocity가 zero 일때 탐색
        2. 가까운 적을 목표로 탐지
        3. 가까운 적의 방향으로 방향 변경
        4. 갈등인게 적이 죽을때 까지 타겟을 변경하냐 아님 그냥 가까운 적의 기준에 맞추어 target을 변경해야 되나가 문제
        5. 둘다 해보기 ㅋ
        range = target을 탐색할 범위 ray를 한바퀴 돌려서 적들을 배열로 받아야지
        반복문을 360번 돌리면 360도로 방향으로 회전 가능하지 않나?
        // 아니 이거 그냥 콜라이더 두는게 더 좋은거 같은데 그냥 테스트를 한번만 해볼까..?
        // interval이 넘사야..
    */
    public void Shoot()
    {
        if(shootTime < delay) return;

        shootTime = 0;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        
        gunParticles.Stop();
        gunParticles.Play();

        gunLine.enabled = true;
        gunLine.SetPosition(0,transform.position); // 시각적 선의 출발지점

        if(Physics.Raycast(ray,out hit,50f,mask))
        {
            TestEnemy enemy = hit.collider.GetComponent<TestEnemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(20);
            }
            gunLine.SetPosition(1,hit.point);
        }
        else
        {
            gunLine.SetPosition(1,ray.origin + ray.direction * 50);
        }
    }
    void EffectOff()
    {
        gunLine.enabled = false;
    }
}
