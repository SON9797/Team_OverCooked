using Overcooked;
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

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<RecipeManager>(Lifetime.Singleton)
               .WithParameter(_recipeList)
               .AsImplementedInterfaces();

        builder.Register<ScoreManager>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<TimerManager>(Lifetime.Singleton).AsImplementedInterfaces();

        builder.RegisterEntryPoint<GameLoopManager>();

        builder.Register<PlayerInput>(Lifetime.Singleton).AsImplementedInterfaces();
        
        if (_playerMovement != null)
        {
            builder.RegisterComponent(_playerMovement);
        }

        if (_levelManager != null)
        {
            builder.RegisterComponent(_levelManager).AsImplementedInterfaces();
        }
    }
}
