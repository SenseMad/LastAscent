using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
  /*[SerializeField, Min(0)] private int _damage = 0;
  [SerializeField, Min(0)] protected float _force = 5.0f;
  [SerializeField, Min(0)] private float _critMultiplier = 1.5f;

  [Space]
  [SerializeField, Min(0)] protected int _attackPerMinutes = 200;
  [SerializeField, Min(0)] protected float _attackSpeed = 8;

  [Space]
  [SerializeField] protected Vector3 _size;
  [SerializeField] protected Vector3 _position;
  [SerializeField] protected Vector3 _rotation;*/

  [SerializeField] protected WeaponData _weaponData;

  [Space]
  [SerializeField] private GameObject _weaponModelObject;

  //--------------------------------------

  private int currentAmountAmmo;
  private int currentAmountAmmoInMagazine;

  protected float critChance;

  //======================================

  public WeaponData WeaponData => _weaponData;

  public GameObject WeaponModelObject => _weaponModelObject;

  public bool IsAttack { get; protected set; }

  public bool IsRecharge { get; protected set; }

  public float LastAttackTime { get; protected set; }

  public int CurrentAmountAmmo
  {
    get => currentAmountAmmo;
    protected set
    {
      currentAmountAmmo = value;
      OnAmmoChanged?.Invoke();
    }
  }

  public int CurrentAmountAmmoInMagazine
  {
    get => currentAmountAmmoInMagazine;
    protected set
    {
      currentAmountAmmoInMagazine = value;
      OnAmmoChanged?.Invoke();
    }
  }

  //======================================

  public event Action OnAttack;

  public event Action OnAmmoChanged;

  //======================================

  public virtual bool Attack(GameObject parOwner)
  {
    OnAttack?.Invoke();
    return true;
  }

  public abstract void EquipWeapons();
  public abstract void NotEquipWeapons();

  public virtual void Recharge() { }

  public void SetAttack(bool parIsAttack)
  {
    IsAttack = parIsAttack;
  }

  public void SetSize()
  {
    _weaponModelObject.transform.localScale = _weaponData.Size;
  }

  public void SetPosition()
  {
    transform.localPosition = _weaponData.Position;
  }

  public void SetRotation()
  {
    transform.localRotation = Quaternion.Euler(_weaponData.Rotation);
  }

  public void GetChanceCritDamage(float parCritChance)
  {
    critChance = parCritChance;
  }

  //======================================
}