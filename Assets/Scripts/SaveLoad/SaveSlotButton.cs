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
    [SerializeField] GameObject contentprefab;

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
        if (mode == SaveSlotMode.load)
        {

            if (!SaveLoad.instance.CurrentDataSet(index))
            {
                print("no");
                return;
            }
            
        }
        SaveLoad.instance.autoSaveIndex = index;
        if (mode == SaveSlotMode.newgame)
        {
            //수정 뉴게임 리셋용도
            SaveLoad.instance.currentData = new SaveData();
            SaveLoad.instance.currentData.bestScores = new Dictionary<string, ChapterScore>();
            SaveLoad.instance.AutoSave(); 

            /*
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            */

            //수정

            /*
            //SaveLoad.instance.CurrentDataUpdate(0, 0, 0, 0);
            SaveLoad.instance.AutoSave();
            */
        }
        PlayerPrefs.SetInt("Continue", index);
        PlayerPrefs.Save();
        MenuManager.instance.MinusMenuStack();
        MenuManager.instance.worldMap.OpenWindow(contentprefab);
        //SceneLoader.Instance.LoadSceneAsync("WorldMapScene");


    }
}
