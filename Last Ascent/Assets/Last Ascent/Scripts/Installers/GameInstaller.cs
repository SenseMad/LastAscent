using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameInstaller", menuName = "Installer/GameInstaller")]
public sealed class GameInstaller : ScriptableObjectInstaller<GameInstaller>
{
  [field: SerializeField] public InstallerPlayerSkinData InstallerPlayerSkinData { get; private set; }
  [field: SerializeField] public ListEnemies ListEnemies { get; private set; }

  //======================================

  public override void InstallBindings()
  {
    Container.BindInstance(InstallerPlayerSkinData).AsSingle();
    Container.BindInstance(ListEnemies).AsSingle();
  }

  //======================================
}