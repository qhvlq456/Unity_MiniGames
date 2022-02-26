using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryObstacle : MonoBehaviour
{
    const int bird_Count = 3;
    const float xInterval = 2f, yInterval = 2.1f;
    const float startXpos = 12.8f, startYpos = -0.75f;
    // Random obstacle rectangle
    GameObject plank, bird;
    List<GameObject> randomList;// obstacle random arrangement
    [Range(0,20)]
    [ContextMenuItem("Default Value", "SetDefaultValue")]
    public int row, col;
    float[,] randomArrPos; // △, □, ◇, ▷
    void SetDefaultValue()
    {
        row = col = 7;
    }
    private void Start()
    {
        SetInit();
        StartObstacle();
    }
    void SetInit()
    {
        randomList = new List<GameObject>();        
        plank = Resources.Load("Angry/Plank") as GameObject;
        bird = Resources.Load("Angry/Bird") as GameObject;
        InitAngryObstaclePos();
    }
    public void StartObstacle()
    {
        int _random = Random.Range(0, 3);
        DestroyObstacle();
        SpawnObstacle(_random);
        CreateBird();
    }
    void InitAngryObstaclePos()
    {
        /* ymax = 4.5f, ymin = 0.2f, yinterval = 2.4f 
        * xmax = 20.48f, xmin = 0, xinterval = 2.2f 
        */
        randomArrPos = new float[2, col]; // 난 pos만 알면되니깐 0 = r , 1 = c
        randomArrPos[0, 0] = startYpos; randomArrPos[1, 0] = startXpos;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 1; j < col; j++)
            {
                randomArrPos[i, j] += randomArrPos[i, j - 1];
                randomArrPos[i, j] += i == 0 ? yInterval : xInterval;
            }
        }

    }
    // midpos를 중심으로 행과열을 고정시켜 내가 생각한 별찍기를 람다식으로 넣어 랜덤으로 생성하자,, 아님 델리게이트
    // nono, 갠적으로 interface만들어서 클래스 상속시키고 다향성으로 호출하는게 좋음
    public void SpawnObstacle(int rand) // 이 rand값을 이 클래스 내에서 호출과 동시에 자동으로 랜덤값 생성하기!!
    {
        // 별찍기 들어가즈아 9x9 찍기 개오랜만이넹        
        switch (rand)
        {
            case 0: // 이건 삼각형
                {
                    for (int i = 0; i < row; i++)
                    {
                        for (int j = i; j < col - i; j++)
                        {
                            randomList.Add(Instantiate(plank, new Vector2(randomArrPos[1, j], randomArrPos[0, i]), Quaternion.identity));
                        }
                    }
                }
                break;
            case 1: // 이건 왼쪽이 큰 직삼각형
                {
                    for (int i = 0; i < row; i++)
                    {
                        for (int j = 0; j < col - i; j++)
                        {
                            randomList.Add(Instantiate(plank, new Vector2(randomArrPos[1, j], randomArrPos[0, i]), Quaternion.identity));
                        }
                    }
                }
                break;
            case 2:// 이건 오른쪽이 큰 직삼각형
                {
                    for (int i = 0; i < row; i++)
                    {
                        for (int j = col - 1; j >= i; j--)
                        {
                            randomList.Add(Instantiate(plank, new Vector2(randomArrPos[1, j], randomArrPos[0, i]), Quaternion.identity));
                        }
                    }
                }
                break;
            case 3: // 이건 다이아
                {
                    int mid = row / 2; // 5
                    for (int i = 0; i <= mid; i++)
                    {
                        for (int j = mid + i; j >= mid - i; j--)
                        {
                            randomList.Add(Instantiate(plank, new Vector2(randomArrPos[1, j], randomArrPos[0, i]), Quaternion.identity));
                        }
                    }
                    for (int i = mid + 1; i < col; i++) // 이건 다이아 // 6,7,8,9 || 5
                    {
                        for (int j = i - mid; j < col - (i - mid); j++) // 저 위에 껏만 보면 단순하게 생각하면 쉽게 풀린거였는데 내가 등신이네
                        {
                            randomList.Add(Instantiate(plank, new Vector2(randomArrPos[1, j], randomArrPos[0, i]), Quaternion.identity));
                        }
                    }
                }
                break;
        }
    }
    public void CreateBird() // bird위치 즉, 인덱스 생성
    {        
        List<int> storageIdx = new List<int>();
        while (storageIdx.Count != bird_Count)
        {
            int random = Random.Range(0, randomList.Count);
            if (storageIdx.Contains(random)) continue;
            storageIdx.Add(random);
        }
        ArragementRandomPosition(storageIdx); // bird create
    }
    void ArragementRandomPosition(List<int> list)
    {                
        for (int i = 0; i < bird_Count; i++)
        {
            float xPos, yPos;
            xPos = randomList[list[i]].transform.GetChild(0).transform.position.x; // 와 고민 많이 했는데 결국은 자식좌표 그냥 쓰면 되는거엿네? 왜 반으로 나누면 안되는거지?
            yPos = randomList[list[i]].transform.GetChild(1).transform.position.y; // 자식만 알면 되지않나? 어차피 월드좌표로 찍히는거 아냐?                    
            randomList.Add(Instantiate(bird, new Vector2(xPos, yPos), Quaternion.identity));
        }
    }    
    public void DestroyObstacle()
    {
        if (randomList.Count == 0) return;
        foreach (var i in randomList)
            Destroy(i);
        randomList.Clear();
    }
}
