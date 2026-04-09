using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
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

    public ChapterScore GetScore(Chapter chapter)
    {
        string key = chapter.ToKey();

        if (bestScores.TryGetValue(key, out var score))
            return score;

        return null;
    }


}

public class SaveLoad : MonoBehaviour
{
    public static SaveLoad instance;

    

    public SaveData currentData;
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
        
        SaveData testData = new SaveData();
        testData.currentChapter = 1;
        testData.currentSubChapter = 1;
        ChapterScore testScore = new ChapterScore();
        testScore = ToChapterScore(200, 2);
        testData.totalStarCount = 2;
        testData.bestScores["1-1"]=testScore;
        Save(testData, 0);
        
        LoadAllSlot();
        Debug.Log($"{savedatas[0].currentChapter}");
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
    public void CurrentDataUpdate(int mainChapter,int subChapter,int bestscore,int starcount)
    {
        //제일 멀리간 스테이지 업데이트
        if (currentData.currentChapter < mainChapter)
        {
            currentData.currentChapter = mainChapter;
            currentData.currentSubChapter = subChapter;
        }
        else if(currentData.currentChapter==mainChapter && currentData.currentSubChapter<subChapter)
        {
            currentData.currentSubChapter = subChapter;
            
        }
        //베스트 스코어, 별 업데이트
        Chapter chapter =new Chapter();
        chapter.mainChapter=mainChapter;
        chapter.subChapter=subChapter;
        
        string chapterString=chapter.ToKey();
        //이미 저장된 점수가 있으면 비교하고 교체
        if (currentData.bestScores.ContainsKey(chapterString))
        {
            if (currentData.bestScores[chapterString].score < bestscore)
            {
                if (starcount - currentData.bestScores[chapterString].starCount > 0)
                {
                    currentData.totalStarCount += starcount - currentData.bestScores[chapterString].starCount;
                }
                currentData.bestScores[chapterString].starCount = starcount;
                currentData.bestScores[chapterString].score = bestscore;
            }
        }
        //새로운 저장이면 그냥 저장.
        else
        {
            currentData.bestScores[chapterString].starCount = starcount;
            currentData.bestScores[chapterString].score = bestscore;

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
        if (!File.Exists(path))
        {
            Debug.Log($"세이브 파일 없음{slotNum}");
            return new SaveData();
        }

        string json = File.ReadAllText(path);
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
}
