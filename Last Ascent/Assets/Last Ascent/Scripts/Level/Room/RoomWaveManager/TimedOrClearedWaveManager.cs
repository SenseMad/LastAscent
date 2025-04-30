using System.Collections;
using UnityEngine;

public sealed class TimedOrClearedWaveManager : RoomWaveManager
{
  [Space]
  [SerializeField, Min(0)] private float _timeStartNextWave = 10.0f;

  //--------------------------------------

  private Coroutine startWaveCoroutine;

  //======================================

  protected override void NextWave()
  {
    if (currentIndexRoomWave >= _roomWaves.Count)
    {
      WavesAreOver();
      return;
    }

    if (startWaveCoroutine != null)
    {
      StopCoroutine(startWaveCoroutine);
      startWaveCoroutine = null;
    }

    base.NextWave();

    if (currentIndexRoomWave + 1 > _roomWaves.Count)
    {
      Debug.Log("The last wave");
      return;
    }

    startWaveCoroutine = StartCoroutine(StartNextWave());
  }

  protected override void WavesAreOver()
  {
    base.WavesAreOver();

    if (startWaveCoroutine != null)
    {
      StopCoroutine(startWaveCoroutine);
      startWaveCoroutine = null;
    }
  }

  //======================================

  private IEnumerator StartNextWave()
  {
    float elapsedTime = 0f;

    while (elapsedTime < _timeStartNextWave)
    {
      elapsedTime += Time.deltaTime;
      yield return null;
    }

    startWaveCoroutine = null;
    NextWave();
  }

  //======================================
}