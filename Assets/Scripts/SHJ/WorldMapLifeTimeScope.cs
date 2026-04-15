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

        var worldMapManager = FindObjectOfType<WorldMapManager>(true);
        if (worldMapManager != null)
        {
            builder.RegisterComponent(worldMapManager);
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

            var irisFader = FindObjectOfType<IrisFader>(true);
            if (irisFader != null)
            {
                container.InjectGameObject(irisFader.gameObject);
            }

            var popupManager = FindObjectOfType<PopupManager>(true);
            if (popupManager != null)
            {
                container.InjectGameObject(popupManager.gameObject);
            }

            var bigToOrigin = FindObjectsOfType<HoverAnime_BigtoOrigin>(true);
            foreach (var button in bigToOrigin)
            {
                container.InjectGameObject(button.gameObject);
            }

            var stageFlags = FindObjectsOfType<StageFlag>(true);
            foreach (var flag in stageFlags)
            {
                container.InjectGameObject(flag.gameObject);
            }

            if (worldMapManager != null)
            {
                container.InjectGameObject(worldMapManager.gameObject);
            }

            var tileRotates = FindObjectsOfType<WorldMapTileRotate>(true);
            foreach (var tile in tileRotates)
            {
                container.InjectGameObject(tile.gameObject);
            }

        });
    }
}
