using UnityEngine;

public sealed class EnemyStateMachine : MonoBehaviour
{
  private EnemyBaseState currentState;

  //======================================

  public Enemy Enemy { get; private set; }

  public EnemyIdleState IdleState { get; private set; }
  public EnemyFollowState FollowState { get; private set; }
  public EnemyPatrolState PatrolState { get; private set; }
  public EnemyAttackState AttackState { get; private set; }
  public EnemyDeadState DeadState { get; private set; }

  //======================================

  private void Awake()
  {
    Enemy = GetComponent<Enemy>();

    IdleState = new();
    FollowState = new();
    PatrolState = new();
    AttackState = new();
    DeadState = new();
  }

  private void Start()
  {
    SwitchState(FollowState);
  }

  private void Update()
  {
    currentState?.UpdateState(this);
  }

  //======================================

  public void SwitchState(EnemyBaseState parState)
  {
    currentState?.ExitState(this);
    currentState = parState;
    currentState?.EnterState(this);
  }

  //======================================
}