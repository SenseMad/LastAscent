using System;
using UnityEngine;

[Serializable]
public class RoomWaveSettings
{
  [field: SerializeField] public Enemy Enemy { get; private set; }
  
  [field: SerializeField] public Transform SpawnPoint { get; private set; }
}