using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class UpgradeSelect : MonoBehaviour, IInteractable
{
  private Collider selectCollider;

  //======================================

  public UpgradeData UpgradeData { get; private set; }

  //======================================

  public event Action<UpgradeSelect> OnSelected;

  //======================================

  private void Awake()
  {
    selectCollider = GetComponent<Collider>();

    selectCollider.isTrigger = true;
  }

  //======================================

  public void Initialize(UpgradeData parUpgradeData)
  {
    UpgradeData = parUpgradeData;

    transform.DORotate(new Vector3(0, 360, 0), 15f, RotateMode.FastBeyond360)
      .SetEase(Ease.Linear)
      .SetLoops(-1);
  }

  public void Interact(Player parPlayer)
  {
    OnSelected?.Invoke(this);

    parPlayer.UpgradeManager.ApplyUpgrade(parPlayer, UpgradeData);

    transform.DOKill();
    transform.rotation = Quaternion.identity;
  }

  //======================================
}