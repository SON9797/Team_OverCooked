using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupButtonData
{
    public string text;
    public Action onclickAction;
}

public class PopupWindow : MonoBehaviour,IMenu
{
    [SerializeField] public TextMeshProUGUI title;
    [SerializeField] public TextMeshProUGUI content;
    [SerializeField] public Transform buttonRoot;
    [SerializeField] public Transform contentRoot;
    [SerializeField] PopupButton buttonPrefab;
    public string scenename { get; set; }
    public void Setting(string titleText,string contentText)
    {
        title.text=titleText; 
        content.text=contentText;
        scenename=SceneManager.GetActiveScene().name;
        MenuManager.instance.AddMenuStack(this);
        
    }
    public void SetupButtons(List<PopupButtonData> buttons)
    {
        if (buttons == null)
            return;
        foreach (var data in buttons)
        {
            var btn = Instantiate(buttonPrefab, buttonRoot);
            btn.Setting(data.text, data.onclickAction);
        }
    }
    public void CloseWindow()
    {
        MenuManager.instance.MinusMenuStack();
        Destroy(gameObject);
    }
}
