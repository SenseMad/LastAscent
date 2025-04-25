using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class Zone : MonoBehaviour
{
  [SerializeField] private List<Transform> _spawnPoints;

  [SerializeField] private bool _isUnlocked;

  //======================================

  public List<Transform> SpawnPoints => _spawnPoints;

  public bool IsUnlocked => _isUnlocked;

  //======================================

  public event Action OnUnlocked;

  //======================================

  public void RoomOpen()
  {
    _isUnlocked = true;
    OnUnlocked?.Invoke();
  }

  //======================================

  public Transform GetRandomSpawnPoint()
  {
    return _spawnPoints[Random.Range(0, _spawnPoints.Count)];
  }

  //======================================
}