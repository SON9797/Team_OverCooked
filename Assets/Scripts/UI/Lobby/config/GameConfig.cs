using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour,IMenu
{
    public string prevScenename { get; set; }
    public string currentScenename { get; set; }

    public void OpenWindow()
    {
        MenuManager.instance.AddMenuStack(this);
        gameObject.SetActive(true);
    }
    public void CloseWindow()
    {

        gameObject.SetActive(true);
        MenuManager.instance.MinusMenuStack();
    }


}
