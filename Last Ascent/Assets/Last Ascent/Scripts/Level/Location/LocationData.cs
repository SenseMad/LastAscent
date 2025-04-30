using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocationData", menuName = "Data/Location/LocationData")]
public sealed class LocationData : ScriptableObject
{
  [field: SerializeField] public int NumberLocataion { get; private set; }
  
  [field: SerializeField] public Room InitialRoom { get; private set; }
  [field: SerializeField] public List<Room> ListRoomPrefabs { get; private set; }
}