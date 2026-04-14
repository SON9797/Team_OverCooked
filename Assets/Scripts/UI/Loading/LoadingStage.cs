using Overcooked;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LoadingStage : MonoBehaviour
{
    [SerializeField] Image stagePicture;
    [SerializeField] TextMeshProUGUI bestscoreText;
    [SerializeField] List<Image> stars;
    [SerializeField] List<TextMeshProUGUI> starsScore;
    [SerializeField] Sprite emptyStar;
    [SerializeField] Sprite fillStar;

    private void Start()
    {
        Setting(MenuManager.instance.enterStage);
    }
    void Setting(string stagename)
    {
        SaveData saveData = SaveLoad.instance.currentData;
        int starcount = 0;
        int bestscore = 0;
        if (saveData.bestScores.ContainsKey(stagename))
        {
            starcount=saveData.bestScores[stagename].starCount;
            bestscore = saveData.bestScores[stagename].score;
        }
        LevelData levelData = SoStageDatas.instance.levelDatas[stagename];
        stagePicture.sprite = levelData.LoadingImage;
        starsScore[0].text = levelData.OneStar.ToString();
        starsScore[1].text = levelData.TwoStar.ToString();
        starsScore[2].text = levelData.ThreeStar.ToString();


        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].sprite = emptyStar;
        }
        for(int i = 0; i < starcount; i++)
        {
            stars[i].sprite = fillStar;
        }
        bestscoreText.text = bestscore.ToString();
    }
}
