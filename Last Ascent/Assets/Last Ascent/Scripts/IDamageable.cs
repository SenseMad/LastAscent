using UnityEngine;

public interface IDamageable
{
  void TakeDamage(int parDamage, Vector3 parForce, Vector3 parHitPoint);
}