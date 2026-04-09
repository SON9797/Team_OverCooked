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
        Debug.Log($"[ShowUI 호출] 입력된 키: '{stageKey}'");
        Debug.Log($"[딕셔너리 상태] 현재 등록된 데이터 개수: {_stageDictionary.Count}");

        if (_stageDictionary.TryGetValue(stageKey, out StageData stageData))
        {
            Debug.Log($"[성공] {stageKey} 데이터를 찾았습니다. 씬 이름: {stageData.sceneName}");

            _currentSceneTarget = stageData.sceneName;

            if (_stageImageDisplay != null)
            {
                _stageImageDisplay.sprite = stageData.stageSprite;
                _stageImageDisplay.gameObject.SetActive(stageData.stageSprite != null);
            }

            _panel.SetActive(true);
            _isWaitingForInput = true;
        }
        else
        {
            Debug.LogError($"[실패] '{stageKey}'라는 키를 딕셔너리에서 찾을 수 없습니다! 대소문자나 공백을 확인하세요.");

            // 어떤 키들이 들어있는지 다 찍어보기
            foreach (var key in _stageDictionary.Keys)
            {
                Debug.Log($"현재 딕셔너리에 있는 키: '{key}'");
            }
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
