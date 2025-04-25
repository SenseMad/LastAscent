using System;
using UnityEngine;

public sealed class LevelManager : MonoBehaviour
{
  public Player Player { get; private set; }

  [field: SerializeField] public Score Score { get; private set; }

  //======================================

  private void Awake()
  {
    Player = FindAnyObjectByType<Player>();

    Score = new Score(100);
  }

  //======================================
}