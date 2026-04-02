using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupWindow : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI title;
    [SerializeField] public TextMeshProUGUI content;
    [SerializeField] public Transform buttonRoot;
    [SerializeField] GameObject buttonPrefab;

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
