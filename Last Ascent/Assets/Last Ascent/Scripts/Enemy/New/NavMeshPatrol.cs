using UnityEngine;
using UnityEngine.AI;

public class NavMeshPatrol : MonoBehaviour, IPatrolBehaviour
{
  [SerializeField] private float _patrolRadius = 5f;
  [SerializeField] private float _waitTime = 1f;

  //--------------------------------------

  private Enemy enemy;

  private float waitTimer = 0f;
  private bool waiting = false;

  //======================================

  private void Awake()
  {
    enemy = GetComponent<Enemy>();
  }

  //======================================

  public void GoToRandomPoint()
  {
    Vector3 randomDir;
    NavMeshPath path = new();

    do
    {
      randomDir = Random.insideUnitSphere * _patrolRadius;
      randomDir += transform.position;
      randomDir.y = transform.position.y;

      NavMesh.CalculatePath(transform.position, randomDir, NavMesh.AllAreas, path);
    } while (path.status != NavMeshPathStatus.PathComplete);

    enemy.NavMeshAgent.SetDestination(randomDir);
    waiting = false;
  }

  public void WaitAtPoint()
  {
    if (!waiting)
    {
      waitTimer = 0;
      waiting = true;
    }

    waitTimer += Time.deltaTime;
    if (waitTimer >= _waitTime)
      GoToRandomPoint();
  }

  //======================================

  public bool AtPatrolPoint()
  {
    return !enemy.NavMeshAgent.pathPending && enemy.NavMeshAgent.remainingDistance < 0.5f;
  }

  //======================================

  private void OnDrawGizmos()
  {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, _patrolRadius);
  }

  //======================================
}