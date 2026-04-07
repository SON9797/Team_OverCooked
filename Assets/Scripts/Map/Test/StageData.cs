using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData : MonoBehaviour
{
    [System.Serializable]
    public class StageInfo
    {
        public int stageIndex;      // 1, 2, 3, 4
        public string stageName;    // "1-1", "1-2" 등
        public string sceneName;    // 실제 로드할 씬 이름
        public bool isUnlocked;     // 해금 여부
    }
}
