using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyWeaponInventory : MonoBehaviour
{
  [SerializeField] private List<EnemyWeapon> _listWeapons = new();

  //--------------------------------------

  private Enemy enemy;

  //======================================

  public EnemyWeapon ActiveWeapon { get; private set; }

  //======================================

  private void Awake()
  {
    enemy = GetComponentInParent<Enemy>();

    ActiveWeapon = _listWeapons[0];

    ActiveWeapon.Initialize(enemy);
  }

  //======================================
}