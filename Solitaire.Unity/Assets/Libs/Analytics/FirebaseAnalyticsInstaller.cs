using Scripts.Analytics;
using Zenject;

namespace Libs.Analytics
{
    public sealed class FirebaseAnalyticsInstaller : Installer<FirebaseAnalyticsInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<FirebaseAnalyticsController>().AsSingle();
        }
    }
}