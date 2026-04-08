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

        //텍스트가 아닌 것들이 있으면 여기서 배치
        if (contentObj != null)
        {
            for(int i= 0;i<contentObj.Count;i++)
            {
                //GameObject contentOn = Instantiate(contentObj[i], currentPopup.contentRoot);
                //RectTransform rect2 = contentOn.GetComponent<RectTransform>();
                //rect2.anchoredPosition = Vector2.zero;
                contentObj[i].transform.SetParent(currentPopup.contentRoot, false);
                RectTransform rect2 = contentObj[i].GetComponent<RectTransform>();
                rect2.anchoredPosition = Vector2.zero;
            }
        }

        //버튼 생성
        currentPopup.SetupButtons(buttons);
    }
    public void GameExitPopup()
    {
        List<PopupButtonData> buttonlist=new List<PopupButtonData>();
        PopupButtonData btnCancel = new PopupButtonData();
        btnCancel.text = "Cancel";
        btnCancel.onclickAction = ClosePopup;
        PopupButtonData btnok=new PopupButtonData();
        btnok.text = "OK";
        btnok.onclickAction = Application.Quit;
        buttonlist.Add(btnCancel);
        buttonlist.Add(btnok);
        OpenPopup("Game Exit", buttonlist, "Do you want exit Game?");
    }
    public void GotoPrevScenePopup()
    {
        List<PopupButtonData> buttonlist = new List<PopupButtonData>();
        PopupButtonData btnCancel = new PopupButtonData();
        btnCancel.text = "Cancel";
        btnCancel.onclickAction = ClosePopup;
        PopupButtonData btnok = new PopupButtonData();
        btnok.text = "OK";
        btnok.onclickAction = ClosePopup;
        btnok.onclickAction += MenuManager.instance.Back;
        buttonlist.Add(btnCancel);
        buttonlist.Add(btnok);
        OpenPopup("Previous", buttonlist, "Do you want move prev screen?");
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
