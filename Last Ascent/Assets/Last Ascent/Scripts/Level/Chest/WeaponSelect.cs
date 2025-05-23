using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class WeaponSelect : MonoBehaviour, IInteractable, IDetectable
{
  private Collider selectCollider;

  private UISelectingWeapon uISelectingWeapon;

  //======================================

  public Weapon Weapon { get; private set; }

  //======================================

  public event Action<WeaponSelect> OnSelected;

  //======================================

  private void Awake()
  {
    selectCollider = GetComponent<Collider>();
    selectCollider.isTrigger = true;

    uISelectingWeapon = GetComponentInChildren<UISelectingWeapon>(true);
  }

  private void OnDestroy()
  {
    Weapon.WeaponModelObject.transform.DOKill();
    Weapon.WeaponModelObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
  }

  //======================================

  public void Initialize(Weapon parWeapon)
  {
    Weapon = parWeapon;

    uISelectingWeapon.Initialize(Weapon);

    Weapon.WeaponModelObject.transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.FastBeyond360)
      .SetEase(Ease.Linear)
      .SetLoops(-1);

    Weapon.WeaponModelObject.transform.DOMoveY(Weapon.WeaponModelObject.transform.position.y + 0.3f, 2f)
      .SetEase(Ease.InOutSine)
      .SetLoops(-1, LoopType.Yoyo);
  }

  public void Interact(Player parPlayer)
  {
    if (Weapon == null)
      return;

    OnSelected?.Invoke(this);

    parPlayer.WeaponInventory.Add(Weapon);

    Destroy(gameObject);
  }

  public void Detect()
  {
    if (uISelectingWeapon == null)
      return;

    uISelectingWeapon.Open();
  }

  public void UnDetect()
  {
    if (uISelectingWeapon == null)
      return;

    uISelectingWeapon.Close();
  }

  //======================================
}