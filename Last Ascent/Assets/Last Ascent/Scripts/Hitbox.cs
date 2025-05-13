using UnityEngine;

public class Hitbox : MonoBehaviour
{
  [SerializeField, Min(0)] private float _damageMultiplier = 1.0f;

  //--------------------------------------

  private IDamageable damageable;

  //======================================

  private void Awake()
  {
    damageable = GetComponentInParent<IDamageable>();
  }

  //======================================

  public void ApplyDamage(int parBaseDamage, Vector3 parHitPoint, Vector3 parHitForce)
  {
    int finalDamage = Mathf.RoundToInt(parBaseDamage * _damageMultiplier);

    damageable.TakeDamage(finalDamage, parHitPoint, parHitForce);
  }

  //======================================
}