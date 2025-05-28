using Code.Cameras.Provider;
using Code.Enemies.Factory;
using Code.Enemies.Provider;
using Code.Infrastructure.Factory;
using Code.Infrastructure.Services.PersistenceProgress;
using Code.Infrastructure.Services.SaveLoad;
using Code.Infrastructure.Services.StaticData;
using Code.Levels;
using Code.Players.Factory;
using Code.Players.Provider;
using Code.VisionCone.Factory;
using Services.PersistenceProgress;
using UnityEngine.SceneManagement;
using Zenject;

namespace Code.Infrastructure
{
    public class BootstrapInstaller : MonoInstaller, IInitializable
    {
        private const string SceneName = "Game";
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<BootstrapInstaller>().FromInstance(this).AsSingle();

            BindFactory();
            BindSaveLoad();
            BindProgressData();
            BindProvider();
            BindLevelController();
            BindStaticData();
        }

        private void BindFactory()
        {
            Container.BindInterfacesTo<PlayerFactory>().AsSingle();
            Container.BindInterfacesTo<EnemyFactory>().AsSingle();
            Container.BindInterfacesTo<VisionConeFactory>().AsSingle();
            
            Container.BindInterfacesTo<UIFactory>().AsSingle();
        }
        
        private void BindSaveLoad()
        {
            Container.Bind<ISaveLoadService>().To<PrefsSaveLoadService>().AsSingle();
        }

        private void BindProvider()
        {
            Container.Bind<ICameraProvider>().To<CameraProvider>().AsSingle();
            Container.Bind<IPlayerProvider>().To<PlayerProvider>().AsSingle();
            Container.Bind<IEnemyProvider>().To<EnemyProvider>().AsSingle();
        }
        
        private void BindProgressData() =>
            Container.Bind<IPersistenceProgressService>().To<PersistenceProgressService>().AsSingle();
        
        private void BindLevelController()
        {
            Container.Bind<ILevelController>().To<LevelController>().AsSingle();
        }
        
        private void BindStaticData()
        {
            Container.Bind<IStaticDataService>().To<StaticDataService>().AsSingle();
        }
        
        public void Initialize()
        {
            Container.Resolve<IStaticDataService>().LoadData();
            
            SceneManager.LoadScene(SceneName);
        }
    }
}