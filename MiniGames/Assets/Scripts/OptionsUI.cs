using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OptionsUI : MonoBehaviour
{
    [SerializeField]
    Slider bgmSlider;
    [SerializeField]
    Slider effectSoundSlider;
    Animator anim;
    private void Awake() {
        anim = GetComponent<Animator>();
    }
    private void Start() {
        effectSoundSlider.value = PlayerPrefs.GetFloat("EffectSound",1);
        bgmSlider.value = PlayerPrefs.GetFloat("BGMSound",1);
    }
    public void SetChangeBGMVolume(float volume)
    {
        SoundManager.instance.bgmSource.volume = volume;
    }
    public void SetChangeEffectVolume(float volume)
    {
        foreach(var v in SoundManager.instance.effectSources)
        {
            v.volume = volume;
        }
    }
    public void OnClickSaveButton() // 이 때만 PlayerPrefs로 저장함
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);

        AlertBoxView.ShowBox("Volume Save", "저장 하시겠습니까?",new AlertBoxOption{
            okButtonTitle = "Ok",
            cancelButtonTitle = "cancel",
            okButtonAction = () => {
                PlayerPrefs.SetFloat("BGMSound",SoundManager.instance.bgmSource.volume);
                PlayerPrefs.SetFloat("EffectSound",SoundManager.instance.effectSources[0].volume);
                StartCoroutine(OptionCloseDelay(anim,false));
            }
            ,
            cancelButtonAction = null 
        });
    }
    public void OnClickCancelButton()
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);

        effectSoundSlider.value = PlayerPrefs.GetFloat("EffectSound",1);
        bgmSlider.value = PlayerPrefs.GetFloat("BGMSound",1);

        StartCoroutine(OptionCloseDelay(anim,false));
    }
    IEnumerator OptionCloseDelay(Animator anim, bool value)
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);

        anim.SetTrigger("isClose");
        
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length); // close 시간 보장이구나
        anim.gameObject.SetActive(value);
        anim.ResetTrigger("isClose");
    }
}
