using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    const float timeBetweenAttacks = 0.5f; // 무한공격 방지 딜레이
    public int attackDamage; // 2, 5, 10
    GameObject player;
    PlayerHealth playerHP;
    bool playerInRange = false; // 플레이어 공격범위 여부
    float timer; // 무한공격 방지 타이머

    Animator animator;

    private void Awake() {
        player = GameObject.Find("Player");
        playerHP = player.GetComponent<PlayerHealth>();
        animator = GetComponent<Animator>();

    }
    private void Update() {
        if(NightmareManager.Manager.gameState != GameState.start) return;

        timer += Time.deltaTime;
        if(timer >= timeBetweenAttacks && playerInRange)
            Attack();
        
        if(playerHP.isDead)
            animator.SetTrigger("PlayerDead");
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject == player)
            playerInRange = true;
    }
    private void OnTriggerExit(Collider other) {
        if(other.gameObject == player) 
            playerInRange = false; // 대체 이건 나간건 어떻게 아는거야?
    }
    void Attack()
    {
        timer = 0f;
        if(!playerHP.isDead)
            playerHP.TakeDamage(attackDamage);
    }
}
