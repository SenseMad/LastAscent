using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSkinData", menuName = "Data/Player/Skin/PlayerSkinData")]
public sealed class PlayerSkinData : ScriptableObject
{
  [field: SerializeField, Min(0)] public int Index { get; private set; }

  /*[field: SerializeField, Min(0)] public float SpeedWalking { get; private set; } = 10.0f;

  [field: SerializeField, Min(0)] public float Acceleration { get; private set; } = 9.0f;
  [field: SerializeField, Min(0)] public float Deceleration { get; private set; } = 11.0f;*/

  //[field: SerializeField] public Health Health { get; private set; }

  [field: SerializeField] public Sprite Icon { get; private set; }

  [field: SerializeField] public GameObject SkinModel { get; private set; }
}