using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChefChange : MonoBehaviour,IMenu
{
    [SerializeField] Transform originCameraPos;
    [SerializeField]Transform camerapos;
    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] GameObject shefSelectCanvas;
    public string prevScenename { get; set; }
    public string currentScenename { get; set; }
    MenuManager menuManager;
    public void EnterMenu()
    {
        menuManager = MenuManager.instance;
        prevScenename = menuManager.GetPrevSceneName();
        currentScenename = SceneManager.GetActiveScene().name;
        menuManager.AddMenuStack(this);
        mainMenuCanvas.SetActive(false);
        shefSelectCanvas.SetActive(true);
        CameraMove.instance.MoveToTargetAsync(camerapos, 3f);
    }

    public void CloseWindow()
    {
        mainMenuCanvas.SetActive(true);
        shefSelectCanvas.SetActive(false);
        CameraMove.instance.MoveToTargetAsync(originCameraPos, 3f);
    }
}
