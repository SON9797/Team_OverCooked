using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class StageTransUI : MonoBehaviour
{

    [SerializeField] WorldMapLevelHover levelPanel;
    [SerializeField] private List<StageEntry> _stageEntries;

    private Dictionary<string, StageData> _stageDictionary = new Dictionary<string, StageData>();

    private string _currentSceneTarget;
    private bool _isWaitingForInput = false;

    [System.Serializable]
    public struct StageEntry
    {
        public string stageKey;
        public StageData data;
    }

    private void Awake()
    {
        levelPanel.HideWindow();
        foreach (var entry in _stageEntries)
        {
            if (!_stageDictionary.ContainsKey(entry.stageKey))
            {
                _stageDictionary.Add(entry.stageKey, entry.data);
            }
        }

    }
    void Update()
    {
        if(_isWaitingForInput && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(_currentSceneTarget);
        }
    }

    public void ShowUI(string stageKey)
    {
        if (_stageDictionary.TryGetValue(stageKey, out StageData stageData))
        {
            _currentSceneTarget = stageData.sceneName;

            levelPanel.ShowLevel(stageKey);

            _isWaitingForInput = true;
        }
    }

    public void HideUI()
    {
        levelPanel.HideWindow();
        _isWaitingForInput = false;
    }

}
