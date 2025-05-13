using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IDamageable
{
  [SerializeField] private EnemyType _enemyType;

  [Header("Specifications")]
  [SerializeField, Min(0)] private float _rotationSpeed = 10.0f;

  [Header("Effect")]
  [SerializeField] private GameObject _spawnEffectPrefab;

  //--------------------------------------

  private RagdollHandler ragdollHandler;

  //======================================

  public float RotationSpeed => _rotationSpeed;

  public EnemyStateMachine EnemyStateMachine { get; private set; }

  public NavMeshAgent NavMeshAgent { get; private set; }
  public NavMeshPath NavMeshPath { get; private set; }

  public Animator Animator { get; private set; }

  public EnemyType EnemyType => _enemyType;

  public GameObject SpawnEffectPrefab => _spawnEffectPrefab;

  public Health Health { get; private set; }

  public List<EnemyBodyPart> EnemyBodyParts { get; private set; }

  public Collider CurrentCollider { get; private set; }

  public TargetDetector TargetDetector { get; private set; }
  public TargetAttackDetector TargetAttackDetector { get; private set; }
  public EnemyWeaponInventory EnemyWeaponInventory { get; private set; }

  public Player NearestPlayer => TargetDetector?.NearestPlayer;

  //======================================

  private void Awake()
  {
    ragdollHandler = GetComponent<RagdollHandler>();
    ragdollHandler.Initialize();

    EnemyStateMachine = transform.AddComponent<EnemyStateMachine>();

    NavMeshAgent = GetComponent<NavMeshAgent>();

    CurrentCollider = GetComponentInChildren<Collider>();

    Animator = GetComponent<Animator>();

    EnemyBodyParts = GetComponentsInChildren<EnemyBodyPart>().ToList();

    InitializingModules();

    TargetAttackDetector = GetComponent<TargetAttackDetector>();
    EnemyWeaponInventory = GetComponentInChildren<EnemyWeaponInventory>();

    //uiHealthBar = GetComponent<UIHealthBar>();
  }

  private void Start()
  {
    NavMeshPath = new();
  }

  private void OnDestroy()
  {
    Health.OnInstantlyKill -= Health_OnInstantlyKill;
  }

  private void Update()
  {
    //SetAttackAnimatorLayer();
  }

  //======================================

  public void Initialize() { }

  public void InitializingModules()
  {
    TargetDetector = GetComponent<TargetDetector>();

    Animator.SetLayerWeight(Animator.GetLayerIndex($"{EnemyAnimatorLayers.UPPER_BODY_LAYER}"), 1);
  }

  public void HealthInitialize(int parMaxHealth)
  {
    Health = new Health();
    Health.SetMaxHealth(parMaxHealth);
    Health.Initialize();

    Health.OnInstantlyKill += Health_OnInstantlyKill;
  }

  public void TakeDamage(int parDamage, Vector3 parForce, Vector3 parHitPoint)
  {
    if (Health == null)
      return;

    Health.TakeHealth(parDamage);

    if (Health.CurrentHealth <= 0)
    {
      Animator.enabled = false;
      ragdollHandler.Enable();
      ragdollHandler.Hit(parForce, parHitPoint);
    }
  }

  /*public void SetAttackAnimatorLayer()
  {
    Animator.SetLayerWeight(Animator.GetLayerIndex($"{EnemyAnimatorLayers.UPPER_BODY_LAYER}"), IsAttackRange ? 1 : 0);
  }*/

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

    Animator.enabled = false;
    ragdollHandler.Enable();

    //Animator.SetTrigger($"{EnemyAnimatorParams.IS_DIE}");

    /*foreach (var enemyBodyPart in EnemyBodyParts)
    {
      if (enemyBodyPart == null)
        continue;

      enemyBodyPart.Collider.enabled = false;
    }*/
    CurrentCollider.enabled = false;

    for (int i = 0; i < Animator.layerCount; i++)
      Animator.SetLayerWeight(i, 0);

    Destroy(gameObject, 3.0f);
  }

  //======================================

  private void OnDealDamage(AnimationEvent animationEvent)
  {
    if (!TargetAttackDetector.IsInAttackRange(TargetDetector.NearestPlayer))
      return;

    if (!TargetDetector.NearestPlayer.TryGetComponent(out IDamageable parDamageable))
      return;

    parDamageable.TakeDamage(EnemyWeaponInventory.ActiveWeapon.Damage, Vector3.zero, Vector3.zero);
    //Debug.Log("Урон нанесен");
  }

  private void OnAttackIsComplete(AnimationEvent animationEvent)
  {
    EnemyWeaponInventory.ActiveWeapon?.NotifyAttackComplete();
  }

  //======================================
}