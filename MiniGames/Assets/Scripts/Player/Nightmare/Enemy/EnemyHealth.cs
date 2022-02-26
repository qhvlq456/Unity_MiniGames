using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public class EnemyHealth : MonoBehaviour
{
    int startingHealth = 100;
    int currentHealth;
    float sinkSpeed = 2.5f; // 땅으로 꺼지는 속도
    public AudioClip deathClip;

    Animator animator;
    AudioSource enemyAudio;
    ParticleSystem hitParticles;
    CapsuleCollider capsuleCollider;
    public Slider enemyHealthSlider;
    public bool isDead;
    bool isSinking; // 땅으로 꺼지는 여부
    public int deadScore;
    public int enemyIndex;
    private void Awake() {
        currentHealth = startingHealth;

        animator = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();
        hitParticles = GetComponentInChildren<ParticleSystem>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        enemyHealthSlider = GetComponentInChildren<Slider>(); // 정말 GetComponentInChildren 좋구나!! 자식의 자식의 자식까지 찾아버리넹

    }
    private void Start() {
        enemyHealthSlider.maxValue = startingHealth;
        enemyHealthSlider.value = startingHealth;
    }

    void Update()
    {
        enemyHealthSlider.transform.LookAt(Camera.main.transform);

        if(isSinking)
            transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
    }

    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        if(isDead) return;

        enemyAudio.Play();
        currentHealth -= amount;
        enemyHealthSlider.value = currentHealth;

        hitParticles.transform.position = hitPoint; // 피격지점으로 위치 선정
        hitParticles.Play();

        if(currentHealth <= 0)
            Death();
    }
    public void Death()
    {
        isDead = true;

        capsuleCollider.isTrigger = true;
        enemyHealthSlider.enabled = false;
        
        animator.SetTrigger("Dead");

        enemyAudio.clip = deathClip;
        enemyAudio.Play();

        if(NightmareManager.Manager.target == gameObject)
        {
            NightmareManager.Manager.target = null;
        }
        
        NightmareManager.Manager.AddScore(deadScore);
        NightmareManager.Manager.EnemyKills(enemyIndex);
        NightmareManager.Manager.RemoveEnemys(gameObject);
    }

    // clip에서 animation으로 구성된 event함수 public으로 구성한다
    public void StartSinking()
    {
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        isSinking = true;
        Destroy(gameObject,2f);
    }
}
