using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeChest : ChestBase
{
  [SerializeField, Min(0)] private int _numberDropped = 3;

  [SerializeField] private List<UpgradeData> _possibleUpgrades;

  [Space]
  [SerializeField] private UpgradeSelect _upgradeSelectPrefab;

  //--------------------------------------

  private readonly List<UpgradeSelect> spawnedUpgrades = new();

  //======================================

  protected override void Open()
  {
    if (_numberDropped > _possibleUpgrades.Count)
      _numberDropped = _possibleUpgrades.Count;

    HashSet<int> usedIndex = new();

    for (int i = 0; i < _numberDropped; i++)
    {
      Vector3 offset = new(-1.5f, 0, _numberDropped - 1 - i);
      Vector3 finalPosition = transform.position + offset;

      int index = Random.Range(0, _possibleUpgrades.Count);
      while (usedIndex.Contains(index))
        index = Random.Range(0, _possibleUpgrades.Count);

      UpgradeData upgradeData = _possibleUpgrades[index];
      usedIndex.Add(index);

      UpgradeSelect upgradeSelect = Instantiate(_upgradeSelectPrefab, transform.position, Quaternion.identity);
      upgradeSelect.transform.localScale = Vector3.one * 0.5f;
      upgradeSelect.OnSelected += OnUpgradeSelect;

      GameObject selectObject = Instantiate(upgradeData.Model, upgradeSelect.transform);
      selectObject.transform.localPosition = Vector3.zero;

      float duration = 1f;
      Sequence sequence = DOTween.Sequence();
      sequence.Append(upgradeSelect.transform.DOJump(finalPosition, 1f, 1, duration).SetEase(Ease.OutQuad))
        .Join(upgradeSelect.transform.DOScale(Vector3.one, duration))
        .Join(upgradeSelect.transform.DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360))
        .OnComplete(() => upgradeSelect.Initialize(upgradeData));

      spawnedUpgrades.Add(upgradeSelect);
    }
  }

  protected override void ItemChosen()
  {
    foreach (var upgrade in spawnedUpgrades)
    {
      if (upgrade == null)
        continue;

      float duration = 1f;
      Sequence sequence = DOTween.Sequence();
      sequence.Append(upgrade.transform.DOJump(transform.position, 1f, 1, duration).SetEase(Ease.OutQuad))
        .Join(upgrade.transform.DOScale(Vector3.one * 0.5f, duration))
        .Join(upgrade.transform.DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360))
        .OnComplete(() => Destroy(upgrade.gameObject));
    }

    base.ItemChosen();
  }

  //======================================

  private void OnUpgradeSelect(UpgradeSelect parUpgradeSelect)
  {
    parUpgradeSelect.OnSelected -= OnUpgradeSelect;

    spawnedUpgrades.Remove(parUpgradeSelect);
    Destroy(parUpgradeSelect.gameObject);

    ItemChosen();
  }

  //======================================
}