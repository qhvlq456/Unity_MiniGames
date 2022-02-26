using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
[Serializable]
public enum EPlayerType{
    None,
    White,
    Black
}
public abstract class BasePlayer : MonoBehaviourPun
{
    #region Variable
    public EPlayerType playerType;

    [SerializeField]
    protected GameObject stone;
    protected Transform parent;
    protected Vector3 putPosition;
    protected int boardNum;
    public int r,c;
    public int m_turn;
    #endregion
    public virtual void IsMouseButtonDown()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GetStonePosition(mousePosition);
    }
    public virtual GameObject CreateStone()
    {
        return Instantiate(stone,putPosition,Quaternion.identity);
    }
    public abstract void GetStonePosition(Vector3 mousePosition);
    public abstract void SetStonePosition();
    
}
