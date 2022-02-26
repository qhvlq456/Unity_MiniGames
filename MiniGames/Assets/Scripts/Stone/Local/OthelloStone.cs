using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OthelloStone : OmokStone
{    
    public override void Awake() {
        base.Awake();
        var manager = GameObject.Find("GameManager").GetComponent<OthelloManager>();
        manager.saveStones.Add(this);
    }
    public virtual void Update()
    {
        SetImageType();
    }
}
