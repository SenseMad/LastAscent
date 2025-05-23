using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
  [field: Header("")]
  [field: SerializeField] public Sprite Icon { get; private set; }
  [field: SerializeField] public string Title { get; private set; }

  [field: Header("Damage")]
  [field: SerializeField, Min(0)] public int Damage { get; private set; } = 0;
  [field: SerializeField, Min(0)] public float Force { get; private set; } = 5.0f;
  [field: SerializeField, Min(0)] public float CritMultiplier { get; private set; } = 1.5f;

  [field: Header("Attack")]
  [field: SerializeField, Min(0)] public int AttackPerMinutes { get; private set; } = 200;
  [field: SerializeField, Min(0)] public float AttackSpeed { get; private set; } = 8;

  [field: Header("Ammo")]
  [field: SerializeField, Min(0)] public int AmountAmmo { get; private set; }
  [field: SerializeField, Min(0)] public int AmountAmmoInMagazine { get; private set; }
  
  [field: Header("Position & Rotation")]
  [field: SerializeField] public Vector3 Size { get; private set; }
  [field: SerializeField] public Vector3 Position { get; private set; }
  [field: SerializeField] public Vector3 Rotation { get; private set; }
}