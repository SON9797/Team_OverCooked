using Overcooked;
using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace OverCooked
{
    public class PauseMenuContorller : MonoBehaviour
    {
        [Header("버튼 연결")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _exitButton;

        private InGameUIManager _inGameUIManager;

        [Inject]
        public void Construct(InGameUIManager inGameUIManager)
        {
            _inGameUIManager = inGameUIManager;
        }

        private void Start()
        {
            if (_resumeButton != null)
            {
                _resumeButton.onClick.AddListener(OnClickResume);
            }

            //if (_restartButton != null)
            //{
            //    _restartButton.onClick.AddListener(OnClickRestart);
            //}
            //
            //if (_exitButton != null)
            //{
            //    _exitButton.onClick.AddListener(OnClickExit);
            //}
        }

        public void OnClickResume()
        {
            _inGameUIManager.SetPause(false);
        }

        //public void OnClickRestart()
        //{
        //    Time.timeScale = 1f;
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}
        //
        //public void OnClickExit()
        //{
        //    Time.timeScale = 1f;
        //    SceneManager.LoadScene("WorldMapScene");
        //}
    }
}
