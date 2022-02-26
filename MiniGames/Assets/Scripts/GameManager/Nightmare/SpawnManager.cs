using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] enemy;
    [SerializeField]
    GameObject player;
    [SerializeField]
    Transform[] enemySpawnPosition;
    [SerializeField]
    Transform playerPosition;
    int enemySpawnNum = 20;
    public int[] enemySpawnPercent = new int[] { 12, 8, 0 };
    public float timer;
    float limtedTime = 60f; // 평균 스폰시간 = leftTime/enemyCount
    private void OnEnable() {
        SetPlayerPosition();
        if(NightmareManager.Manager.currentStage > 1)
            SetEnemySpawnPercentage();

        StartCoroutine(StartSpawn());
    }
    private void Update() {
        if(timer < limtedTime)
            timer += Time.deltaTime;
    }
    void SetPlayerPosition()
    {
        Debug.Log("Set player Position");
        player.transform.position = playerPosition.position;
    }
    // 1stage
    // bunny = 3second, bear = 6second, ele = 12second
    // bunny 70% , bear 30%, ele 0%
    // 2stage
    // bunny = 5second, bear = 4second, ele = 9second
    // bunny 60% , bear 30%, ele 10%
    // 3stage
    // bunny = 7second, bear = 2second, ele = 6second 
    // bunny 50% , bear 50%, ele 10%
    public void SetEnemySpawnPercentage()
    {
        for(int i = 0; i < enemySpawnPercent.Length; i++)
        {
            enemySpawnPercent[0] -= 1;

            if(i % 2 == 0) // 코끼리
                enemySpawnPercent[i] += 1;
            else // 베어
                enemySpawnPercent[i] += 1;
        }
    }
    IEnumerator StartSpawn() // 이거 바꿔야 됨!!
    {
        int enemyCount = 0;
        int[] percentage = new int[enemySpawnNum];
        int kind = 1;
        float wait = limtedTime / enemySpawnNum;
        Debug.Log(wait);

        for(int i = 0; i < 3; i++)
        {
            int count = enemySpawnPercent[i]; // int[] enemySpawnPer = new int[] { 7, 3, 0 };
            while(count > 0)
            {
                int index = Random.Range(0,enemySpawnNum);
                if(percentage[index] == 0) 
                {
                    percentage[index] = kind;
                    count--;
                }
            }
            kind++;
        }
        
        while(enemySpawnNum > enemyCount)
        {
            for(int i = 0; i < percentage.Length; i++)
            {
                if(NightmareManager.Manager.isGameOver) yield break;

                Vector3 randomPosition = enemySpawnPosition[Random.Range(0,enemySpawnPosition.Length)].position;
                NightmareManager.Manager.enemys.Add(Instantiate(enemy[percentage[i] - 1],randomPosition,Quaternion.identity));

                yield return new WaitForSeconds(wait);
                enemyCount++;
            }
            yield return null;
        }
    }
}
