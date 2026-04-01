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
        instance = this;
    }
    public void ClickMenu(IMenu menu)
    {
        menueStack.Push(menu);
        menu.ClickMenu();
    }
    public void Back()
    {
        if (menueStack.Count <= 0)
        {
            //°ÔÀÓÁ¾·áÆË¾÷ ¶ç¿ö¾ß ÇÔ
            return;
        }

        menueStack.Pop().CloseMenue();
    }
}
