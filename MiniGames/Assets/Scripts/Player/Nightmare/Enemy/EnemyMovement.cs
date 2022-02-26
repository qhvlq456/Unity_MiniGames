using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    Transform _player;
    NavMeshAgent _nav;
    PlayerHealth _playerHealth;
    EnemyHealth _enemyHealth;
    public float speed;
    public float angularSpeed;
    // speed를 둬야 되는뎅..
    private void Awake()    
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _nav = GetComponent<NavMeshAgent>();
        _playerHealth = _player.gameObject.GetComponent<PlayerHealth>();
        //_playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _nav.speed = speed;
        _nav.angularSpeed = angularSpeed;
    }

    void Update()
    {
        if(NightmareManager.Manager.gameState != GameState.start) return;
        
        if(!_playerHealth.isDead && !_enemyHealth.isDead)
        {
            if(!_nav.enabled)
                _nav.enabled = true;
        }
        else
            _nav.enabled = false;

        if(_nav.enabled)
        {
           _nav.SetDestination(_player.position); 
        }
    }
}
