using Overcooked;
using Overcooked.Interfaces;
using OverCooked;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private PlayerInput _playerMovement;
    [SerializeField] private List<RecipeData> _recipeList;
    [SerializeField] private InGameUIManager _inGameUiManager;
    [SerializeField] private LevelData _currentLevelData;
    [SerializeField] private PlayerSwitchManager _playerSwitchManager;

    [SerializeField] private InGameInputInjector _player1Injector;
    [SerializeField] private InGameInputInjector _player2Injector;

    [SerializeField] private GameObject _platePrefab;
    [SerializeField] private GameObject _dirtyPlatePrefab;

    // VContainer 등록 설정
    protected override void Configure(IContainerBuilder builder)
    {
        if (_currentLevelData != null)
        {
            builder.Register<RecipeManager>(Lifetime.Singleton)
                   .WithParameter(_currentLevelData.Recipes)
                   .AsImplementedInterfaces()
                   .AsSelf();

            builder.RegisterInstance(_currentLevelData);
        }

        // 주문 리스트 UI 등록
        builder.RegisterComponentInHierarchy<OrderListUI>();

        // 타이머 등록
        builder.Register<TimerManager>(Lifetime.Singleton)
               .AsImplementedInterfaces();

        // 인게임 UI 매니저 등록
        if (_inGameUiManager != null)
        {
            builder.RegisterComponent(_inGameUiManager)
                   .AsImplementedInterfaces()
                   .AsSelf();
        }

        // 씬 흐름 매니저 등록
        builder.Register<SceneFlowManager>(Lifetime.Singleton)
               .AsImplementedInterfaces()
               .AsSelf();

        // 인게임 입력 등록
        builder.Register<IInGamePlayerInput, InGamePlayerInput>(Lifetime.Singleton);

        // 플레이어 스위치 매니저 등록
        if (_playerSwitchManager != null)
        {
            builder.RegisterComponent(_playerSwitchManager);
        }

        // 스코어 매니저 등록
        builder.RegisterComponentInHierarchy<ScoreManager>()
               .AsImplementedInterfaces()
               .AsSelf();

        // 주문 매니저 등록
        builder.RegisterComponentInHierarchy<OrderManager>()
               .AsSelf();

        // 제출 카운터 등록
        builder.RegisterComponentInHierarchy<DeliveryCounter>()
               .AsSelf();

        // 접시 리스폰 등록
        builder.RegisterComponentInHierarchy<PlateRespawn>();

        // 접시 팩토리 등록
        builder.Register<PlateFactory>(resolver =>
        {
            return new PlateFactory(resolver, _platePrefab, _dirtyPlatePrefab);
        }, Lifetime.Singleton);

        // Pause Menu 관련 등록
        builder.RegisterComponentInHierarchy<PauseMenuContorller>();

        // 엔딩 별 연출 관련 등록
        builder.RegisterComponentInHierarchy<EndingStarsContorller>();

        // 인게임 사운드 매니저 등록
        builder.RegisterComponentInHierarchy<IInGameSoundManager>()
               .AsImplementedInterfaces()
               .AsSelf();

        // 빌드 완료 후 필요한 게임오브젝트에 주입
        builder.RegisterBuildCallback(container =>
        {
            if (_player1Injector != null)
            {
                container.InjectGameObject(_player1Injector.gameObject);
            }

            if (_player2Injector != null)
            {
                container.InjectGameObject(_player2Injector.gameObject);
            }

            if (_playerSwitchManager != null)
            {
                container.InjectGameObject(_playerSwitchManager.gameObject);
            }

            DishWash[] dishWashes = FindObjectsOfType<DishWash>(true);
            for (int i = 0; i < dishWashes.Length; i++)
            {
                container.InjectGameObject(dishWashes[i].gameObject);
            }            

            var irisFader = FindObjectOfType<IrisFader>(true);
            if (irisFader != null)
            {
                container.InjectGameObject(irisFader.gameObject);
            }

            var bigToOrigin = FindObjectsOfType<HoverAnime_BigtoOrigin>(true);
            foreach (var button in bigToOrigin)
            {
                container.InjectGameObject(button.gameObject);
            }

            var popupManager = FindObjectOfType<PopupManager>(true);
            if (popupManager != null)
            {
                container.InjectGameObject(popupManager.gameObject);
            }

            var hoverSounds = FindObjectsOfType<UIHoverSound>(true);
            foreach (var soundBtn in hoverSounds)
            {
                container.InjectGameObject(soundBtn.gameObject);
            }
        });
    }
}