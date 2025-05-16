using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChest : ChestBase
{
  [SerializeField, Min(0)] private int _numberDropped = 3;

  [SerializeField] private List<Weapon> _weaponPrefabs;

  //--------------------------------------

  private readonly List<Weapon> spawnedWeapons = new();
  
  //======================================

  protected override void Open()
  {
	if (_numberDropped > _weaponPrefabs.Count)
      _numberDropped = _weaponPrefabs.Count;

    HashSet<int> usedIndex = new();

    for (int i = 0; i < _numberDropped; i++)
	{
	  Vector3 offset = new(-1.5f, 0, _numberDropped - 1 - i);
	  Vector3 finalPosition = transform.position + offset;

	  int index = Random.Range(0, _weaponPrefabs.Count);
      while (usedIndex.Contains(index))
        index = Random.Range(0, _weaponPrefabs.Count);

      Weapon weapon = Instantiate(_weaponPrefabs[index], transform.position, Quaternion.identity);
      usedIndex.Add(index);

      GameObject selectObject = new("WeaponSelect");
      selectObject.layer = LayerMask.NameToLayer("Interact");
      selectObject.transform.SetParent(weapon.transform);
      selectObject.transform.localPosition = Vector3.zero;

      WeaponSelect weaponSelect = selectObject.transform.gameObject.AddComponent<WeaponSelect>();
      weaponSelect.OnSelect += WeaponSelect_OnSelect;

      weapon.transform.localScale = Vector3.one * 0.5f;

      float duration = 1f;
      Sequence sequence = DOTween.Sequence();
      sequence.Append(weapon.transform.DOJump(finalPosition, 1f, 1, duration).SetEase(Ease.OutQuad))
        .Join(weapon.transform.DOScale(Vector3.one, duration))
        .Join(weapon.WeaponModelObject.transform.DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360))
        .OnComplete(() => weaponSelect.Initialize(weapon));

      spawnedWeapons.Add(weapon);
    }
  }

  protected override void ItemChosen()
  {
    foreach (var weapon in spawnedWeapons)
    {
      if (weapon == null)
        continue;

      float duration = 1f;
      Sequence sequence = DOTween.Sequence();
      sequence.Append(weapon.transform.DOJump(transform.position, 1f, 1, duration).SetEase(Ease.OutQuad))
        .Join(weapon.transform.DOScale(Vector3.one * 0.5f, duration))
        .Join(weapon.WeaponModelObject.transform.DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360))
        .OnComplete(() => Destroy(weapon.gameObject));
    }

    base.ItemChosen();
  }

  //======================================

  private void WeaponSelect_OnSelect(WeaponSelect parWeaponSelect)
  {
    parWeaponSelect.OnSelect -= WeaponSelect_OnSelect;

    spawnedWeapons.Remove(parWeaponSelect.Weapon);

    ItemChosen();
  }

  //======================================
}