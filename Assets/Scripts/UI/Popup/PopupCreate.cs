using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupCreate : MonoBehaviour
{

    [SerializeField] PopupWindow popupWindow;
    [SerializeField] Vector3 popupPos = Vector3.zero;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI content;

    private PopupWindow currentPopup;
    void OpenPopup(string titleText,string contentText)
    {
        PopupWindow popup= Instantiate(popupWindow);
        popup.transform.position = popupPos;
        popup.Setting(titleText, contentText);
    }
    public void OpenPopup(string titleText,GameObject content)
    {
        currentPopup = Instantiate(popupWindow);
        currentPopup.transform.position = popupPos;
        GameObject contentObj= Instantiate(content);
        contentObj.transform.position = popupPos;
        contentObj.transform.SetParent(currentPopup.transform,false);
        
    }
    public void ClosePopup()
    {
        currentPopup.CloseWindow();
    }

}
