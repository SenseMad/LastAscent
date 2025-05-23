using TMPro;
using UnityEngine;

public class UISelectingUpgrade : UIBaseSelectingItems
{


  //--------------------------------------

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

  public void Initialize()
  {
    
  }

  public override void Open()
  {
    /*if (weapon == null)
      return;

    UpdateText();

    base.Open();*/
  }

  //======================================

  private void UpdateText()
  {
    /*_icon.sprite = weapon.WeaponData.Icon;
    _titleText.text = $"{weapon.WeaponData.Title}";

    _damageText.text = $"{weapon.WeaponData.Damage}";
    _critMultiplierText.text = $"{weapon.WeaponData.CritMultiplier}";
    _attackSpeedText.text = $"{weapon.WeaponData.AttackSpeed}";
    _ammoMagazineText.text = $"{weapon.CurrentAmountAmmoInMagazine}";*/
  }

  //======================================
}