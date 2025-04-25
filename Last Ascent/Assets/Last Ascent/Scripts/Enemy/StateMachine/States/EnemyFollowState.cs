using UnityEngine;

public class EnemyFollowState : EnemyBaseState
{
  public override void EnterState(EnemyStateMachine parState)
  {
    parState.Enemy.NavMeshAgent.isStopped = false;

    parState.Enemy.Animator.SetFloat(EnemyAnimatorParams.SPEED, parState.Enemy.NavMeshAgent.speed);
  }

  public override void ExitState(EnemyStateMachine parState)
  {
    parState.Enemy.Animator.SetFloat(EnemyAnimatorParams.SPEED, 0);
  }

  public override void UpdateState(EnemyStateMachine parState)
  {
    if (parState.Enemy.NearestPlayer == null || !parState.Enemy.IsPossibleReachPlayer())
    {
      parState.SwitchState(parState.PatrolState);
      return;
    }

    if (parState.Enemy.IsPlayerInAttackRange())
    {
      parState.SwitchState(parState.AttackState);
      return;
    }

    FollowPlayer(parState);
  }

  //======================================

  private void FollowPlayer(EnemyStateMachine parState)
  {
    Vector3 targetPosition = parState.Enemy.NearestPlayer.transform.position;

    parState.Enemy.NavMeshAgent.SetDestination(targetPosition);
  }

  //======================================
}