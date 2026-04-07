using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    Stack<IMenu>menueStack=new Stack<IMenu>();

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
        if (menueStack.Count <= 0)
        {
            PopupManager.instance.GameExitPopup();
            return;
        }
        print(menueStack.Count);
        menueStack.Peek().CloseWindow();
    }
}
