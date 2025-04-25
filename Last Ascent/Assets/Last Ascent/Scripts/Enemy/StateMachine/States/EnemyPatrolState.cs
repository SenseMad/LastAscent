using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyBaseState
{
  private bool endPatrolPoint = false;
  private float tempTimeEndPatrolPoint = 0;
  private float timeDelayAfterPatrolling = 0;

  //======================================

  public override void EnterState(EnemyStateMachine parState)
  {
    parState.Enemy.NavMeshAgent.isStopped = false;

    endPatrolPoint = false;
    tempTimeEndPatrolPoint = 0;
    timeDelayAfterPatrolling = 0;

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
      Patrolling(parState);
      return;
    }

    parState.SwitchState(parState.FollowState);
  }

  //======================================

  private void Patrolling(EnemyStateMachine parState)
  {
    if (endPatrolPoint)
    {
      tempTimeEndPatrolPoint += Time.deltaTime;
      if (tempTimeEndPatrolPoint >= timeDelayAfterPatrolling)
      {
        tempTimeEndPatrolPoint = 0;
        endPatrolPoint = false;

        parState.Enemy.Animator.SetFloat(EnemyAnimatorParams.SPEED, parState.Enemy.NavMeshAgent.speed);

        RandomPatrolDirection(parState);
      }

      return;
    }

    if (!parState.Enemy.NavMeshAgent.pathPending && parState.Enemy.NavMeshAgent.remainingDistance < 0.5f && !endPatrolPoint)
    {
      endPatrolPoint = true;

      parState.Enemy.Animator.SetFloat(EnemyAnimatorParams.SPEED, 0);

      timeDelayAfterPatrolling = Random.Range(0.1f, 1.5f);
    }
  }

  private void RandomPatrolDirection(EnemyStateMachine parState)
  {
    Vector3 randomDirection;

    do
    {
      randomDirection = Random.insideUnitSphere * parState.Enemy.PatrolRadius;
      randomDirection += parState.Enemy.transform.position;
      randomDirection.y = parState.Enemy.transform.position.y;

      NavMesh.CalculatePath(parState.Enemy.transform.position, randomDirection, NavMesh.AllAreas, parState.Enemy.NavMeshPath);
    } while (parState.Enemy.NavMeshPath.status != NavMeshPathStatus.PathComplete);

    parState.Enemy.NavMeshAgent.SetDestination(randomDirection);
  }

  //======================================
}