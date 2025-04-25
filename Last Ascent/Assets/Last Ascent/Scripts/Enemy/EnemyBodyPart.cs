using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class EnemyBodyPart : MonoBehaviour
{
  [SerializeField] private BodyPartType _bodyPartType;

  [Space]
  [SerializeField] private List<GameObject> _listDisabledParts;

  //--------------------------------------

  private Enemy enemy;

  //======================================

  public Collider Collider { get; private set; }

  //======================================

  public event Action<BodyPartType> OnHit;

  //======================================

  private void Awake()
  {
    enemy = GetComponentInParent<Enemy>();

    Collider = GetComponent<Collider>();
  }

  //======================================

  public void TakeHit(int parBaseDamage)
  {
    int finalDamage = parBaseDamage;

    if (enemy.Health.CurrentHealth <= 0)
      return;

    switch (_bodyPartType)
    {
      case BodyPartType.Head:
        finalDamage *= 2;
        OnHit?.Invoke(BodyPartType.Head);

        if (enemy.Health.CurrentHealth - finalDamage > 0 || Random.value >= 0.3f)
          break;

        gameObject.SetActive(false);
        foreach (var disabledPart in _listDisabledParts)
        {
          if (disabledPart == null)
            continue;

          disabledPart.SetActive(false);
        }
        break;
      case BodyPartType.Torso:
        OnHit?.Invoke(BodyPartType.Torso);
        break;
      case BodyPartType.Arm:
          finalDamage /= 2;
          OnHit?.Invoke(BodyPartType.Arm);
        break;
        case BodyPartType.Leg:
          finalDamage /= 2;
          OnHit?.Invoke(BodyPartType.Leg);
        break;
    }

    enemy.Health.TakeHealth(finalDamage);
  }

  //======================================
}