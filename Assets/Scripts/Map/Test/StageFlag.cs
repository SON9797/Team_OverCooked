using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageFlag : MonoBehaviour
{
    [SerializeField] private string _stageKey = "Stage_1_1";


    [SerializeField] private StageTransUI _transUI;

    private void OnTriggerEnter(Collider other)
    {

        BusMove bus = other.GetComponentInParent<BusMove>();
        if (bus != null)
        {
            if (_transUI != null)
            {
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
