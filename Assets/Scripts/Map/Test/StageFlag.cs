using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class StageFlag : MonoBehaviour
{
    [SerializeField] private string _stageKey = "Stage_1_1";


    [SerializeField] private StageTransUI _transUI;

    //private ICommonSoundManager _soundManager;

    private IWorldMapSoundManager _soundManager;

    [Inject]
    public void Construct(IWorldMapSoundManager soundManager)
    {
        _soundManager = soundManager;
    }

    private void OnTriggerEnter(Collider other)
    {

        BusMove bus = other.GetComponentInParent<BusMove>();
        if (bus != null)
        {
            if (_transUI != null)
            {
                if (_soundManager != null)
                {
                    Debug.Log("사운드 재생");
                    _soundManager.PlaySFX(OverCooked.SFXType.Van_Flag);
                }
                _transUI.ShowUI(_stageKey);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<BusMove>() != null)
        {
            if (_transUI != null)
            {
                _transUI.HideUI(); // UI를 숨기는 함수 호출
            }
        }
    }
}
