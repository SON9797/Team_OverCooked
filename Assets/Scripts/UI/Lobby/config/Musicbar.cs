using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Musicbar : MonoBehaviour
{
    [SerializeField] List<Image> bars;
    [SerializeField] Sprite activebar;
    [SerializeField] Sprite inactivebar;
    int volume = 5;
    void Start()
    {
        UpdateVolume();
    }

    public void UpdateVolume() 
    {
        for (int i = 0; i < bars.Count; i++)
        {
            bars[i].sprite = inactivebar;
        }
        for (int i = 0; i < volume; i++)
        {
            bars[i].sprite = activebar;
        }
    }
    public void MinusVolume()
    {
        if (volume > 0)
        {
            volume--;
            UpdateVolume();
        }
    }
    public void PlusVolume()
    {
        if (volume < bars.Count)
        {
            volume++;
            UpdateVolume();
        }
    }
    
}
