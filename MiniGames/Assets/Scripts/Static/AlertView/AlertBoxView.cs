using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AlertBoxOption
{
    public string cancelButtonTitle;
    public string okButtonTitle;
    public System.Action cancelButtonAction;
    public System.Action okButtonAction;
    public System.Action addAction;
}
public class AlertBoxView : MonoBehaviour
{
    public static GameObject alertBox;
    [SerializeField]
    Text titleText;
    [SerializeField]
    Text bodyText; // commom 
    [SerializeField]
    Text okButtonTitle;
    [SerializeField]
    Text cancelButtonTitle;
    [SerializeField]
    Button cancelButton;
    [SerializeField]
    Button okButton;
    System.Action okButtonAction;
    System.Action cancelButtonAction;
    System.Action addAction; // commom

    public static void ShowBox(string title, string body, AlertBoxOption option = null)
    {
        if(alertBox == null)
        {
            alertBox = Resources.Load("AlertViewUI/AlertBox") as GameObject;
        }

        AlertBoxView alertView = Instantiate(alertBox).GetComponent<AlertBoxView>();
        alertView.UpdateAlertBox(title,body,option);
    }    
    public void UpdateAlertBox(string title, string body, AlertBoxOption option = null)
    {
        titleText.text = title;
        bodyText.text = body;

        if(option != null)
        {
            // cancel button event
            // bool istrue = false || true // 하나라도 true이면 true임 그냥 햇갈려서 적었음
            cancelButton.gameObject.SetActive(option.cancelButtonTitle != null || option.okButtonTitle != null);
            cancelButtonTitle.text = option.cancelButtonTitle ?? ""; // ?? 는 null 이면 뒤에게 들어감
            cancelButtonAction = option.cancelButtonAction;

            // ok button event
            okButton.gameObject.SetActive(option.okButtonTitle != null);
            okButtonTitle.text = option.okButtonTitle ?? "";
            okButtonAction = option.okButtonAction;

            if(option.addAction != null)
            {
                addAction = option.addAction;
            }
        }
        else // default value
        {
            cancelButton.gameObject.SetActive(false);
            okButton.gameObject.SetActive(true);
            okButtonTitle.text = "OK";
        }
    }

    public void AlertDismission()
    {
        SoundManager.instance.PlayClip(EEffactClipType.DefaultButton);
        
        if(addAction != null)
        {
            addAction.Invoke();
        }
        Destroy(gameObject);
    }

    public void OnClickOkButton()
    {
        if(okButtonAction != null)
        {
            okButtonAction.Invoke();
        }
        AlertDismission();
    }
    public void OnClickCancelButton()
    {
        if(cancelButtonAction != null)
        {
            cancelButtonAction.Invoke();
        }
        AlertDismission();
    }
}
