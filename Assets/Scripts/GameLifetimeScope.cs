using Overcooked;
using Overcooked.Interfaces;
using OverCooked;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        builder.RegisterComponentInHierarchy<OrderListUI>();

        builder.Register<TimerManager>(Lifetime.Singleton)
               .AsImplementedInterfaces();

        if (_inGameUiManager != null)
        {
            builder.RegisterComponent(_inGameUiManager)
                   .AsImplementedInterfaces()
                   .AsSelf();
        }

        builder.Register<SceneFlowManager>(Lifetime.Singleton)
               .AsImplementedInterfaces()
               .AsSelf();

        builder.Register<IInGamePlayerInput, InGamePlayerInput>(Lifetime.Singleton);

        if (_playerSwitchManager != null)
        {
            builder.RegisterComponent(_playerSwitchManager);
        }

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
        });

        // 스코어 / 주문 / 제출 관련 등록
        builder.RegisterComponentInHierarchy<ScoreManager>()
               .AsImplementedInterfaces()
               .AsSelf();

        builder.RegisterComponentInHierarchy<OrderManager>()
               .AsSelf();

        builder.RegisterComponentInHierarchy<DeliveryCounter>()
               .AsSelf();

        // 접시 리스폰 통합 스크립트 등록
        builder.RegisterComponentInHierarchy<PlateRespawn>();

        // 접시 팩토리 등록
        builder.Register<PlateFactory>(resolver =>
        {
            return new PlateFactory(resolver, _platePrefab, _dirtyPlatePrefab);
        }, Lifetime.Singleton);

        // Pause Menu 버튼 관련
        builder.RegisterComponentInHierarchy<PauseMenuContorller>();

        // Ending Panel 별 연출 관련
        builder.RegisterComponentInHierarchy<EndingStarsContorller>();

        // 인게임 사운드 관련
        builder.RegisterComponentInHierarchy<IInGameSoundManager>()
               .AsImplementedInterfaces()
               .AsSelf();


        builder.RegisterComponentInHierarchy<DishWash>();
    }
}