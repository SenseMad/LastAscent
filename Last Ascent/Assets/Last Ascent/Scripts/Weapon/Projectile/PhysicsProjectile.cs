using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PhysicsProjectile : BaseProjectile
{
  private Rigidbody rb;

  //======================================

  protected override void Awake()
  {
    base.Awake();

    rb = GetComponent<Rigidbody>();
  }

  //======================================

  public override void Launch(Vector3 parDirection, float parSpeed, float parForce)
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