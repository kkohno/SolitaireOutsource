using Scripts.DecksDataBase.Services;
using Zenject;

namespace Scripts.DecksDataBase
{
    public sealed class DecksDataBaseInstaller : Installer<DecksDataBaseInstaller>
    {
        public override void InstallBindings()
        {
            //Container.BindInterfacesTo<DecksDataBaseService>().AsSingle();
            Container.BindInterfacesTo<DecksDataBaseSqliteService>().AsSingle();
        }
    }
}