using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoneUI : MonoBehaviour
{
    [SerializeField]
    Button[] stoneButton;
    public int selectStone {get; private set; }
    private void Awake() {
        selectStone = (int)EPlayerType.White;
    }
    private void OnEnable() {
        stoneButton[selectStone].image.color = Color.green;
    }
    public void OnClickStoneSelectButton(int stone)
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
        
        for(int i = 1; i < stoneButton.Length; i++)
        {
            if(i == stone)
            {
                stoneButton[i].image.color = Color.green;
            }
            else
            {
                stoneButton[i].image.color = Color.white;
            }
        }
        selectStone = stone;
    }
}
