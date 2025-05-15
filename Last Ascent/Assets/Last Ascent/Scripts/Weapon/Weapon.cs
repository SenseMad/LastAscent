using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
  [SerializeField, Min(0)] protected int _damage = 0;
  [SerializeField, Min(0)] protected float _force = 5.0f;

  [Space]
  [SerializeField] protected Vector3 _size;
  [SerializeField] protected Vector3 _position;
  [SerializeField] protected Vector3 _rotation;

  [Space]
  [SerializeField] private GameObject _weaponModelObject;

  //--------------------------------------

  private int currentAmountAmmo;
  private int currentAmountAmmoInMagazine;

  //======================================

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
    _weaponModelObject.transform.localScale = _size;
  }

  public void SetPosition()
  {
    transform.localPosition = _position;
  }

  public void SetRotation()
  {
    transform.localRotation = Quaternion.Euler(_rotation);
  }

  //======================================
}