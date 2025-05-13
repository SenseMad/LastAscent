using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
  [SerializeField, Min(0)] protected float _speed = 50.0f;
  [SerializeField, Min(0)] protected float _force = 10.0f;

  [SerializeField, Min(0)] protected float _destroyAfter = 5.0f;

  [SerializeField] private Transform _hitEffectPrefab;

  //--------------------------------------

  protected Collider currentCollider;

  private int damage;

  protected GameObject owner;

  //======================================

  protected virtual void Awake()
  {
    currentCollider = GetComponent<Collider>();
  }

  protected virtual void Start()
  {
    Destroy(gameObject, _destroyAfter);
  }

  //======================================

  public virtual void Initialize(int parDamage, GameObject parOwner = null)
  {
    damage = parDamage;
    owner = parOwner;

    //IgnoreCollision();
  }

  public virtual void Launch(Vector3 parDirection, float parSpeed, float parForce)
  {
    _speed = parSpeed;
    _force = parForce;
  }

  //======================================

  protected virtual void OnHit(Vector3 parHitPoint, Vector3 parHitNormal, Collider parOther)
  {
    CreateHitEffect(parHitPoint, parHitNormal);

    if (parOther.TryGetComponent(out Hitbox parHitbox))
      parHitbox.ApplyDamage(damage, parHitPoint * _force, parHitNormal);
    /*else if (parOther.TryGetComponent(out IDamageable parDamageable))
      parDamageable.TakeDamage(damage, parHitPoint * _force, parHitNormal);*/

    Destroy(gameObject);
  }

  //======================================

  private void CreateHitEffect(Vector3 parHitPoint, Vector3 parHitNormal)
  {
    if (_hitEffectPrefab == null)
      return;

    Instantiate(_hitEffectPrefab, parHitPoint, Quaternion.LookRotation(parHitNormal));
  }

  private void IgnoreCollision()
  {
    if (owner != null)
    {
      var ownerColliders = owner.GetComponentsInChildren<Collider>();

      foreach (var ownerCollider in ownerColliders)
      {
        if (ownerCollider == null)
          continue;

        Physics.IgnoreCollision(ownerCollider, currentCollider, true);
      }
    }
  }

  //======================================
}