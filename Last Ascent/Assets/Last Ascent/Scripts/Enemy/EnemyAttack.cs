using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
  [SerializeField, Min(0)] private int _damage = 1;

  [SerializeField, Min(0)] private int _attacksPerMinute = 15;

  [SerializeField] private Transform _attackPoint;

  //--------------------------------------

  private Enemy enemy;

  private float lastAttackTime;

  private bool isStartAttack = false;
  private bool wasAnAttack = false;
  private bool youCanAttack = true;

  //======================================

  public Transform AttackPoint => _attackPoint;

  //======================================

  private void Awake()
  {
    enemy = GetComponent<Enemy>();
  }

  private void Update()
  {
    SetAttackAnimatorLayer();
  }

  //======================================

  public bool Attack()
  {
    if (!(Time.time - lastAttackTime > 60.0f / _attacksPerMinute) && !youCanAttack)
      return false;

    lastAttackTime = Time.time;
    enemy.Animator.SetTrigger($"{EnemyAnimatorParams.IS_ATTACK}");

    isStartAttack = true;
    youCanAttack = false;

    return true;
  }

  public void SetAttackAnimatorLayer()
  {
    if (isStartAttack)
      return;

    //enemy.Animator.SetLayerWeight(enemy.Animator.GetLayerIndex($"{EnemyAnimatorLayers.UPPER_BODY_LAYER}"), enemy.IsAttackRange ? 1 : 0);
  }

  //======================================

  private void OnDealDamage(AnimationEvent animationEvent)
  {
    if (wasAnAttack)
      return;

    /*if (!enemy.IsPlayerInAttackRange())
      return;*/

    if (!enemy.NearestPlayer.TryGetComponent(out IDamageable parDamageable))
      return;

    parDamageable.TakeDamage(_damage, Vector3.zero, Vector3.zero);

    wasAnAttack = true;
  }

  private void OnYouCanAttack(AnimationEvent animationEvent)
  {
    youCanAttack = true;
    wasAnAttack = false;
    isStartAttack = false;
  }

  //======================================
}