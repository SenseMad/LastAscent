using UnityEngine;

public sealed class DirectProjectile : BaseProjectile
{
  protected override void Collider(Collision parCollision)
  {
    if (parCollision.collider.TryGetComponent(out EnemyBodyPart parEnemyBodyPart))
      parEnemyBodyPart.TakeHit(damage);
    else if (parCollision.collider.TryGetComponent(out IDamageable parDamageable))
      parDamageable.TakeDamage(damage);

    base.Collider(parCollision);

    Destroy(gameObject);
  }
}