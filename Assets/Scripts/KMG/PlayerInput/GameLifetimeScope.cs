using Overcooked;
using VContainer;
using VContainer.Unity;


namespace Overcooked
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IInGamePlayerInput, InGamePlayerInput>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<InGameInputInjector>();
        }
    }

}