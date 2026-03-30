
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouceHoverChangeImg : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Sprite normalImg;
    [SerializeField] Sprite hoverImg;
    [SerializeField] GameObject menu;

    Image img;
    private void Start()
    {
        img = GetComponent<Image>();
        img.sprite = normalImg;
        menu.SetActive(false);
    }
    
        
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        img.sprite = hoverImg;
        menu.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        img.sprite = normalImg;
        menu.SetActive(false);
    }
}
