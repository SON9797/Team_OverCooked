using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum SaveSlotMode
{
    newgame,
    load
};
public class SaveSlotButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI currentStage;
    [SerializeField] TextMeshProUGUI totalStar;
    [SerializeField] TextMeshProUGUI defaultText;
    [SerializeField] GameObject starImg;

    int index;
    bool loadck = false;
    SaveSlotMode mode;

    public void Setting(int slotnum,SaveSlotMode mode,string currentStageText="",string totalStarText="",bool loadck=true)
    {
        index = slotnum;
        title.text = $"Slot {slotnum}";
        this.loadck = loadck;
        this.mode = mode;
        if (loadck == false)
        {
            defaultText.text = "Slot is empty";
            totalStar.gameObject.SetActive(false);
            currentStage.gameObject.SetActive(false);
            starImg.SetActive(false);
            return;
        }
        defaultText.text = "";
        
        currentStage.text = currentStageText;
        totalStar.text = totalStarText;
    }

    public void Onclick()
    {
        SaveLoad.instance.CurrentDataSet(index);
        SceneLoader.Instance.LoadSceneAsync("WorldMapScene");
    }
}
