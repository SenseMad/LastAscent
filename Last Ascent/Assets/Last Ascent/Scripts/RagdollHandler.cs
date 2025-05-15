using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RagdollHandler : MonoBehaviour
{
  private List<Rigidbody> rigidbodies;

  //======================================

  public void Initialize()
  {
    rigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
    Disable();
  }

  public void Hit(Vector3 parForce, Vector3 parHitPosition)
  {
    Rigidbody injuredRigidbody = rigidbodies.OrderBy(rigidbody => Vector3.Distance(rigidbody.position, parHitPosition)).First();

    injuredRigidbody.AddForceAtPosition(parForce, parHitPosition, ForceMode.Impulse);
  }

  public void Enable()
  {
    foreach (var rigidbody in rigidbodies)
    {
      rigidbody.isKinematic = false;

      if (rigidbody.TryGetComponent(out Collider parCollider))
        parCollider.isTrigger = false;
    }
  }

  public void Disable()
  {
    foreach (var rigidbody in rigidbodies)
    {
      rigidbody.isKinematic = true;

      if (rigidbody.TryGetComponent(out Collider parCollider))
        parCollider.isTrigger = true;
    }
  }

  //======================================
}