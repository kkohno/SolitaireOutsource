using Zenject;

namespace Scripts.Decks
{
    public sealed class DecksServiceInstaller : Installer<DecksServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<DecksService>().AsSingle();
        }
    }
}