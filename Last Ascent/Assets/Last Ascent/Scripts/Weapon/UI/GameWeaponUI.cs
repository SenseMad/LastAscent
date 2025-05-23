using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class GameWeaponUI : MonoBehaviour
{
  [SerializeField] private List<ButtonWeaponUI> _listButtonWeaponUI;

  //--------------------------------------

  private LevelManager levelManager;

  private ButtonWeaponUI currentActiveButtonWeaponUI;

  //======================================

  [Inject]
  private void Construct(LevelManager parLevelManager)
  {
    levelManager = parLevelManager;
  }

  //======================================

  private void OnEnable()
  {
    levelManager.OnInitialize += Initialize;
  }

  private void OnDestroy()
  {
    levelManager.OnInitialize -= Initialize;

    levelManager.Player.WeaponInventory.OnWeaponChanged -= WeaponInventory_OnWeaponChanged;
    levelManager.Player.WeaponInventory.OnAddWeapon -= WeaponInventory_OnAddWeapon;
    levelManager.Player.WeaponInventory.OnReplaceActiveWeapon -= WeaponInventory_OnReplaceActiveWeapon;
  }

  //======================================

  public void Initialize()
  {
    List<Weapon> weapons = levelManager.Player.WeaponInventory.listWeapons;

    for (int i = 0; i < weapons.Count; i++)
    {
      Weapon weapon = weapons[i];
      _listButtonWeaponUI[i].Initialize(weapon);
    }

    currentActiveButtonWeaponUI = _listButtonWeaponUI[0];
    currentActiveButtonWeaponUI.Active();

    levelManager.Player.WeaponInventory.OnWeaponChanged += WeaponInventory_OnWeaponChanged;
    levelManager.Player.WeaponInventory.OnAddWeapon += WeaponInventory_OnAddWeapon;
    levelManager.Player.WeaponInventory.OnReplaceActiveWeapon += WeaponInventory_OnReplaceActiveWeapon;
  }

  //======================================

  private void ChangeButtonWeaponUI(ButtonWeaponUI parNewButtonWeaponUI, Weapon parNewWeapon)
  {
    currentActiveButtonWeaponUI.Deactive();

    currentActiveButtonWeaponUI = parNewButtonWeaponUI;

    currentActiveButtonWeaponUI.Active();
  }

  //======================================

  private void WeaponInventory_OnWeaponChanged(Weapon parNewWeapon)
  {
    foreach (var buttonWeaponUI in _listButtonWeaponUI)
    {
      if (buttonWeaponUI == null)
        continue;

      if (buttonWeaponUI.Weapon != levelManager.Player.WeaponInventory.ActiveWeapon)
        continue;

      ChangeButtonWeaponUI(buttonWeaponUI, parNewWeapon);
    }
  }

  private void WeaponInventory_OnAddWeapon(Weapon parNewWeapon)
  {
    foreach (var buttonWeaponUI in _listButtonWeaponUI)
    {
      if (buttonWeaponUI == null)
        continue;

      if (buttonWeaponUI.Weapon == null)
      {
        ChangeButtonWeaponUI(buttonWeaponUI, parNewWeapon);

        currentActiveButtonWeaponUI.Initialize(parNewWeapon);
        break;
      }
    }
  }

  private void WeaponInventory_OnReplaceActiveWeapon(Weapon parNewWeapon)
  {
    currentActiveButtonWeaponUI.Initialize(parNewWeapon);
  }

  //======================================
}