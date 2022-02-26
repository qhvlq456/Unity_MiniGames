using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AlertPanelView : MonoBehaviour
{
    public static GameObject alertPanel;
    public GameObject inst;
    [SerializeField]
    Image image;
    [SerializeField]
    Text bodyText;
    public static void ShowPanel(string body,float deleteTime)
    {
        if(alertPanel == null)
        {
            alertPanel = Resources.Load("AlertViewUI/AlertPanel") as GameObject;
        }

        AlertPanelView alertView = Instantiate(alertPanel).GetComponent<AlertPanelView>();
        alertView.UpdateAlertPanel(body, deleteTime);
    }
    public void UpdateAlertPanel(string body, float deleteTime = 1f) // 독립적으로 만들어야겠다..
    {
        bodyText.text = body;
        
        Destroy(gameObject,deleteTime);
    }
}
