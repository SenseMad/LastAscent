using System;
using System.Collections;
using UnityEngine;

public sealed class WaveManager : MonoBehaviour
{
  [SerializeField, Min(0)] private float _timeBetweenWaves = 2.0f;
  [SerializeField, Min(0)] private float _startTimeAfterWaveChange = 2.0f;

  //--------------------------------------

  private Coroutine startWaveCoroutine;

  private int numberWave;

  //======================================

  public int NumberWaves
  {
    get => numberWave;
    private set
    {
      numberWave = value;
      OnChangeWaveNumber?.Invoke(value);
    }
  }

  //======================================

  public event Action<int> OnChangeWaveNumber;

  public event Action<int> OnWaveStarted;

  public event Action OnWaveEnded;

  //======================================

  private void Start()
  {
    NextWave();
  }

  private void OnEnable()
  {
    OnWaveEnded += NextWave;
  }

  private void OnDisable()
  {
    OnWaveEnded -= NextWave;
  }

  //======================================

  public void WaveEnd()
  {
    OnWaveEnded?.Invoke();
  }

  //======================================

  private void NextWave()
  {
    if (startWaveCoroutine != null)
      return;

    startWaveCoroutine = StartCoroutine(StartNextWave());
  }

  private IEnumerator StartNextWave()
  {
    yield return new WaitForSeconds(_timeBetweenWaves);

    NumberWaves++;

    yield return new WaitForSeconds(_startTimeAfterWaveChange);

    OnWaveStarted?.Invoke(NumberWaves);

    startWaveCoroutine = null;
  }

  //======================================
}