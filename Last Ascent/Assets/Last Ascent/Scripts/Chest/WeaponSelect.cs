using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WeaponSelect : MonoBehaviour, IInteractable
{
  private Collider selectCollider;

  //======================================

  public Weapon Weapon { get; private set; }

  //======================================

  public event Action<WeaponSelect> OnSelect;

  //======================================

  private void Awake()
  {
    selectCollider = GetComponent<Collider>();

    selectCollider.isTrigger = true;
  }

  //======================================

  public void Initialize(Weapon parWeapon)
  {
    Weapon = parWeapon;

    Weapon.WeaponModelObject.transform.DORotate(new Vector3(0, 360, 0), 15f, RotateMode.FastBeyond360)
      .SetEase(Ease.Linear)
      .SetLoops(-1);
  }

  public void Interact(Player parPlayer)
  {
    OnSelect?.Invoke(this);

    Weapon.WeaponModelObject.transform.DOKill();
    Weapon.WeaponModelObject.transform.rotation = Quaternion.identity;

    parPlayer.WeaponInventory.Add(Weapon);

    Destroy(gameObject);
  }

  //======================================
}