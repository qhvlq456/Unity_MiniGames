using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Stone : MonoBehaviourPun
{
    // Start is called before the first frame update
    // turn == 1 : white , 2 == black
    public Sprite[] _sprite;
    public SpriteRenderer _renderer;
    public EPlayerType stoneType;
    public int m_row;
    public int m_col;
    public virtual void SetStoneType(EPlayerType type)
    {
        stoneType = type;
    }
    public virtual void SetImageType()
    {
        _renderer.sprite = _sprite[(int)stoneType];
    }
}
