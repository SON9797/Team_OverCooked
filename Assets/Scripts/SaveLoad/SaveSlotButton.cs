using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveSlotButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI currentStage;
    [SerializeField] TextMeshProUGUI totalStar;
    [SerializeField] TextMeshProUGUI defaultText;

    enum Mode{
        newgame,
        load
    };

    public void Onclick()
    {

    }
}
