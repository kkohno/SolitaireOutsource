using Libs.Analytics;
using Scripts.Decks;
using Scripts.DecksDataBase;
using Scripts.GameEvents;
using Scripts.GameState;
using UnityEngine;
using Zenject;

namespace Scripts.Installers
{
    public sealed class ProjectContextInstaller : MonoInstaller<ProjectContextInstaller>
    {
        [SerializeField]
        bool _debugLog = true;

        public override void InstallBindings()
        {
            GameEventsInstaller.Install(Container);
            Container.BindInterfacesTo<GameStateService>().AsSingle();
            DecksDataBaseInstaller.Install(Container);
            DecksServiceInstaller.Install(Container);
            FirebaseAnalyticsInstaller.Install(Container);
        }
    }
}