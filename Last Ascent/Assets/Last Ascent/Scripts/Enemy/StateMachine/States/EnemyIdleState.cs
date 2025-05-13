using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
  public override void EnterState(EnemyStateMachine parState) { }

  public override void ExitState(EnemyStateMachine parState) { }

  public override void UpdateState(EnemyStateMachine parState)
  {
    if (parState.Enemy.TargetDetector.IsPossibleReachPlayer())
    {
      parState.SwitchState(parState.FollowState);
      return;
    }
  }
}