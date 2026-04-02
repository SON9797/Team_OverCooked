using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupButtonData
{
    public string text;
    public Action onclickAction;
}
public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;
    [SerializeField] PopupWindow popupWindow;
    [SerializeField] Transform canvasTransform;
    [SerializeField] GameObject buttonPrefab;

    private PopupWindow currentPopup;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }   
        instance = this;
    }
    void OpenPopup(string titleText,List<PopupButtonData>buttons, string contentText = "", GameObject contentObj = null)
    {
        currentPopup = Instantiate(popupWindow, canvasTransform);
        RectTransform rect = currentPopup.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        currentPopup.Setting(titleText, contentText);

        if (contentObj != null)
        {
            GameObject contentOn = Instantiate(contentObj,currentPopup.content.transform);
            RectTransform rect2 = contentOn.GetComponent<RectTransform>();
            rect2.anchoredPosition = Vector2.zero;
        }
    }
    public void OpenInformationPopup(string titletext,string contentText)
    {
        
    }
    public void ClosePopup()
    {
        currentPopup.CloseWindow();
    }

}
