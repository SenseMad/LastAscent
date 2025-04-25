using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseProjectile : MonoBehaviour
{
  [SerializeField] protected float _destroyAfter = 5.0f;

  [SerializeField] private Transform _hitEffectPrefab;

  //--------------------------------------

  protected int damage;

  //======================================

  public Rigidbody Rigidbody { get; private set; }

  //======================================

  private void Awake()
  {
    Rigidbody = GetComponent<Rigidbody>();
  }

  private void Start()
  {
    Destroy();
  }

  //======================================

  public virtual void Initialize(int parDamage)
  {
    damage = parDamage;
  }

  //======================================

  protected virtual void Collider(Collision parCollision)
  {
    CreateHitEffect(parCollision);
  }

  protected void Destroy()
  {
    Destroy(gameObject, _destroyAfter);
  }

  //======================================

  private void CreateHitEffect(Collision parCollision)
  {
    if (_hitEffectPrefab == null)
      return;

    Instantiate(_hitEffectPrefab, transform.position, Quaternion.LookRotation(parCollision.contacts[0].normal));
  }

  //======================================

  private void OnCollisionEnter(Collision parCollision)
  {
    Collider(parCollision);
  }

  //======================================
}