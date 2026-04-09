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
    [SerializeField] Sprite emptyStar;
    [SerializeField] Sprite fillStar;
    [SerializeField] TextMeshProUGUI bestScoreText;
    [SerializeField] List<ImageList> imageList;

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

    public void ShowLevel(int mainChapter,int subChapter)
    {
        //해당 스테이지 사진 받아오기
        stagePicture.sprite = imageDict[$"{mainChapter}-{subChapter}"];

        //해당 스테이지 별,최고점수 받아오기
        SaveData savedata = SaveLoad.instance.currentData;
        int starcount=0;
        int bestscore=0;
        if (savedata.bestScores.ContainsKey($"{mainChapter}-{subChapter}")) {
            starcount = savedata.bestScores[$"{mainChapter}-{subChapter}"].starCount;
            bestscore = savedata.bestScores[$"{mainChapter}-{subChapter}"].score;
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

        gameObject.SetActive(true);
    }
    public void HideWindow()
    {
        gameObject.SetActive(false);
    }
}
