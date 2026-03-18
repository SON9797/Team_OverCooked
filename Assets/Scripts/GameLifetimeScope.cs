using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;


public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private List<RecipeData> _recipeList;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<RecipeManager>(Lifetime.Singleton)
               .WithParameter(_recipeList)
               .AsImplementedInterfaces();

        builder.Register<ScoreManager>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<TimerManager>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<RecipeManager>(Lifetime.Singleton).AsImplementedInterfaces();

        builder.RegisterEntryPoint<GameLoopManager>();
    }
}
