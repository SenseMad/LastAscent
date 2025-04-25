using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
  public override void EnterState(EnemyStateMachine parState)
  {
    parState.Enemy.NavMeshAgent.isStopped = true;
  }

  public override void ExitState(EnemyStateMachine parState) { }

  public override void UpdateState(EnemyStateMachine parState)
  {
    if (!parState.Enemy.IsPlayerInAttackRange())
    {
      parState.SwitchState(parState.FollowState);
      return;
    }

    RotationNearestPlayer(parState);

    parState.Enemy.EnemyAttack.Attack();
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