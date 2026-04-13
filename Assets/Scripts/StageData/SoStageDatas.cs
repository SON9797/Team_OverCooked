using Overcooked;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageDataToDict
{
    public string stageName;
    public LevelData data;
}
public class SoStageDatas : MonoBehaviour
{
    public static SoStageDatas instance; 
    [SerializeField] List<StageDataToDict> levelDatasInput;
    public Dictionary<string, LevelData> levelDatas { get; private set; } = new Dictionary<string, LevelData>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        foreach (StageDataToDict data in levelDatasInput)
        {
            levelDatas[data.stageName] = data.data;
        }

    }
}
