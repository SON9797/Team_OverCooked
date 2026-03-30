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
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private LevelData _currentLevelData;
    [SerializeField] private PlayerSwitchManager _playerSwitchManager;

    [SerializeField] private InGameInputInjector _player1Injector;
    [SerializeField] private InGameInputInjector _player2Injector;
    [SerializeField] private GameObject _platePrefab;

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

        if (_uiManager != null)
        {
            builder.RegisterComponent(_uiManager)
                   .AsImplementedInterfaces()
                   .AsSelf();
        }

        builder.Register<SceneFlowManager>(Lifetime.Singleton)
               .AsImplementedInterfaces().AsSelf();

        builder.Register<IInGamePlayerInput, InGamePlayerInput>(Lifetime.Singleton);

        if (_playerSwitchManager != null)
        {
            builder.RegisterComponent(_playerSwitchManager);
        }

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

        // 스코어 관련 / 레시피 UI 관련
        builder.RegisterComponentInHierarchy<ScoreManager>()
            .AsImplementedInterfaces()
            .AsSelf();
        builder.RegisterComponentInHierarchy<OrderManager>().AsSelf();
        builder.RegisterComponentInHierarchy<DeliveryCounter>().AsSelf();


        builder.RegisterComponentInHierarchy<PlateReSpawn>();

        builder.Register<PlateFactory>(Lifetime.Singleton)
       .WithParameter<GameObject>(_platePrefab);

        // Pause Menu 버튼 관련
        builder.RegisterComponentInHierarchy<PauseMenuContorller>();

    }
}