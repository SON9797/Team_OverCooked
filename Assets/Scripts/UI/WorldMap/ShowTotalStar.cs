using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowTotalStar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI starcountText;

    private void Start()
    {
        UpdateStarCount();
    }
    void UpdateStarCount()
    {
        starcountText.text=SaveLoad.instance.currentData.totalStarCount.ToString();
    }
}
