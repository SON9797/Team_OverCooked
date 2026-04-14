using Overcooked;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class ImageList
{
    public string key;
    public Sprite image;
}
public class WorldMapLevelHover : MonoBehaviour
{
    [SerializeField] Image stagePicture;
    [SerializeField] List<Image> stars;
    [SerializeField] List<TextMeshProUGUI> starScore;
    [SerializeField] Sprite emptyStar;
    [SerializeField] Sprite fillStar;
    [SerializeField] TextMeshProUGUI bestScoreText;

    Dictionary<string, Sprite> imageDict=new Dictionary<string, Sprite>();
    void Start()
    {
        for(int i = 0; i < imageList.Count; i++)
        {
            imageDict[imageList[i].key]=imageList[i].image;
        }

        //테스트
        //ShowLevel(1, 1);
    }

    public void ShowLevel(string chapter)
    {

        //해당 스테이지 사진 받아오기
        LevelData leveldata = SoStageDatas.instance.levelDatas[chapter];
        stagePicture.sprite = leveldata.LoadingImage;
        starScore[0].text = leveldata.OneStar.ToString();
        starScore[1].text = leveldata.TwoStar.ToString();
        starScore[2].text = leveldata.ThreeStar.ToString();

        //해당 스테이지 별,최고점수 받아오기
        SaveData savedata = SaveLoad.instance.currentData;
        int starcount=0;
        int bestscore=0;
        if (savedata.bestScores.ContainsKey(chapter)) {
            starcount = savedata.bestScores[chapter].starCount;
            bestscore = savedata.bestScores[chapter].score;
        }
        for(int i = 0; i < stars.Count; i++)
        {
            stars[i].sprite = emptyStar;
        }
        for(int i = 0; i < starcount; i++)
        {
            stars[i].sprite = fillStar;
        }
        bestScoreText.text=bestscore.ToString();

        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
        //gameObject.SetActive(true);
    }
    public void HideWindow()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.alpha = 0f;           // 안 보이게
        cg.interactable = false; // 클릭 막기
        cg.blocksRaycasts = false;
        //gameObject.SetActive(false);
    }
}
