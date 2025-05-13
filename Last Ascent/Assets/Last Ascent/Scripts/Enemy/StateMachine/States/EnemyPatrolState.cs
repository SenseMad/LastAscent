using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{
  private IPatrolBehaviour patrolBehaviour;

  //======================================

  public override void EnterState(EnemyStateMachine parState)
  {
    patrolBehaviour ??= parState.GetComponent<IPatrolBehaviour>();

    if (patrolBehaviour == null)
    {
      //Debug.LogWarning($"{parState.name} can't patrol");
      parState.SwitchState(parState.IdleState);
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
    if (parState.Enemy.TargetDetector.IsPossibleReachPlayer())
    {
      parState.SwitchState(parState.FollowState);
      return;
    }

    if (patrolBehaviour.AtPatrolPoint())
    {
      parState.Enemy.Animator.SetFloat(EnemyAnimatorParams.SPEED, 0);
      patrolBehaviour.WaitAtPoint();
    }
    else
    {
      parState.Enemy.Animator.SetFloat(EnemyAnimatorParams.SPEED, parState.Enemy.NavMeshAgent.speed);
    }
  }

  //======================================
}