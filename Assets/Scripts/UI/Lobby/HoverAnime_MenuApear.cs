using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using OverCooked;
using UnityEngine.UI;

public class HoverAnime_MenuApear : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] int index;
    [SerializeField] bool active = true;
    [SerializeField] GameObject menu;
    [SerializeField] Sprite inactiveImage;
    float duration=0.1f;
    float movedistance=-0.2f;
    Vector3 originMenuPos;
    int hoverCount=0;

    Image thisImage;

    MenuApearManager menuManager;

    private IInGameSoundManager _soundManager;

    [Inject]
    public void Construct(IInGameSoundManager soundManager)
    {
        Debug.Log($"{gameObject.name} 사운드 매니저 주입");
        _soundManager = soundManager;
    }
    private void Awake()
    {
        thisImage=GetComponent<Image>();
    }
    void Start()
    {
        if (active == false)
        {
            thisImage.sprite= inactiveImage;
        }
        menu.SetActive(false);
        originMenuPos = menu.transform.position;
        menuManager = MenuApearManager.instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (active==false)
        {
            return;
        }
        menuManager.ChangeMenuList(index);
    }
    public void MenuApear()
    {
        if (active == false)
        {
            return;
        }
        if (menu.activeSelf)
            return;

        if (_soundManager != null)
        {
            _soundManager.PlaySFX(SFXType.UI_ButtonSound);
        }

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
