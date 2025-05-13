using UnityEngine;
using UnityEngine.AI;

public class TargetDetector : MonoBehaviour, ITargetDetector
{
  [SerializeField, Min(0)] private float _detectionRange = 50.0f;

  [SerializeField] private LayerMask _playerLayer;

  //--------------------------------------

  private Enemy enemy;

  private readonly Collider[] detectedPlayers = new Collider[4];

  private NavMeshPath navMeshPath;

  //======================================

  public Player NearestPlayer { get; private set; }

  //======================================

  private void Awake()
  {
    enemy = GetComponent<Enemy>();

    navMeshPath = new();
  }

  private void Update()
  {
    TickDetection();
  }

  //======================================

  public void TickDetection()
  {
    int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _detectionRange, detectedPlayers, _playerLayer);
    if (hitCount <= 0)
      return;

    Player nearestPlayer = null;
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

      NavMesh.CalculatePath(transform.position, parPlayer.transform.position, NavMesh.AllAreas, navMeshPath);

      if (navMeshPath.status != NavMeshPathStatus.PathComplete)
        continue;

      float distanceToPlayer = CalculateDistanceToPlayer();

      if (distanceToPlayer < nearestDistance)
      {
        nearestDistance = distanceToPlayer;
        nearestPlayer = parPlayer;
      }
    }

    NearestPlayer = nearestPlayer;
  }

  //======================================

  public bool IsPossibleReachPlayer()
  {
    return NearestPlayer != null && IsPossibleReachPlayer(NearestPlayer.transform);
  }

  public bool IsPossibleReachPlayer(Transform parNearestTarget)
  {
    if (parNearestTarget == null)
    {
      navMeshPath = null;
      return false;
    }

    enemy.NavMeshAgent.CalculatePath(parNearestTarget.position, navMeshPath);

    return navMeshPath.status == NavMeshPathStatus.PathComplete;
  }

  //======================================

  private float CalculateDistanceToPlayer()
  {
    if (navMeshPath.corners.Length < 2)
      return 0;

    float retDistance = 0;

    for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
      retDistance += Vector3.Distance(navMeshPath.corners[i], navMeshPath.corners[i + 1]);

    return retDistance;
  }

  //======================================

  private void OnDrawGizmos()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, _detectionRange);
  }

  //======================================
}