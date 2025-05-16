using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Data/Upgrade/UpgradeData")]
public class UpgradeData : ScriptableObject
{
  [field: SerializeField] public string Title { get; private set; }

  [field: SerializeField] public string Description { get; private set; }

  [field: SerializeField] public Sprite Icon { get; private set; }
  
  [field: SerializeField] public GameObject Model { get; private set; }

  [field: SerializeField] public UpgradeType UpgradeType { get; private set; }

  [field: SerializeField] public float Value { get; private set; }
}