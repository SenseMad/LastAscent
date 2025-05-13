using UnityEngine;

public class EnemyFollowState : EnemyBaseState
{
  private IFollowBehaviour followBehaviour;

  public override void EnterState(EnemyStateMachine parState)
  {
    followBehaviour ??= parState.GetComponent<IFollowBehaviour>();
    parState.Enemy.Animator.SetLayerWeight(parState.Enemy.Animator.GetLayerIndex($"{EnemyAnimatorLayers.UPPER_BODY_LAYER}"), 0);

    if (followBehaviour == null)
    {
      if (parState.Enemy.TargetDetector.IsPossibleReachPlayer())
      {
        if (parState.Enemy.TargetAttackDetector.IsInAttackRange(parState.Enemy.NearestPlayer) && parState.Enemy.EnemyWeaponInventory.ActiveWeapon.AttackState == AttackState.Ready)
        {
          parState.SwitchState(parState.AttackState);
          return;
        }
      }

      //Debug.LogWarning($"{parState.name} can't follow the player");
      parState.SwitchState(parState.PatrolState);
      return;
    }

    parState.Enemy.NavMeshAgent.isStopped = false;
    parState.Enemy.Animator.SetFloat(EnemyAnimatorParams.SPEED, parState.Enemy.NavMeshAgent.speed);
  }

  public override void ExitState(EnemyStateMachine parState)
  {
    parState.Enemy.Animator.SetFloat(EnemyAnimatorParams.SPEED, 0);
  }

  public override void UpdateState(EnemyStateMachine parState)
  {
    if (!parState.Enemy.TargetDetector.IsPossibleReachPlayer())
    {
      parState.SwitchState(parState.PatrolState);
      return;
    }

    if (parState.Enemy.TargetAttackDetector.IsInAttackRange(parState.Enemy.NearestPlayer) && parState.Enemy.EnemyWeaponInventory.ActiveWeapon.AttackState == AttackState.Ready)
    {
      parState.SwitchState(parState.AttackState);
      return;
    }

    followBehaviour.Follow(parState.Enemy.NearestPlayer.transform);
  }

  //======================================
}