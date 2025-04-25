using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InstallerPlayerSkinData", menuName = "Data/Player/Skin/InstallerPlayerSkinData")]
public sealed class InstallerPlayerSkinData : ScriptableObject
{
  [field: SerializeField] public List<PlayerSkinData> PlayerSkinDatas { get; private set; }
}