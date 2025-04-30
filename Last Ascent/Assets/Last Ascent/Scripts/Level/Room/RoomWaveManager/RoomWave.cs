using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RoomWave : MonoBehaviour
{
  [SerializeField] private List<RoomWaveSettings> _roomWaveSettings;

  //======================================

  public List<RoomWaveSettings> RoomWaveSettings => _roomWaveSettings;

  //======================================
}