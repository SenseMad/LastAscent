using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawnEnemyManager : MonoBehaviour
{
  private RoomWaveManager roomWaveManager;

  private readonly List<Enemy> listCreatedEnemies = new();

  private Coroutine coroutineStartSpawnEnemy;

  //======================================

  public event Action OnAllEnemiesKilled;

  //======================================

  private void OnDestroy()
  {
    roomWaveManager.OnWaveStart -= RoomWaveManager_OnWaveStart;
  }

  //======================================

  public void Initialize(RoomWaveManager parRoomWaveManager)
  {
    roomWaveManager = parRoomWaveManager;
    roomWaveManager.OnWaveStart += RoomWaveManager_OnWaveStart;
  }

  public void CreateEnemy(Enemy parEnemy, Vector3 parPosition)
  {
    StartCoroutine(CreateEnemyWithPortal(parEnemy, parPosition));
  }

  //======================================

  private void EnemyDead(Enemy parEnemy)
  {
    parEnemy.Health.OnInstantlyKill -= () => EnemyDead(parEnemy);

    listCreatedEnemies.Remove(parEnemy);

    if (listCreatedEnemies.Count == 0)
      OnAllEnemiesKilled?.Invoke();
  }

  private void RoomWaveManager_OnWaveStart(RoomWave parRoomWave)
  {
    if (coroutineStartSpawnEnemy != null)
      return;

    coroutineStartSpawnEnemy = StartCoroutine(StartSpawnEnemy());
  }

  //======================================

  private IEnumerator CreateEnemyWithPortal(Enemy parEnemy, Vector3 parPosition)
  {
    if (parEnemy == null || roomWaveManager?.Room == null)
      yield break;

    bool portalIsOpen = false;

    GameObject effectPrefab = parEnemy.SpawnEffectPrefab;
    GameObject portal = null;

    if (effectPrefab != null)
    {
      portal = Instantiate(effectPrefab, new Vector3(parPosition.x, parPosition.y + 1, parPosition.z), Quaternion.identity, roomWaveManager.Room.transform);
      portal.transform.localScale = Vector3.zero;

      portal.transform.DOScale(Vector3.one, 1.5f).SetEase(Ease.InQuad)
        .OnComplete(() => portalIsOpen = true);
    }

    if (portal != null)
      yield return new WaitUntil(() => portalIsOpen);

    yield return new WaitForSeconds(1f);

    Enemy newEnemy = Instantiate(parEnemy, roomWaveManager.Room.transform);
    newEnemy.Initialize();
    newEnemy.HealthInitialize(100);

    newEnemy.transform.SetPositionAndRotation(parPosition, Quaternion.identity);
    newEnemy.Health.OnInstantlyKill += () => EnemyDead(newEnemy);

    listCreatedEnemies.Add(newEnemy);

    newEnemy.transform.localScale = Vector3.zero;
    newEnemy.transform.DOScale(Vector3.one, 1f);
    //newEnemy.transform.DOShakePosition(0.5f, 0.2f);

    if (portal != null)
    {
      yield return new WaitForSeconds(1f);
      portal.transform.DOScale(Vector3.zero, 1).SetEase(Ease.InQuad)
        .OnComplete(() => Destroy(portal));
    }
  }

  private IEnumerator StartSpawnEnemy()
  {
    yield return new WaitUntil(() => roomWaveManager != null && roomWaveManager.Room.IsRoomLoaded);

    if (roomWaveManager.CurrentRoomWave == null)
      yield break;

    foreach (var roomWaveSetting in roomWaveManager.CurrentRoomWave.RoomWaveSettings)
    {
      if (roomWaveSetting == null || roomWaveSetting.Enemy == null || roomWaveSetting.SpawnPoint == null)
        continue;

      CreateEnemy(roomWaveSetting.Enemy, roomWaveSetting.SpawnPoint.position);
      yield return null;
    }

    coroutineStartSpawnEnemy = null;
  }

  //======================================
}