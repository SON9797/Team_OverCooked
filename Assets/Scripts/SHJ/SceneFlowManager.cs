using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Overcooked
{
    public class SceneFlowManager : IStartable
    {
        private readonly IUIManager _uiManager;

        private readonly float _gamePlayTime = 150f; // 임시

        [Inject]
        public SceneFlowManager(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }

        public void Start()
        {
            _uiManager.SetPanelActive(_uiManager.TutorialPanel, false);
            _uiManager.SetPanelActive(_uiManager.ReadyPanel, false);
            _uiManager.SetPanelActive(_uiManager.StartPanel, false);
            _uiManager.SetPanelActive(_uiManager.CoinPanel, false);
            _uiManager.SetPanelActive(_uiManager.RecipePanel, false);
            _uiManager.SetPanelActive(_uiManager.TimerPanel, false);
            _uiManager.SetPanelActive(_uiManager.EndingPanel, false);

            _uiManager.StartManagerCoroutine(InitializeFlowCoroutine());
        }


        private IEnumerator InitializeFlowCoroutine()
        {
            // 1. 로딩화면
            _uiManager.SetPanelActive(_uiManager.LoadingPanel, true);
            yield return new WaitForSeconds(3.0f);

            // 2. 레시피튜토리얼
            _uiManager.SetPanelActive(_uiManager.LoadingPanel, false);
            _uiManager.SetPanelActive(_uiManager.TutorialPanel, true);

            // 3. 레시피 튜토리얼에서 Space 입력시 사라지고
            while (!Input.GetKeyDown(KeyCode.Space))
            {
                yield return null; // 스페이스바가 눌릴 때까지 무한 대기
            }
            _uiManager.SetPanelActive(_uiManager.TutorialPanel, false);

            _uiManager.SetPanelActive(_uiManager.CoinPanel, true);
            _uiManager.SetPanelActive(_uiManager.RecipePanel, true);
            _uiManager.SetPanelActive(_uiManager.TimerPanel, true);

            // 4. Ready 1.5초
            _uiManager.SetPanelActive(_uiManager.ReadyPanel, true);
            yield return new WaitForSeconds(1.5f);

            // 5. Start 1.5초 보여주고
            _uiManager.SetPanelActive(_uiManager.ReadyPanel, false);
            _uiManager.SetPanelActive(_uiManager.StartPanel, true);
            yield return new WaitForSeconds(1.5f);

            // 6. 게임 시작
            _uiManager.SetPanelActive(_uiManager.StartPanel, false);

            // 7. 엔딩 타이틀
            yield return new WaitForSeconds(_gamePlayTime);

            _uiManager.SetPanelActive(_uiManager.CoinPanel, false);
            _uiManager.SetPanelActive(_uiManager.RecipePanel, false);
            _uiManager.SetPanelActive(_uiManager.TimerPanel, false);

            _uiManager.SetPanelActive(_uiManager.EndingPanel, true);
        }
    }
}
