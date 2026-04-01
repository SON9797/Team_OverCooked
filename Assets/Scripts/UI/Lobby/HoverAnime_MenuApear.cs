using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverAnime_MenuApear : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] int index;
    [SerializeField] GameObject menu;
    float duration=0.1f;
    float movedistance=-0.2f;
    Vector3 originMenuPos;
    int hoverCount=0;

    MenuApearManager menuManager;

    void Start()
    {
        menu.SetActive(false);
        originMenuPos = menu.transform.position;
        menuManager = MenuApearManager.instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        menuManager.ChangeMenuList(index);
    }
    public void MenuApear()
    {
        if (menu.activeSelf)
            return;
        menu.transform.position = originMenuPos;
        menu.SetActive(true);
        StartCoroutine(ScrollMove());
    }
    public void MenuDisapear()
    {
        menu.SetActive(false);
    }
    
    IEnumerator ScrollMove()
    {
        Vector3 targetPos = originMenuPos + new Vector3(0, movedistance, 0);
        float t = 0;
        while (t<duration)
        {
            t+= Time.deltaTime;
            menu.transform.position= Vector3.Lerp(originMenuPos, targetPos, t / duration);
            yield return null;
        }

        yield return null;
    }
}
