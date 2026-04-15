using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

public class UIHoverSound : MonoBehaviour, IPointerEnterHandler
{
    private ICommonSoundManager _soundManager;

    [Inject]
    public void Construct(ICommonSoundManager soundManager)
    {
        _soundManager = soundManager;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Selectable selectable = GetComponent<Selectable>();

        if (selectable != null && !selectable.interactable)
        {
            return;
        }

        if (_soundManager != null)
        {
            _soundManager.PlaySFX(OverCooked.SFXType.UI_Click);
        }
    }
}
