using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuApearManager : MonoBehaviour
{
    public static MenuApearManager instance;

    [SerializeField] List<HoverAnime_MenuApear> btns;
    public int select { get; private set; } = 0;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

    }
    public void ChangeMenuList(int index)
    {
        select = index;
        for (int i = 0; i < btns.Count; i++)
        {
            if (i == index)
                continue;
            btns[i].MenuDisapear();
        }
        btns[index].MenuApear();
    }
}
