public abstract class EnemyBaseState
{
  public abstract void EnterState(EnemyStateMachine parState);

  public abstract void UpdateState(EnemyStateMachine parState);

  public abstract void ExitState(EnemyStateMachine parState);
}