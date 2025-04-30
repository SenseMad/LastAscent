using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InstallerLocationsData", menuName = "Data/Location/InstallerLocationsData")]
public sealed class InstallerLocationsData : ScriptableObject
{
  [field: SerializeField] public List<LocationData> ListLocationData { get; private set; }
}