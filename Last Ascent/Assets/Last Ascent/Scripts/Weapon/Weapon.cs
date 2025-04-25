using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
  [SerializeField, Min(0)] protected int _damage;

  [Space]
  [SerializeField] protected Vector3 _position;
  [SerializeField] protected Vector3 _rotation;

  //--------------------------------------

  private int currentAmountAmmo;
  private int currentAmountAmmoInMagazine;

  //======================================

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

  public virtual bool Attack()
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