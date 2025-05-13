using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class SpawnEnemyManager : MonoBehaviour
{
  private const int InitialHealth = 100;

  //======================================

  private WaveManager waveManager;
  private ZoneManager zoneManager;
  private ListEnemies listEnemies;
  private LevelManager levelManager;

  private Coroutine spawnEnemiesCoroutine;

  private List<Enemy> listSpawnedEnemies = new();

  //======================================

  public int TotalNumberEnemiesInWave { get; private set; }
  public int CurrentNumberSpawnedEnemies { get; private set; }

  //======================================

  [Inject]
  private void Construct(WaveManager parWaveManager, ZoneManager parZoneManager, ListEnemies parListEnemies, LevelManager parLevelManager)
  {
    waveManager = parWaveManager;
    zoneManager = parZoneManager;
    listEnemies = parListEnemies;
    levelManager = parLevelManager;
  }

  //======================================

  private void OnEnable()
  {
    //waveManager.OnWaveStarted += StartEnemySpawn;
  }

  private void OnDisable()
  {
    //waveManager.OnWaveStarted -= StartEnemySpawn;
  }

  //======================================

  private void StartEnemySpawn(int parNumberWave)
  {
    TotalNumberEnemiesInWave = CalculateEnemiesForWave();

    spawnEnemiesCoroutine = StartCoroutine(SpawnEnemies());
  }

  private void SpawnEnemy(EnemyType parEnemyType)
  {
    Transform spawnPoint = zoneManager.GetNearestAvailableSpawnPoint();

    foreach (var enemy in listEnemies.Enemies)
    {
      if (enemy == null/* || enemy.EnemyType != parEnemyType*/)
        continue;

      Enemy newEnemy = Instantiate(enemy, spawnPoint.position, Quaternion.identity);
      newEnemy.Initialize();
      newEnemy.HealthInitialize(CalculateEnemiesForHealth());

      newEnemy.Health.OnInstantlyKill += () => EnemyKilled(newEnemy);

      OnEnemyTakeDamage(newEnemy);

      listSpawnedEnemies.Add(newEnemy);
      CurrentNumberSpawnedEnemies++;
      break;
    }
  }

  private void EnemyKilled(Enemy parEnemy)
  {
    if (CurrentNumberSpawnedEnemies <= 0)
      return;

    CurrentNumberSpawnedEnemies--;
    parEnemy.Health.OnInstantlyKill -= () => EnemyKilled(parEnemy);

    if (CurrentNumberSpawnedEnemies <= 0 && spawnEnemiesCoroutine == null)
      waveManager.WaveEnd();
  }

  private void OnEnemyTakeDamage(Enemy parEnemy)
  {
    foreach (var bodyPart in parEnemy.EnemyBodyParts)
    {
      if (bodyPart == null)
        continue;

      bodyPart.OnHit += BodyPart_OnHit;
    }
  }

  private void BodyPart_OnHit(BodyPartType parPartType)
  {
    switch (parPartType)
    {
      case BodyPartType.Head:
        levelManager.Score.AddScore(50);
        break;
      case BodyPartType.Torso:
        levelManager.Score.AddScore(25);
        break;
      case BodyPartType.Arm:
      case BodyPartType.Leg:
        levelManager.Score.AddScore(10);
        break;
    }
  }

  private IEnumerator SpawnEnemies()
  {
    WaitForSeconds delayBetweenSpawnEnemies = new(0.5f);

    for (int i = 0; i < TotalNumberEnemiesInWave; i++)
    {
      SpawnEnemy(EnemyType.Standart);

      yield return delayBetweenSpawnEnemies;
    }

    if (waveManager.NumberWaves % 5 == 0)
      SpawnEnemy(EnemyType.Boss);

    spawnEnemiesCoroutine = null;
  }

  //======================================

  private int CalculateEnemiesForHealth()
  {
    return InitialHealth + (InitialHealth / 2 * (waveManager.NumberWaves - 1));
    //return Mathf.RoundToInt(100 * Mathf.Pow(1.15f, WaveManager.NumberWaves - 1));
  }

  private int CalculateEnemiesForWave()
  {
    return 5 + (waveManager.NumberWaves - 1) * 3;
  }

  //======================================
}