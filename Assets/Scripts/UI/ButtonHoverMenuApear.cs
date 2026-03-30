using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverMenuApear : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject menu;
    [SerializeField] float moveSpeed;
    [SerializeField] float movedistance;
    Vector3 originMenuPos;
    void Start()
    {
        menu.SetActive(false);
        originMenuPos = menu.transform.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        menu.transform.position = originMenuPos;
        menu.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        menu.SetActive(false);
    }
    IEnumerator ScrollMove()
    {
        yield return null;
    }
}
