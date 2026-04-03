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
    [SerializeField] GameObject saveSlot;
    

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
    void OpenPopup(string titleText,List<PopupButtonData>buttons=null, string contentText = "", List<GameObject> contentObj = null)
    {
        currentPopup = Instantiate(popupWindow, canvasTransform);
        RectTransform rect = currentPopup.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        currentPopup.Setting(titleText, contentText);

        //텍스트가 아닌 것들이 있으면 여기서 생성
        if (contentObj != null)
        {
            foreach (var co in contentObj)
            {
                GameObject contentOn = Instantiate(co, currentPopup.contentRoot);
                RectTransform rect2 = contentOn.GetComponent<RectTransform>();
                rect2.anchoredPosition = Vector2.zero;
            }
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

    //mode가 0이면 새로 시작, 1이면 로드하는거
    public void OpenSaveLoadPopup(int mode)
    {
        string titletext = "Load";
        
        List<GameObject> slotList=new List<GameObject>();
        SaveData[] saveDatas = SaveLoad.instance.savedatas;
        for(int i = 0; i < 3; i++)
        {
            GameObject slot = Instantiate( saveSlot);
            SaveSlotButton b=slot.GetComponent<SaveSlotButton>();
            b.Setting(i, mode==0 ? SaveSlotMode.newgame:SaveSlotMode.load, $"{saveDatas[i].currentChapter}-{saveDatas[i].currentSubChapter}", saveDatas[i].totalStarCount.ToString(), saveDatas[i].currentChapter == 0 ? false : true);
            slotList.Add(slot);
        }

        OpenPopup(titletext, contentObj: slotList);

    }
    public void ClosePopup()
    {
        currentPopup.CloseWindow();
    }

}
