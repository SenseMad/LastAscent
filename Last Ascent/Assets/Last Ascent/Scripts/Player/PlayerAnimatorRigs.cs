using UnityEngine;
using UnityEngine.Animations.Rigging;

public sealed class PlayerAnimatorRigs : MonoBehaviour
{
  [Header("Rigs")]
  [SerializeField] private Rig _idleWeaponRig;
  [SerializeField] private Rig _aimWeaponRig;

  [Header("Settings")]
  [SerializeField, Min(0)] private float _speedWeight = 10.0f;

  //--------------------------------------

  private Player player;

  private float idleWeaponRigWeight;
  private float aimWeaponRigWeight;

  //======================================

  private void Awake()
  {
    player = GetComponent<Player>();
  }

  private void OnEnable()
  {
    player.Health.OnInstantlyKill += Health_OnInstantlyKill;
  }

  private void OnDestroy()
  {
    player.Health.OnInstantlyKill -= Health_OnInstantlyKill;
  }

  private void Update()
  {
    if (player.Health.IsDead)
      return;

    ChangeRigWeight();
  }

  //======================================

  private void ChangeRigWeight()
  {
    bool weight = player.WeaponInventory.IsInShootinStance && player.WeaponInventory.ActiveWeapon != null;

    idleWeaponRigWeight = weight ? 0 : 1;
    aimWeaponRigWeight = weight ? 1 : 0;

    _idleWeaponRig.weight = Mathf.Lerp(_idleWeaponRig.weight, idleWeaponRigWeight, Time.deltaTime * _speedWeight);
    _aimWeaponRig.weight = Mathf.Lerp(_aimWeaponRig.weight, aimWeaponRigWeight, Time.deltaTime * _speedWeight);
  }

  private void Health_OnInstantlyKill()
  {
    _idleWeaponRig.weight = 0;
    _aimWeaponRig.weight = 0;
  }

  //======================================
}