using UnityEngine;

public class TargetAttackDetector : MonoBehaviour
{
  [SerializeField, Min(0)] protected float _attackRadius = 2f;
  [SerializeField] protected LayerMask _playerMask;

  //--------------------------------------

  private Enemy enemy;

  private readonly Collider[] attackRangePlayers = new Collider[4];

  //======================================

  private void Awake()
  {
    enemy = GetComponent<Enemy>();
  }

  //======================================

  public bool IsInAttackRange(Player parPlayer)
  {
    Vector3 position = new(enemy.transform.position.x, enemy.transform.position.y + enemy.CurrentCollider.bounds.center.y, enemy.transform.position.z);
    int count = Physics.OverlapSphereNonAlloc(position, _attackRadius, attackRangePlayers, _playerMask);
    if (count <= 0)
      return false;

    for (int i = 0; i < count; i++)
    {
      Collider attackRangePlayer = attackRangePlayers[i];

      if (attackRangePlayer == null)
        continue;

      if (!attackRangePlayer.TryGetComponent(out Player parFoundPlayer))
        continue;

      if (parPlayer != parFoundPlayer)
        continue;

      return true;
    }

    return false;
  }

  //======================================

  private void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    var position = enemy != null ? new Vector3(enemy.transform.position.x, enemy.transform.position.y + enemy.CurrentCollider.bounds.center.y, enemy.transform.position.z) : transform.position;
    Gizmos.DrawWireSphere(position, _attackRadius);
  }

  //======================================
}