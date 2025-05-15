using UnityEngine;

public class SphereCastProjectile : BaseProjectile
{
  [SerializeField, Min(0)] private float _maxDistance = 100.0f;
  [SerializeField, Min(0)] private float _radius = 0.05f;

  [SerializeField] private LayerMask _ignoreHitMask;

  //--------------------------------------

  private float traveledDistance = 0;

  //======================================

  private void Update()
  {
    float step = _speed * Time.deltaTime;
    Vector3 move = transform.forward * step;

    if (Physics.SphereCast(transform.position, _radius, transform.forward, out RaycastHit hit, move.magnitude, ~_ignoreHitMask))
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