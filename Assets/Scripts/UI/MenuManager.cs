using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    Stack<IMenu>menueStack=new Stack<IMenu>();
    public SceneMenu lobbyMap { get; private set; } = new SceneMenu();
    public SceneMenu worldMap { get; private set; }=new SceneMenu();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
    private void Start()
    {
        worldMap.sceneName = "WorldMapScene";
        lobbyMap.sceneName = "SclectStageMap_UITEst";
        menueStack.Push(lobbyMap);

    }
    public void AddMenuStack(IMenu menu)
    {
        menueStack.Push(menu);
    }
    public void MinusMenuStack()
    {
        menueStack.Pop();
    }
    public void Back()
    {
        if (menueStack.Count <= 1)
        {
            PopupManager.instance.GameExitPopup();
            return;
        }
        print(menueStack.Count);
        menueStack.Peek().CloseWindow();
    }
    public string GetPrevSceneName()
    {
        return menueStack.Peek().currentScenename;
    }
}
