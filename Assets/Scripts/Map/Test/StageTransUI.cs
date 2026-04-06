using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageTransUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Image _stageImageDisplay;

    private string _currentSceneTarget;
    private bool _isWaitingForInput = false;

    private void Awake()
    {
        if(_panel != null)
        {
            _panel.SetActive(false);
        }
    }
    void Update()
    {
        if(_isWaitingForInput && Input.GetKeyUp(KeyCode.LeftAlt))
        {
            SceneManager.LoadScene(_currentSceneTarget);
        }
    }

    public void ShowUI(string stageTitle, string sceneName, Sprite stageSprite)
    {
        _currentSceneTarget = sceneName;
        if (_stageImageDisplay != null && stageSprite != null)
        {
            _stageImageDisplay.sprite = stageSprite;
            _stageImageDisplay.gameObject.SetActive(true); // 이미지가 있을 때만 켬
        }
        else if (_stageImageDisplay != null)
        {
            _stageImageDisplay.gameObject.SetActive(false); // 이미지가 없으면 끔
        }

        _panel.SetActive(true);
        _isWaitingForInput = true;
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
