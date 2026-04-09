using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageFlag : MonoBehaviour
{
    [SerializeField] private string _stageKey = "Stage_1_1";


    [SerializeField] private StageTransUI _transUI;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.name}이(가) 들어옴");

        BusMove bus = other.GetComponentInParent<BusMove>();
        if (bus != null)
        {
            if (_transUI != null)
            {
                Debug.Log("BusMove 컴포넌트 찾음!");
                _transUI.ShowUI(_stageKey);
            }
        }
        else
        {
            Debug.Log("BusMove를 찾지 못했습니다.");
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
