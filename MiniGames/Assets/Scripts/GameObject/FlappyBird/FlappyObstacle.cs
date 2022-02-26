using UnityEngine;

public class FlappyObstacle : MonoBehaviour
{
    public GameObject Column;
    GameObject[] Obstacles;
    const int maxNum = 5;
    const float colPosmax = 3f;
    const float colPosmin = -0.5f;
    const float spawnXpos = 10f;
    const float IntroTime = 3f;
    int idx = 0;
    const float spawnTime = 3f;
    void Start()
    {                
        Column = Resources.Load("Flappy/Obstacle") as GameObject;

        Obstacles = new GameObject[maxNum];
        for (int i = 0; i < Obstacles.Length; i++)
            Obstacles[i] = Instantiate(Column, new Vector2(-15, -15), Quaternion.identity);

        StartSpawn();
    }
    public void StartSpawn()
    {
        InvokeRepeating("SpawnObstacle", IntroTime, spawnTime);
    }

    void SpawnObstacle()
    {
        float _colPosition = Random.Range(colPosmin, colPosmax);
        Obstacles[idx].transform.position = new Vector2(spawnXpos, _colPosition);
        idx = (idx + 1) % Obstacles.Length;
    }
}
