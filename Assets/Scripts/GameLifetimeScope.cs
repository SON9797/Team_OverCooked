using Overcooked;
using Overcooked.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private PlayerInput _playerMovement;
    [SerializeField] private List<RecipeData> _recipeList;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private LevelData _currentLevelData;
    [SerializeField] private PlayerSwitchManager _playerSwitchManager;

    [SerializeField] private InGameInputInjector _player1Injector;
    [SerializeField] private InGameInputInjector _player2Injector;

    protected override void Configure(IContainerBuilder builder)
    {
        if (_currentLevelData != null)
        {
            builder.Register<RecipeManager>(Lifetime.Singleton)
                   .WithParameter(_currentLevelData.Recipes)
                   .AsImplementedInterfaces();

            builder.RegisterInstance(_currentLevelData);
        }

        builder.RegisterComponentInHierarchy<OrderListUI>();

        builder.Register<ScoreManager>(Lifetime.Singleton)
               .AsImplementedInterfaces();

        builder.Register<TimerManager>(Lifetime.Singleton)
               .AsImplementedInterfaces();

        builder.RegisterEntryPoint<GameLoopManager>();

        if (_levelManager != null)
        {
            builder.RegisterComponent(_levelManager)
                   .AsImplementedInterfaces();
        }

        if (_uiManager != null)
        {
            builder.RegisterComponent(_uiManager)
                   .As<IUIManager>();
        }

        builder.Register<SceneFlowManager>(Lifetime.Singleton)
               .AsImplementedInterfaces();

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
    }
}