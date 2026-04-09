using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class StageTransUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Image _stageImageDisplay;

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
        if(_panel != null)
        {
            _panel.SetActive(false);
        }

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

            if (_stageImageDisplay != null)
            {
                _stageImageDisplay.sprite = stageData.stageSprite;
                _stageImageDisplay.gameObject.SetActive(stageData.stageSprite != null);
            }

            _panel.SetActive(true);
            _isWaitingForInput = true;
        }
    }

    public void HideUI()
    {
        if (_panel != null)
        {
            _panel.SetActive(false);
            _isWaitingForInput = false;
        }
    }

}
