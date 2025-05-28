using Zenject;

namespace Scripts.GameEvents
{
    public sealed class GameEventsInstaller : Installer<GameEventsInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameEventsService>().AsSingle();
        }
    }
}