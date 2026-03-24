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

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<RecipeManager>(Lifetime.Singleton)
               .WithParameter(_currentLevelData.Recipes)
               .AsImplementedInterfaces();

        builder.RegisterComponentInHierarchy<OrderListUI>();

        builder.Register<ScoreManager>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<TimerManager>(Lifetime.Singleton).AsImplementedInterfaces();

        builder.RegisterEntryPoint<GameLoopManager>();

        builder.Register<PlayerInput>(Lifetime.Singleton).AsImplementedInterfaces();    // 삭제 예정

        if (_playerMovement != null)        // 삭제 예정
        {
            builder.RegisterComponent(_playerMovement);
        }

        if (_levelManager != null)
        {
            builder.RegisterComponent(_levelManager).AsImplementedInterfaces();
        }

        builder.RegisterComponent(_uiManager).As<IUIManager>();

        builder.Register<SceneFlowManager>(Lifetime.Singleton).AsImplementedInterfaces();

        builder.RegisterInstance(_currentLevelData);

        builder.Register<Overcooked.IInGamePlayerInput, Overcooked.InGamePlayerInput>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<Overcooked.InGameInputInjector>();

    }
}