using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Musicbar : MonoBehaviour
{
    public enum SoundType
    {
        bgm,
        effect
    }
    [SerializeField] List<Image> bars;
    [SerializeField] Sprite activebar;
    [SerializeField] Sprite inactivebar;
    [SerializeField] SoundType soundType = SoundType.bgm;
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
        switch (soundType)
        {
            case SoundType.bgm:
                VolumeManager.Instance.bgmVolume = volume / 5.0f;
                break;
            case SoundType.effect:
                VolumeManager.Instance.effectVolume = volume / 5.0f;
                break;
        }
        VolumeManager.Instance.UpdateVolumToAudioSource();
        
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
