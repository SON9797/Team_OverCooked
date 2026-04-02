using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;
    [SerializeField] PopupWindow popupWindow;
    [SerializeField] Transform canvasTransform;
    

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

        //텍스트가 아닌 것들이 있으면 여기서 생성
        if (contentObj != null)
        {
            GameObject contentOn = Instantiate(contentObj,currentPopup.content.transform);
            RectTransform rect2 = contentOn.GetComponent<RectTransform>();
            rect2.anchoredPosition = Vector2.zero;
        }

        //버튼 생성
        currentPopup.SetupButtons(buttons);
    }
    public void OpenInformationPopup(string contentText)
    {
        string titletext = "Info";
        PopupButtonData buttonData = new PopupButtonData();
        buttonData.text = "OK";
        buttonData.onclickAction = ClosePopup;
        List<PopupButtonData>buttonList=new List<PopupButtonData>();
        buttonList.Add(buttonData);

        OpenPopup(titletext, buttonList, contentText);
    }
    public void OpenSaveLoadPopup()
    {
        string titletext = "Load";
        PopupButtonData buttonData = new PopupButtonData();
        buttonData.text = "OK";
        buttonData.onclickAction = ClosePopup;
        List<PopupButtonData> buttonList = new List<PopupButtonData>();
        buttonList.Add(buttonData);

      //  OpenPopup(titletext,buttonList,,)

    }
    public void ClosePopup()
    {
        currentPopup.CloseWindow();
    }

}
