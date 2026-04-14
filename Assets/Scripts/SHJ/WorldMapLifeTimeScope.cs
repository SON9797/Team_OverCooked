using OverCooked;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class WorldMapLifeTimeScope : LifetimeScope
{
    [Header("사운드 설정")]
    [SerializeField] private WorldMapSoundManager _soundManager;

    [Header("월드맵 배경음")]
    [SerializeField] private AudioClip _worldMapBGM;

    protected override void Configure(IContainerBuilder builder)
    {
        if (_soundManager != null)
        {
            builder.RegisterComponent(_soundManager)
                   .AsImplementedInterfaces()
                   .AsSelf();
        }

        builder.RegisterBuildCallback(container =>
        {
            var busMove = FindObjectOfType<BusMove>(true);
            if (busMove != null)
            {
                container.InjectGameObject(busMove.gameObject);
            }

            if (_worldMapBGM != null && _soundManager != null)
            {
                _soundManager.PlayBGM(_worldMapBGM);
            }
        });
    }
}
