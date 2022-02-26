using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField]
    GameObject numbering;
    [SerializeField]
    Transform board;
    GameObject _numbering;
    GameObject _line;    
    GameObject line;
    Vector2 startPos;
    float x = -4f;
    float y = 4f; 
    float interval = 0.5f;

    void Start()
    {
        SetInit();
        SetLinePosition();
    }
    
    void SetInit()
    {
        line = Resources.Load("Line") as GameObject;
        startPos = new Vector2(x, y);

    }
    void SetLinePosition()
    {
        float numberingInterval = 0.3f;

        for(int i = 0; i < GameVariable.omokBoardNum; i++)
        {
            CreateLine();
            CreateNumbering(i,new Vector2(startPos.x  - numberingInterval ,startPos.y));
            _line.GetComponent<LineRenderer>().SetPosition(0, startPos);
            _line.GetComponent<LineRenderer>().SetPosition(1, new Vector2(-startPos.x,startPos.y));
            _line.transform.SetParent(board);
            startPos.y -= interval;
        }
        startPos.y = y;
        for(int i = 0; i < GameVariable.omokBoardNum; i++)
        {
            CreateLine();
            CreateNumbering(i,new Vector2(startPos.x,startPos.y + numberingInterval));
            _line.GetComponent<LineRenderer>().SetPosition(0, startPos);
            _line.GetComponent<LineRenderer>().SetPosition(1, new Vector2(startPos.x, -startPos.y));
            _line.transform.SetParent(board);
            startPos.x += interval;
        }
    }
    void CreateLine()
    {
        _line = Instantiate(line, startPos,Quaternion.identity);
    }
    void CreateNumbering(int number, Vector2 vec)
    {
        _numbering = Instantiate(numbering,vec,Quaternion.identity);
        _numbering.transform.Find("Text").GetComponent<Text>().text = number.ToString();
        _numbering.transform.SetParent(board);
    }
}
