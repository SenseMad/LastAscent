using DG.Tweening;
using System;
using UnityEngine;

public sealed class RoomPortal : MonoBehaviour
{
  [SerializeField] private bool _isPortalOpen;

  [SerializeField] private GameObject _portalObject;

  //--------------------------------------

  private Collider colliderPortal;

  //======================================

  public event Action OnPortalOpen;

  public event Action OnPlayerEnteredPortal;

  //======================================

  private void Awake()
  {
    colliderPortal = GetComponent<Collider>();
  }

  private void Start()
  {
    colliderPortal.enabled = _isPortalOpen;

    _portalObject.SetActive(_isPortalOpen);
  }

  //======================================

  public void PortalOpen()
  {
    _isPortalOpen = true;

    _portalObject.SetActive(_isPortalOpen);
    _portalObject.transform.localScale = Vector3.zero;
    _portalObject.transform.DOScale(Vector3.one, 1).SetEase(Ease.InQuad)
      .OnComplete(() => colliderPortal.enabled = _isPortalOpen);

    OnPortalOpen?.Invoke();
  }

  //======================================

  private void OnTriggerEnter(Collider other)
  {
    if (!_isPortalOpen)
      return;

    if (!other.GetComponent<Player>())
      return;

    OnPlayerEnteredPortal?.Invoke();
  }

  //======================================
}