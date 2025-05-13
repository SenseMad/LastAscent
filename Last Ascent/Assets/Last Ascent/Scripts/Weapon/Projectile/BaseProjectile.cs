using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
  [SerializeField, Min(0)] protected float _speed = 50.0f;

  [SerializeField, Min(0)] protected float _destroyAfter = 5.0f;

  [SerializeField] private Transform _hitEffectPrefab;

  //--------------------------------------

  private int damage;

  protected GameObject owner;

  //======================================

  protected virtual void Start()
  {
    Destroy(gameObject, _destroyAfter);
  }

  //======================================

  public virtual void Initialize(int parDamage, GameObject parOwner = null)
  {
    damage = parDamage;
    owner = parOwner;
  }

  public virtual void Launch(Vector3 parDirection, float parSpeed)
  {
    _speed = parSpeed;
  }

  //======================================

  protected virtual void OnHit(Vector3 parHitPoint, Vector3 parHitNormal, Collider parOther)
  {
    CreateHitEffect(parHitPoint, parHitNormal);

    if (parOther.TryGetComponent(out IDamageable parDamageable))
      parDamageable.TakeDamage(damage);

    Destroy(gameObject);
  }

  //======================================

  private void CreateHitEffect(Vector3 parHitPoint, Vector3 parHitNormal)
  {
    if (_hitEffectPrefab == null)
      return;

    Instantiate(_hitEffectPrefab, parHitPoint, Quaternion.LookRotation(parHitNormal));
  }

  //======================================
}