using System;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public sealed class LevelManager : MonoBehaviour
{
  private RoomManager roomManager;
  private CinemachineCamera cinemachineCamera;
  private UpgradeManager upgradeManager;

  //======================================

  public Player Player { get; private set; }

  public Score Score { get; private set; }

  public LocationData LocationData { get; private set; }
  public InstallerLocationsData InstallerLocationsData { get; private set; }

  //======================================
  
  [Inject]
  private void Construct(InstallerLocationsData parInstallerLocationsData, RoomManager parRoomManager, CinemachineCamera parCinemachineCamera, UpgradeManager parUpgradeManager)
  {
    InstallerLocationsData = parInstallerLocationsData;
    roomManager = parRoomManager;
    cinemachineCamera = parCinemachineCamera;
    upgradeManager = parUpgradeManager;
  }

  //======================================

 /* private void Awake()
  {
    Score = new Score(100);
  }*/

  private void Start()
  {
    CreatePlayer();

    Initialize(InstallerLocationsData.ListLocationData[0]);

    roomManager.CreateRoom(LocationData.InitialRoom);
  }

  //======================================

  public void Initialize(LocationData parLocationData)
  {
    LocationData = parLocationData;
  }

  public void CreatePlayer()
  {
    var playerPrefab = Resources.Load<Player>("Player/Player");

    Player = Instantiate(playerPrefab);
    Player.CameraController.Initialize(cinemachineCamera);
    Player.UpgradeManagerInitialize(upgradeManager);
  }

  //======================================
}