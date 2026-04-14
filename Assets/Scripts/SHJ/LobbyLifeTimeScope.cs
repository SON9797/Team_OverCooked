using OverCooked;
using UnityEngine;
using VContainer.Unity;
using VContainer;

public class LobbyLifeTimeScope : LifetimeScope
{
    [Header("사운드")]
    [SerializeField] private InGameSoundManager _soundManager;

    [Header("로비")]
    [SerializeField] private AudioClip _lobbyBGM;

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
            var hoverButton = FindObjectsOfType<HoverAnime_MenuApear>(true);
            foreach (var button in hoverButton)
            {
                container.InjectGameObject(button.gameObject);
            }

            var irisFader = FindObjectOfType<IrisFader>(true);
            if (irisFader != null)
            {
                container.InjectGameObject(irisFader.gameObject);
            }
        

            if (_lobbyBGM != null && _soundManager != null)
            {
                _soundManager.PlayBGM(_lobbyBGM);
            }
        });
    }
}
