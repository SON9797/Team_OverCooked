
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouceHoverChangeImg : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Sprite normalImg;
    [SerializeField] Sprite hoverImg;
    

    Image img;
    private void Start()
    {
        img = GetComponent<Image>();
        img.sprite = normalImg;
    }
    
        
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (normalImg == null)
        {
            Color c = img.color;
            c.a = 1f;
            img.color = c;
        }
            
        img.sprite = hoverImg;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (normalImg == null)
        {
            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }
        img.sprite = normalImg;
    }
}
