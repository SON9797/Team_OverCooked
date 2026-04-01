using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupButton : MonoBehaviour
{
    [SerializeField]private GameObject button;
    [SerializeField]private TextMeshProUGUI btnText;

    public void Setting(string text)
    {
        btnText.text = text;
    }
}
