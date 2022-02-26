using UnityEngine;
using UnityEngine.UI;

public class OmokStone : Stone
{
    public Text _text;
    public virtual void Awake() {
        SoundManager.instance.PlayClip(EEffactClipType.Put);
        _renderer = GetComponent<SpriteRenderer>();
    }
}
