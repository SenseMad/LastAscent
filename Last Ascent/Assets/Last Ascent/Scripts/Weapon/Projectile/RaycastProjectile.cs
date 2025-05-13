using UnityEngine;

public class RaycastProjectile : BaseProjectile
{
  [SerializeField, Min(0)] private float _maxDistance = 100.0f;

  [SerializeField] private LayerMask _hitMask;

  //--------------------------------------

  private float traveledDistance = 0;

  //======================================

  private void Update()
  {
    float step = _speed * Time.deltaTime;
    Vector3 move = transform.forward * step;

    if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, move.magnitude, ~_hitMask))
      OnHit(hit.point, hit.normal, hit.collider);
    else
    {
      transform.position += move;
      traveledDistance += step;

      if (traveledDistance >= _maxDistance)
        Destroy(gameObject);
    }
  }

  //======================================
}