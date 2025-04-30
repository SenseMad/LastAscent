using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public sealed class ZoneManager : MonoBehaviour
{
  [SerializeField, Min(0)] private int _maxUsedSpawnPoints = 5;

  [Space]
  [SerializeField] private List<Zone> _zones;

  //--------------------------------------

  private NavMeshPath navMeshPath;

  private LevelManager levelManager;
  private WaveManager waveManager;

  private readonly List<Transform> usedSpawnPoints = new();

  //======================================

  [Inject]
  private void Construct(LevelManager parLevelManager, WaveManager parWaveManager)
  {
    levelManager = parLevelManager;
    waveManager = parWaveManager;
  }

  //======================================

  private void Awake()
  {
    navMeshPath = new();
  }

  private void OnEnable()
  {
    waveManager.OnChangeWaveNumber += StartSpawn;
  }

  private void OnDisable()
  {
    waveManager.OnChangeWaveNumber -= StartSpawn;
  }

  //======================================

  public void StartSpawn(int parNumberWAve)
  {
    usedSpawnPoints.Clear();
  }

  //======================================

  public Transform GetNearestAvailableSpawnPoint()
  {
    var availableSpawnPoints = GetValidSpawnPoints();

    _maxUsedSpawnPoints = Mathf.Min(availableSpawnPoints.Count, 5);

    var retNearestPoint = FindNearestAvailablePoint(availableSpawnPoints);

    if (retNearestPoint != null)
      usedSpawnPoints.Add(retNearestPoint);

    if (usedSpawnPoints.Count > _maxUsedSpawnPoints)
      usedSpawnPoints.Clear();

    return retNearestPoint;
  }

  //======================================

  private Transform FindNearestAvailablePoint(List<Transform> parAvailablePoints)
  {
    float nearestDistance = Mathf.Infinity;
    Transform retSpawnPoint = null;
    var playerPos = levelManager.Player.transform.position;

    foreach (var point in parAvailablePoints)
    {
      NavMesh.CalculatePath(point.position, playerPos, NavMesh.AllAreas, navMeshPath);

      if (navMeshPath.status != NavMeshPathStatus.PathComplete)
        continue;

      float distancePlayer = CalculatePathDistance();

      if (distancePlayer < nearestDistance)
      {
        nearestDistance = distancePlayer;
        retSpawnPoint = point;
      }
    }

    return retSpawnPoint;
  }

  private List<Transform> GetValidSpawnPoints()
  {
    var retValidPoints = new List<Transform>();

    foreach (var zone in _zones)
    {
      if (zone == null || !zone.IsUnlocked)
        continue;

      foreach (var point in zone.SpawnPoints)
      {
        if (point == null || usedSpawnPoints.Contains(point))
          continue;

        retValidPoints.Add(point);
      }
    }

    return retValidPoints;
  }

  private float CalculatePathDistance()
  {
    if (navMeshPath.corners.Length < 2)
      return 0;

    float distance = 0;

    for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
      distance += Vector3.Distance(navMeshPath.corners[i], navMeshPath.corners[i + 1]);

    return distance;
  }

  //======================================
}