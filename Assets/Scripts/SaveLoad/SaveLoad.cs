using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using Formatting = Newtonsoft.Json.Formatting;

public class Chapter
{
    public int mainChapter;
    public int subChapter;
    public string ToKey()
    {
        return $"{mainChapter}-{subChapter}";
    }
    public Chapter FromKey(string key)
    {
        var split = key.Split('-');

        return new Chapter
        {
            mainChapter = int.Parse(split[0]),
            subChapter = int.Parse(split[1])
        };
    }
}
public class ChapterScore
{
    public int score;
    public int starCount;
}
public class SaveData
{
    public int currentChapter;
    public int currentSubChapter;
    public int totalStarCount;
    public Dictionary<string,ChapterScore> bestScores = new Dictionary<string,ChapterScore>();

    public List<string> unlockedStages = new List<string>(); //추가

    public bool hasPlayedIntro = false;// 추가

    public ChapterScore GetScore(Chapter chapter)
    {
        string key = chapter.ToKey();

        if (bestScores.TryGetValue(key, out var score))
            return score;

        return null;
    }

    //추가
    public bool IsUnlocked(string stageKey)
    {
        return bestScores.ContainsKey(stageKey) || unlockedStages.Contains(stageKey);
    }
    public void UnlockStage(string stageKey)
    {
        if (!unlockedStages.Contains(stageKey))
        {
            unlockedStages.Add(stageKey);
        }
    }
    //추가

}


public class SaveLoad : MonoBehaviour
{
    public static SaveLoad instance;

    

    public SaveData currentData=new SaveData();
    public int autoSaveIndex=0;

    public SaveData[] savedatas = new SaveData[3];



    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        /*
        SaveData testData = new SaveData();
        testData.currentChapter = 1;
        testData.currentSubChapter = 1;
        ChapterScore testScore = new ChapterScore();
        testScore = ToChapterScore(200, 2);
        testData.totalStarCount = 2;
        testData.bestScores["1-1"]=testScore;
        Save(testData, 0);
        */
        LoadAllSlot();
        Debug.Log($"{savedatas[0].currentChapter}");
        //수정
        if (savedatas[0] != null)
        {
            currentData = savedatas[0];
            // 딕셔너리가 null로 로드되는 것을 방지
            if (currentData.bestScores == null)
                currentData.bestScores = new Dictionary<string, ChapterScore>();

            Debug.Log($"[SaveLoad] 데이터 로드 완료. 스테이지 개수: {currentData.bestScores.Count}");
        }
        //수정
    }

    public void LoadAllSlot()
    {
        for (int i = 0; i < savedatas.Length; i++)
        {
            savedatas[i] = Load(i);
        }
    }

    public void Save(SaveData data, int slotNum)
    {
        string path = Path.Combine(Application.persistentDataPath, $"save{slotNum}.json");
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        
     
        File.WriteAllText(path, json);

        Debug.Log("저장 완료: " + path);
    }

    public void CurrentDataUpdate(int mainChapter, int subChapter, int bestscore, int starcount)
    {
        //수정
        if (currentData == null)
        {
            currentData = new SaveData();
        }
        if (currentData.bestScores == null)
        {
            currentData.bestScores = new Dictionary<string, ChapterScore>();
        }

        //수정

        if (currentData.bestScores == null)
        {
            currentData.bestScores = new Dictionary<string, ChapterScore>();
        }

        if (currentData.currentChapter < mainChapter)
        {
            currentData.currentChapter = mainChapter;
            currentData.currentSubChapter = subChapter;
        }
        else if (currentData.currentChapter == mainChapter && currentData.currentSubChapter < subChapter)
        {
            currentData.currentSubChapter = subChapter;
        }

        //Chapter chapter = new Chapter();
        //chapter.mainChapter = mainChapter;
        //chapter.subChapter = subChapter;
        string chapterString = $"{mainChapter}-{subChapter}";//수정

        //추가
        if (starcount < 1)
        {
            Debug.Log($"[CurrentDataUpdate] 별 1개 미만 - bestScores 저장 스킵, currentChapter={currentData.currentChapter}, currentSubChapter={currentData.currentSubChapter}");
            return;
        }

        if (currentData.bestScores.ContainsKey(chapterString))
        {
            if (currentData.bestScores[chapterString].score < bestscore)
            {
                //수정
                currentData.totalStarCount += Mathf.Max(0, starcount - currentData.bestScores[chapterString].starCount);//수정
                currentData.bestScores[chapterString].score = bestscore;
                currentData.bestScores[chapterString].starCount = starcount;
            }
        }
        else
        {
            currentData.bestScores[chapterString] = ToChapterScore(bestscore, starcount);
            currentData.totalStarCount += starcount;
        }
    }

    public void AutoSave()
    {
        Save(currentData, autoSaveIndex);
    }

    public SaveData Load(int slotNum)
    {
        string path = Path.Combine(Application.persistentDataPath, $"save{slotNum}.json");

        Debug.Log($"[SaveLoad] 로드 시도 경로: {path}");
        Debug.Log($"[SaveLoad] 파일 존재 여부: {File.Exists(path)}");


        if (!File.Exists(path))
        {
            Debug.Log($"세이브 파일 없음{slotNum}");
            return new SaveData();
        }

        string json = File.ReadAllText(path);
        Debug.Log($"[SaveLoad] 로드된 JSON: {json}");
        SaveData data = JsonConvert.DeserializeObject<SaveData>(json);

        return data;
    }

    //chapeterscore에 필요한 값 넣어줘서 타입대로 만들어주는 함수
    private ChapterScore ToChapterScore(int bestscore,int starcount)
    {
        ChapterScore score = new ChapterScore();
        score.score = bestscore;
        score.starCount = starcount;
        return score;
    }
    public bool CurrentDataSet(int index)
    {
        print(index);
        if (savedatas[index].currentChapter==0)
        {
            return false;
        }
        currentData=savedatas[index];
        return true;
    }
    public bool ContinueGame()
    {
        if (PlayerPrefs.HasKey("ContinueGame"))
        {
            SaveLoad.instance.autoSaveIndex = PlayerPrefs.GetInt("ContinueGame");
            Load(PlayerPrefs.GetInt("ContinueGame"));
            return true;
        }
        else
        {
            print("continue할 파일없음");
            PopupManager.instance.OpenInformationPopup("Please Click New Game First");
            return false;
            
        }
        
    }


}
