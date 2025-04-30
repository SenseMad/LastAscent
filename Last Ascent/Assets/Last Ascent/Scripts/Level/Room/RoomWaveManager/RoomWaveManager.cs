using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class RoomWaveManager : MonoBehaviour
{
  [SerializeField] protected List<RoomWave> _roomWaves;

  //--------------------------------------

  private RoomSpawnEnemyManager roomSpawnEnemyManager;

  protected int currentIndexRoomWave;

  //======================================

  public Room Room { get; private set; }

  public RoomWave CurrentRoomWave { get; protected set; }

  //======================================

  public event Action<RoomWave> OnWaveStart;
  public event Action<RoomWave> OnWaveStarted;

  public event Action<RoomWave> OnWaveComplete;

  public event Action OnWavesAreOver;

  //======================================

  private void Awake()
  {
    Room = GetComponentInParent<Room>();

    Initialize();
  }

  private void Start()
  {
    NextWave();
  }

  protected virtual void OnDestroy()
  {
    roomSpawnEnemyManager.OnAllEnemiesKilled -= NextWave;
  }

  //======================================

  public void Initialize()
  {
    _roomWaves = GetComponentsInChildren<RoomWave>().ToList();

    roomSpawnEnemyManager = Room.gameObject.AddComponent<RoomSpawnEnemyManager>();
    roomSpawnEnemyManager.Initialize(this);
    roomSpawnEnemyManager.OnAllEnemiesKilled += NextWave;
  }

  //======================================

  protected virtual void NextWave()
  {
    CurrentRoomWave = _roomWaves[currentIndexRoomWave];

    currentIndexRoomWave++;
    OnWaveStart?.Invoke(CurrentRoomWave);
  }

  protected virtual void WavesAreOver()
  {
    Debug.Log("The waves are over");

    Room.RoomPortal.PortalOpen();

    OnWavesAreOver?.Invoke();
  }

  //======================================
}