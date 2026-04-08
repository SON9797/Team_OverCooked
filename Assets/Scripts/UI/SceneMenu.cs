using System.Collections;
using System.Collections.Generic;
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
    }
    public void CloseWindow()
    {
        SceneLoader.Instance.LoadSceneAsync(menuManager.GetPrevSceneName());
        menuManager.MinusMenuStack();
    }
}
