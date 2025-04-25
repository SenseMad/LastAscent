using Unity.Cinemachine;
using UnityEngine;

public sealed class WeaponRecoil : MonoBehaviour
{
  [SerializeField, Min(0)] private float _forceImpulse = 0.3f;

  [Space]
  [SerializeField, Min(0)] private float _recoilStrength = 1f;

  //--------------------------------------

  private Weapon weapon;

  private CinemachineImpulseSource cinemachineImpulseSource;

  public float RecoilStrength => _recoilStrength;

  //======================================

  private void Awake()
  {
    weapon = GetComponent<Weapon>();

    cinemachineImpulseSource = GetComponentInChildren<CinemachineImpulseSource>();
  }

  private void OnEnable()
  {
    weapon.OnAttack += Weapon_OnAttack;
  }

  private void OnDisable()
  {
    weapon.OnAttack -= Weapon_OnAttack;
  }

  //======================================

  private void Weapon_OnAttack()
  {
    cinemachineImpulseSource?.GenerateImpulseWithForce(_forceImpulse);
  }

  //======================================
}