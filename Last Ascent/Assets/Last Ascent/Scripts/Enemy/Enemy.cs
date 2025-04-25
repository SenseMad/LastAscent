using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IDamageable
{
  [SerializeField] private EnemyType _enemyType;

  [Header("Health")]
  [SerializeField] private Health _health;

  [Header("Specifications")]
  [SerializeField, Min(0)] private float _rotationSpeed = 10.0f;
  [SerializeField, Min(0)] private float _detectionRange = 5.0f;

  [SerializeField, Min(0)] private float _attackRadius = 2.0f;
  [SerializeField, Min(0)] private float _patrolRadius = 5.0f;

  [Header("Mask")]
  [SerializeField] private LayerMask _playerLayer;

  [Space]
  [SerializeField] private GameObject _enemySkin;

  //--------------------------------------

  private readonly Collider[] detectedPlayers = new Collider[4];
  private readonly Collider[] attackRangePlayers = new Collider[4];

  private Collider currentCollider;

  //======================================

  public float RotationSpeed => _rotationSpeed;

  public EnemyStateMachine EnemyStateMachine { get; private set; }
  public EnemyAttack EnemyAttack { get; private set; }

  public NavMeshAgent NavMeshAgent { get; private set; }
  public NavMeshPath NavMeshPath { get; private set; }

  public Animator Animator { get; private set; }

  public Player NearestPlayer { get; private set; }

  public EnemyType EnemyType => _enemyType;

  public Health Health { get; private set; }

  public float AttackRadius => _attackRadius;
  public float PatrolRadius => _patrolRadius;

  public bool IsAttackRange { get; private set; }

  public List<EnemyBodyPart> EnemyBodyParts { get; private set; }

  //======================================

  private void Awake()
  {
    EnemyStateMachine = GetComponent<EnemyStateMachine>();
    EnemyAttack = GetComponent<EnemyAttack>();

    NavMeshAgent = GetComponent<NavMeshAgent>();

    currentCollider = GetComponent<Collider>();

    Animator = GetComponent<Animator>();

    EnemyBodyParts = GetComponentsInChildren<EnemyBodyPart>().ToList();

    //uiHealthBar = GetComponent<UIHealthBar>();
  }

  private void Start()
  {
    NavMeshPath = new();
  }

  private void OnDestroy()
  {
    if (Health != null)
      Health.OnInstantlyKill -= Health_OnInstantlyKill;
  }

  private void Update()
  {
    NearestPlayer = DetectNearestPlayer();

    //SetAttackAnimatorLayer();
  }

  //======================================

  public void Initialize()
  {

  }

  public void HealthInitialize(int parMaxHealth)
  {
    Health = new Health();
    Health.SetMaxHealth(parMaxHealth);
    Health.Initialize();

    Health.OnInstantlyKill += Health_OnInstantlyKill;
  }

  public void TakeDamage(int parDamage)
  {
    if (Health == null)
      return;

    Health.TakeHealth(parDamage);
  }

  /*public void SetAttackAnimatorLayer()
  {
    Animator.SetLayerWeight(Animator.GetLayerIndex($"{EnemyAnimatorLayers.UPPER_BODY_LAYER}"), IsAttackRange ? 1 : 0);
  }*/

  //======================================

  public bool IsPossibleReachPlayer()
  {
    return IsPossibleReachPlayer(NearestPlayer.transform);
  }

  public bool IsPossibleReachPlayer(Transform parNearestTarget)
  {
    if (parNearestTarget == null)
    {
      NavMeshPath = null;
      return false;
    }

    NavMeshAgent.CalculatePath(parNearestTarget.position, NavMeshPath);

    return NavMeshPath.status == NavMeshPathStatus.PathComplete;
  }

  public bool IsPlayerInAttackRange()
  {
    IsAttackRange = false;

    if (NearestPlayer == null)
      return false;

    int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _attackRadius, attackRangePlayers, _playerLayer);
    if (hitCount <= 0)
      return false;

    for (int i = 0; i < hitCount; i++)
    {
      Collider detectPlayer = detectedPlayers[i];

      if (detectPlayer == null)
        continue;

      if (!detectPlayer.TryGetComponent(out Player parPlayer))
        continue;

      if (parPlayer != NearestPlayer)
        continue;

      IsAttackRange = true;

      return true;
    }

    return false;
  }

  //======================================

  private void Health_OnInstantlyKill()
  {
    if (!NavMeshAgent.enabled && Health.IsDead)
      return;

    EnemyStateMachine.SwitchState(EnemyStateMachine.DeadState);

    if (NavMeshAgent.isOnNavMesh)
    {
      NavMeshAgent.isStopped = true;
      NavMeshAgent.enabled = false;
    }

    Animator.SetTrigger($"{EnemyAnimatorParams.IS_DIE}");

    foreach (var enemyBodyPart in EnemyBodyParts)
    {
      if (enemyBodyPart == null)
        continue;

      enemyBodyPart.Collider.enabled = false;
    }
    currentCollider.enabled = false;

    for (int i = 0; i < Animator.layerCount; i++)
      Animator.SetLayerWeight(i, 0);

    Destroy(gameObject, 3.0f);
  }

  //======================================

  private Player DetectNearestPlayer()
  {
    int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _detectionRange, detectedPlayers, _playerLayer);
    if (hitCount <= 0)
      return null;

    Player retNearestPlayer = null;

    float nearestDistance = Mathf.Infinity;

    for (int i = 0; i < hitCount; i++)
    {
      Collider detectPlayer = detectedPlayers[i];

      if (detectPlayer == null)
        continue;

      if (!detectPlayer.TryGetComponent(out Player parPlayer))
        continue;

      if (parPlayer.IsPlayerUnavailable())
        continue;

      NavMesh.CalculatePath(transform.position, parPlayer.transform.position, NavMesh.AllAreas, NavMeshPath);

      if (NavMeshPath.status != NavMeshPathStatus.PathComplete)
        continue;

      float distanceToPlayer = CalculateDistanceToPlayer();

      if (distanceToPlayer < nearestDistance)
      {
        nearestDistance = distanceToPlayer;
        retNearestPlayer = parPlayer;
      }
    }

    return retNearestPlayer;
  }

  private float CalculateDistanceToPlayer()
  {
    if (NavMeshPath.corners.Length < 2)
      return 0;

    float retDistance = 0;

    for (int i = 0; i < NavMeshPath.corners.Length - 1; i++)
      retDistance += Vector3.Distance(NavMeshPath.corners[i], NavMeshPath.corners[i + 1]);

    return retDistance;
  }

  //======================================

  private void OnDrawGizmos()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, _detectionRange);

    Gizmos.color = Color.red;
    var attackPosition = EnemyAttack != null && EnemyAttack.AttackPoint != null ? EnemyAttack.AttackPoint.position : transform.position;
    Gizmos.DrawWireSphere(attackPosition, _attackRadius);

    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, _patrolRadius);
  }

  //======================================
}