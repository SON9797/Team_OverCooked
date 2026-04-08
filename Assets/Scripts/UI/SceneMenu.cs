using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SceneMenu : IMenu
{
    public string prevScenename { get; set; }
    public string currentScenename { get; set; }

    MenuManager menuManager;
    public string sceneName;


    public void OpenWindow()
    {
        menuManager = MenuManager.instance;
        prevScenename = menuManager.GetPrevSceneName();
        currentScenename = sceneName;
        SceneLoader.Instance.LoadSceneAsync(currentScenename);
        menuManager.AddMenuStack(this);
        Debug.Log($"current:{currentScenename},prev:{prevScenename}");
    }
    public void CloseWindow()
    {
        PopupManager.instance.GotoScenePopup(prevScenename);
        /*
        SceneLoader.Instance.LoadSceneAsync(prevScenename);
        menuManager.MinusMenuStack();
        */
    }
}
