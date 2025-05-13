using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
  public override void EnterState(EnemyStateMachine parState)
  {
    parState.Enemy.NavMeshAgent.isStopped = true;

    parState.Enemy.Animator.SetLayerWeight(parState.Enemy.Animator.GetLayerIndex($"{EnemyAnimatorLayers.UPPER_BODY_LAYER}"), 1);
  }

  public override void ExitState(EnemyStateMachine parState)
  {
    parState.Enemy.NavMeshAgent.isStopped = false;

    parState.Enemy.Animator.SetLayerWeight(parState.Enemy.Animator.GetLayerIndex($"{EnemyAnimatorLayers.UPPER_BODY_LAYER}"), 0);
  }

  public override void UpdateState(EnemyStateMachine parState)
  {
    var enemy = parState.Enemy;
    var weapon = enemy.EnemyWeaponInventory.ActiveWeapon;

    if (weapon == null)
    {
      parState.SwitchState(parState.FollowState);
      return;
    }

    if (weapon.AttackState == AttackState.Ready)
    {
      if (enemy.NearestPlayer == null || !enemy.TargetAttackDetector.IsInAttackRange(parState.Enemy.NearestPlayer))
      {
        parState.SwitchState(parState.FollowState);
        return;
      }

      weapon.TryStartAttack();
    }

    RotationNearestPlayer(parState);
  }

  //======================================

  private void RotationNearestPlayer(EnemyStateMachine parState)
  {
    if (parState.Enemy.NearestPlayer == null)
      return;

    Vector3 direction = parState.Enemy.NearestPlayer.transform.position - parState.Enemy.transform.position;
    Quaternion lookRotation = Quaternion.LookRotation(direction);
    Vector3 rotation = Quaternion.Lerp(parState.Enemy.transform.rotation, lookRotation, parState.Enemy.RotationSpeed * Time.deltaTime).eulerAngles;

    parState.Enemy.transform.rotation = Quaternion.Euler(0, rotation.y, 0);
  }

  //======================================
}