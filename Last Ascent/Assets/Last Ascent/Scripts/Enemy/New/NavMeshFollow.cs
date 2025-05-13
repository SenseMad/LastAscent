using UnityEngine;

public class NavMeshFollow : MonoBehaviour, IFollowBehaviour
{
  private Enemy enemy;

  //======================================

  private void Awake()
  {
    enemy = GetComponent<Enemy>();
  }

  //======================================

  public void Follow(Transform parTarget)
  {
    if (parTarget == null || !enemy.NavMeshAgent.enabled)
      return;

    enemy.NavMeshAgent.SetDestination(parTarget.position);
  }

  //======================================
}