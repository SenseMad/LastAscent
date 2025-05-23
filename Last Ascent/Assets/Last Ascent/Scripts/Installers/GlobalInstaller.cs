using Zenject;

public sealed class GlobalInstaller : MonoInstaller
{
  public override void InstallBindings()
  {
    FromNewComponentOnNewGameObject();

    BindInterfacesToTick();
  }

  //======================================

  private void FromNewComponentOnNewGameObject()
  {
    Container.Bind<GameManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
  }

  private void BindInterfacesToTick()
  {
    //Container.BindInterfacesAndSelfTo<GameManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
  }

  //======================================
}