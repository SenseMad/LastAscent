using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PhysicsProjectile : BaseProjectile
{
  private Collider currentCollider;

  private Rigidbody rb;

  //======================================

  private void Awake()
  {
    currentCollider = GetComponent<Collider>();

    rb = GetComponent<Rigidbody>();

    if (owner != null)
      Physics.IgnoreCollision(owner.GetComponent<Collider>(), currentCollider);
  }

  //======================================

  public override void Launch(Vector3 parDirection, float parSpeed)
  {
    rb.angularVelocity = parDirection * _speed;
  }

  //======================================

  private void OnCollisionEnter(Collision parCollision)
  {
    Vector3 hitPoint = parCollision.contacts[0].point;
    Vector3 hitNormal = parCollision.contacts[0].normal;

    OnHit(hitPoint, hitNormal, parCollision.collider);
  }

  //======================================
}