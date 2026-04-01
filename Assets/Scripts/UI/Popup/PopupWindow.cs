using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI content;

    public void Setting(string titleText,string contentText)
    {
        title.text=titleText; 
        content.text=contentText;
    }
    public void CloseWindow()
    {
        Destroy(gameObject);
    }
}
