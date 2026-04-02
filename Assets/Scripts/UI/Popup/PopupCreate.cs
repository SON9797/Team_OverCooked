using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupCreate : MonoBehaviour
{

    [SerializeField] PopupWindow popupWindow;
    [SerializeField] Transform canvasTransform;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI content;
    [SerializeField] Transform buttonRoot;

    private PopupWindow currentPopup;
    private void Awake()
    {
        
    }
    void OpenPopup(string titleText,string contentText)
    {
        currentPopup = Instantiate(popupWindow, canvasTransform);
        RectTransform rect = currentPopup.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        currentPopup.Setting(titleText, contentText);
    }
    public void OpenPopup(string titleText,GameObject contentObj)
    {
        currentPopup = Instantiate(popupWindow, canvasTransform);
        currentPopup.Setting(titleText, "");
        RectTransform rect = currentPopup.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        

        GameObject contentOn = Instantiate(contentObj, content.transform);
        RectTransform rect2 = contentOn.GetComponent<RectTransform>();
        rect2.anchoredPosition = Vector2.zero;

    }
    public void ClosePopup()
    {
        currentPopup.CloseWindow();
    }

}
