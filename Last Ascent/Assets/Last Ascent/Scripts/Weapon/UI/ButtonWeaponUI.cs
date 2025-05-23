using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ButtonWeaponUI : MonoBehaviour
{
  [SerializeField] private Image _icon;

  [SerializeField] private TextMeshProUGUI _ammoText;

  [SerializeField] private GameObject _transformSelected;

  //======================================

  public Weapon Weapon { get; private set; }

  //======================================

  private void Awake()
  {
    _icon.gameObject.SetActive(false);
    _ammoText.gameObject.SetActive(false);
    Deactive();
  }

  private void OnDestroy()
  {
    if (Weapon != null)
      Weapon.OnAmmoChanged -= UpdateTextAmmo;
  }

  //======================================

  public void Initialize(Weapon parWeapon)
  {
    if (parWeapon == null)
      return;

    _icon.gameObject.SetActive(true);
    _ammoText.gameObject.SetActive(true);

    Weapon = parWeapon;
    Weapon.OnAmmoChanged += UpdateTextAmmo;

    UpdateTextAmmo();

    _icon.sprite = Weapon.WeaponData.Icon;
  }

  public void Active()
  {
    _transformSelected.SetActive(true);
  }

  public void Deactive()
  {
    _transformSelected.SetActive(false);
  }

  //======================================

  private void UpdateTextAmmo()
  {
    _ammoText.text = $"{Weapon.CurrentAmountAmmoInMagazine}/{Weapon.CurrentAmountAmmo}";
  }

  //======================================
}