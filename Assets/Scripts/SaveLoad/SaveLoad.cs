using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using Formatting = Newtonsoft.Json.Formatting;

public class ChapterScore
{
    public int main;
    public int sub;
    public int score;
}
public class SaveData
{
    public int currentChapter;
    public int currentSubChapter;
    public int totalStarCount;
    public List<ChapterScore> bestScores = new List<ChapterScore>();



}

public class SaveLoad : MonoBehaviour
{
    public static SaveLoad instance;

    

    public SaveData currentData;

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
        testScore = ToChapterScore(1, 1, 10);
        testData.bestScores.Add(testScore);
        Save(testData, 0);
        */
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
    public ChapterScore ToChapterScore(int mainchapter, int subchapter, int bestscore)
    {
        ChapterScore score = new ChapterScore();
        score.main = mainchapter;
        score.sub = subchapter;
        score.score = bestscore;
        return score;
    }
    public void CurrentDataSet(int index)
    {
        currentData=savedatas[index];
    }
}
