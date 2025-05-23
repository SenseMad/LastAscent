using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class UpgradeSelect : MonoBehaviour, IInteractable, IDetectable
{
  private Collider selectCollider;

  private UISelectingUpgrade uISelectingUpgrade;

  //======================================

  public UpgradeData UpgradeData { get; private set; }

  //======================================

  public event Action<UpgradeSelect> OnSelected;

  //======================================

  private void Awake()
  {
    selectCollider = GetComponent<Collider>();
    selectCollider.isTrigger = true;

    uISelectingUpgrade = GetComponentInChildren<UISelectingUpgrade>();
  }

  //======================================

  public void Initialize(UpgradeData parUpgradeData)
  {
    UpgradeData = parUpgradeData;

    transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.FastBeyond360)
      .SetEase(Ease.Linear)
      .SetLoops(-1);

   transform.DOMoveY(transform.position.y + 0.3f, 2f)
      .SetEase(Ease.InOutSine)
      .SetLoops(-1, LoopType.Yoyo);
  }

  public void Interact(Player parPlayer)
  {
    if (UpgradeData == null)
      return;

    OnSelected?.Invoke(this);

    parPlayer.UpgradeManager.ApplyUpgrade(UpgradeData);

    transform.DOKill();
  }

  public void Detect()
  {

  }

  public void UnDetect()
  {

  }

  //======================================
}