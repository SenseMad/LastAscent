using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedWeapon : EnemyWeapon
{
  [SerializeField] private List<Transform> _startShotPoints;

  [Header("Projectile")]
  [SerializeField] private BaseProjectile _projectilePrefab;

  //--------------------------------------

  private Vector3 direction;
  private GameObject owner;

  //======================================

  private void OnEnable()
  {
    OnAttackChargeComplete += CreateProjectile;
  }

  private void OnDisable()
  {
    OnAttackChargeComplete -= CreateProjectile;
  }

  //======================================

  private void DirectionFire(Transform parTarget, GameObject parOwner)
  {
    if (_projectilePrefab == null || _startShotPoints.Count == 0)
      return;

    Vector3 startPoint = _startShotPoints[0].position;
    Vector3 targetPosition = parTarget.position + Vector3.up;
    direction = (targetPosition - startPoint).normalized;

    owner = parOwner;
  }

  private void CreateProjectile(Vector3 parStartPoint, Vector3 parDirection, GameObject parOwner)
  {
    Quaternion rotation = Quaternion.LookRotation(parDirection);
    BaseProjectile projectile = Instantiate(_projectilePrefab, parStartPoint, rotation);

    projectile.Initialize(Damage, parOwner);
    projectile.Launch(parDirection, _attackSpeed, _force);
  }

  private void CreateProjectile()
  {
    DirectionFire(enemy.TargetDetector.NearestPlayer.transform, enemy.gameObject);

    CreateProjectile(_startShotPoints[0].position, direction, owner);
  }

  //======================================

  protected override IEnumerator AttackRoutine()
  {
    yield return base.AttackRoutine();

    _effectLaunchingAttack.gameObject.SetActive(false);
    AttackState = AttackState.Cooldown;

    yield return new WaitForSeconds(_delayAfterAttack);

    lastAttackTime = Time.time;
    AttackState = AttackState.Ready;
    attackRoutine = null;
  }

  //======================================
}