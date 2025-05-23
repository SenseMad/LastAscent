using TMPro;
using UnityEngine;

public class UISelectingWeapon : UIBaseSelectingItems
{
  [SerializeField] private TextMeshProUGUI _damageText;

  [SerializeField] private TextMeshProUGUI _critMultiplierText;

  [SerializeField] private TextMeshProUGUI _attackSpeedText;

  [SerializeField] private TextMeshProUGUI _ammoMagazineText;

  //--------------------------------------

  private Weapon weapon;

  private Camera mainCamera;

  //======================================

  private void Awake()
  {
    mainCamera = Camera.main;
  }

  private void LateUpdate()
  {
    transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
  }

  //======================================

  public void Initialize(Weapon parWeapon)
  {
    weapon = parWeapon;
  }

  public override void Open()
  {
    if (weapon == null)
      return;

    UpdateText();

    base.Open();
  }

  //======================================

  private void UpdateText()
  {
    _icon.sprite = weapon.WeaponData.Icon;
    _titleText.text = $"{weapon.WeaponData.Title}";

    _damageText.text = $"{weapon.WeaponData.Damage}";
    _critMultiplierText.text = $"{weapon.WeaponData.CritMultiplier}";
    _attackSpeedText.text = $"{weapon.WeaponData.AttackSpeed}";
    _ammoMagazineText.text = $"{weapon.CurrentAmountAmmoInMagazine}";
  }

  //======================================
}