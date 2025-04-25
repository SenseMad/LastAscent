using Unity.Cinemachine;
using Zenject;

public sealed class GameplaySceneInstaller : MonoInstaller
{
  public override void InstallBindings()
  {
    BindFromNewComponentOnNewGameObject();

    BindFromComponentInHierarchy();

    BindInterfacesAndSelfToTick();

    BindInterfacesToTick();
  }

  //======================================

  private void BindFromNewComponentOnNewGameObject()
  {
    Container.Bind<LevelManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    Container.Bind<SpawnEnemyManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
  }

  private void BindFromComponentInHierarchy()
  {
    Container.Bind<CinemachineImpulseSource>().FromComponentInHierarchy().AsSingle().NonLazy();

    Container.Bind<ZoneManager>().FromComponentInHierarchy().AsSingle().NonLazy();
    Container.Bind<WaveManager>().FromComponentInHierarchy().AsSingle().NonLazy();
    //Container.Bind<LevelManager>().FromComponentInHierarchy().AsSingle().NonLazy();
  }

  private void BindInterfacesAndSelfToTick()
  {
    //Container.BindInterfacesAndSelfTo<TickManager>().FromNewComponentOnNewGameObject().AsSingle();
  }

  private void BindInterfacesToTick()
  {
    //Container.BindInterfacesTo<CharacterMovement>().FromComponentInHierarchy().AsSingle();
  }

  //======================================
}