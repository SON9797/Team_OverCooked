using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupButton : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI btnText;
    Button button;
    private void Awake()
    {
        button=GetComponent<Button>();
    }
    public void Setting(string text,Action action)
    {
        btnText.text = text;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => action?.Invoke());
    }
}
