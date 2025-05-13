using System.Collections.Generic;
using UnityEngine;

public sealed class Room : MonoBehaviour
{
  [SerializeField] private Transform _spawnPointPlayer;

  [SerializeField] private List<Transform> _spawnPointEnemies;

  //======================================

  public LevelManager LevelManager { get; private set; }

  public RoomPortal RoomPortal { get; private set; }

  public Transform SpawnPointPlayer => _spawnPointPlayer;
  public List<Transform> SpawnPointEnemies => _spawnPointEnemies;

  public bool IsRoomCreated { get; private set; }

  public bool IsRoomLoaded { get; private set; }

  //======================================

  private void Awake()
  {
    RoomPortal = GetComponentInChildren<RoomPortal>();
  }

  //======================================

  public void Initialize(LevelManager parLeveManager)
  {
    LevelManager = parLeveManager;

    IsRoomCreated = true;
  }

  public void RoomLoaded()
  {
    IsRoomLoaded = true;
  }

  //======================================
}