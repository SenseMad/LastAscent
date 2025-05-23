using System;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public sealed class LevelManager : MonoBehaviour
{
  private RoomManager roomManager;
  private CinemachineCamera cinemachineCamera;
  private UpgradeManager upgradeManager;
  private GameManager gameManager;

  //======================================

  public Player Player { get; private set; }

  public Score Score { get; private set; }

  public LocationData LocationData { get; private set; }
  public InstallerLocationsData InstallerLocationsData { get; private set; }

  public LevelProgressData LevelProgressData { get; private set; }

  //======================================

  public event Action OnInitialize;

  //======================================

  [Inject]
  private void Construct(
    InstallerLocationsData parInstallerLocationsData, 
    RoomManager parRoomManager, 
    CinemachineCamera parCinemachineCamera, 
    UpgradeManager parUpgradeManager,
    GameManager parGameManager
    )
  {
    InstallerLocationsData = parInstallerLocationsData;
    roomManager = parRoomManager;
    cinemachineCamera = parCinemachineCamera;
    upgradeManager = parUpgradeManager;
    gameManager = parGameManager;
  }

  //======================================

  private void Awake()
  {
    LevelProgressData = new LevelProgressData();
  }

  /* private void Awake()
   {
     Score = new Score(100);
   }*/

  private void Start()
  {
    CreatePlayer();

    Initialize(InstallerLocationsData.ListLocationData[0]);

    roomManager.CreateRoom(LocationData.InitialRoom);

    OnInitialize?.Invoke();
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
    Player.GameManagerInitialize(gameManager);
    Player.LevelManagerInitialize(this);
  }

  public float CalculateTotalCritChance()
  {
    return gameManager.GlobalProgressData.CritChance + LevelProgressData.CritChance;
  }

  //======================================
}