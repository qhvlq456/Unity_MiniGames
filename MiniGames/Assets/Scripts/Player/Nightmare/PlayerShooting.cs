using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{        
    [SerializeField]
    JoyStickValue jValue;
    public float timeBetweenBullets = 0.15f;
    public float range = 100f;
    //int damagePerShot = 20;
    float timer;
    Ray shootRay = new Ray(); // 공격을 위한 레이저
    RaycastHit shootHit; // 충돌 물체의 정보 저장
    int shootableMast; // 물체 필터
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight; // 총구 밝아지는 효과
    float effectDisplayTime = 0.2f; // 총 한발의 이펙트 유지시간

    private void Awake() {
        shootableMast = LayerMask.GetMask("Shootable");
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();
    }    
    void FixedUpdate()
    {   
        timer += Time.deltaTime;

        if(timer >= timeBetweenBullets * effectDisplayTime)
            DisableEffects();
    }
    public void DisableEffects()
    {
        gunLight.enabled = false;
        gunLine.enabled = false;
    }

    public void Shoot()
    {   
        if(NightmareManager.Manager.gameState != GameState.start) return;
        if(jValue.jVector.x != 0 || jValue.jVector.y != 0) return;
        
        if(timer < timeBetweenBullets) return;

        timer = 0f;
        
        gunAudio.Play();
        gunLight.enabled = true;

        gunParticles.Stop();
        gunParticles.Play();

        gunLine.enabled = true;
        gunLine.SetPosition(0,transform.position); // 시각적 선의 출발지점

        shootRay.origin = transform.position; // 빔이 시작되는 지점 // 여기 버그임 가까이서 때림 안맞음ㅋㅋ
        shootRay.direction = transform.forward; // 빔이 발사되는 지점 여기선 z축임

        
        // 빔을 range까지 발사하여 빔을 맞은 물체중 shootableMast로 필터링된 정보를 shootHit 저장
        if(Physics.Raycast(shootRay,out shootHit, range, shootableMast))
        {
            EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();
            if(enemyHealth != null)
            {
                int damage = 20; // 나중에 생각해서 고칠거
                enemyHealth.TakeDamage(damage,shootHit.point);
            }
            // 시각적 효과의 종료지점
            gunLine.SetPosition(1,shootHit.point);
        }
        else
        {
            // 맞은 물체가 없다면 그냥 range까지 시각적 효과
            gunLine.SetPosition(1,shootRay.origin + shootRay.direction * range);
        }
    }
}

