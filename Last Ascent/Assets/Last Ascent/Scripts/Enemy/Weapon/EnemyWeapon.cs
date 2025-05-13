using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public abstract class EnemyWeapon : MonoBehaviour
{
  [SerializeField, Min(0)] private int damage;
  [SerializeField, Min(0)] protected float _force = 10.0f;

  [Header("Attack Settings")]
  [SerializeField, Min(0)] protected int _attackPerMinutes = 60;
  [SerializeField, Min(0)] protected float _attackSpeed = 8;

  [Header("Delay Attack")]
  [SerializeField, Min(0)] protected float _delayBeforeAttack = 1.0f;
  [SerializeField, Min(0)] protected float _delayAfterAttack = 1.0f;

  [Header("Effect")]
  [SerializeField] protected Transform _effectLaunchingAttack;

  [Header("Position & Rotation")]
  [SerializeField] private Vector3 _startPosition;
  [SerializeField] private Vector3 _startRotation;

  //--------------------------------------

  protected Enemy enemy;

  protected Coroutine attackRoutine;

  protected float lastAttackTime;

  protected bool isAttackFinished;

  //======================================

  public AttackState AttackState { get; protected set; } = AttackState.Ready;

  public bool CanAttack => AttackState == AttackState.Ready;

  public int Damage => damage;

  //======================================

  public event Action OnAttackChargeComplete;

  //======================================

  public void Initialize(Enemy parEnemy)
  {
    enemy = parEnemy;

    transform.SetLocalPositionAndRotation(_startPosition, Quaternion.Euler(_startRotation));
  }

  public void NotifyAttackComplete()
  {
    isAttackFinished = true;
  }

  //======================================

  public virtual bool TryStartAttack()
  {
    if (!CanAttack || Time.time - lastAttackTime < 60 / _attackPerMinutes)
      return false;

    attackRoutine = StartCoroutine(AttackRoutine());
    return true;
  }

  //======================================

  protected void AttackChargeComplete()
  {
    OnAttackChargeComplete?.Invoke();
  }

  //======================================

  protected virtual IEnumerator AttackRoutine()
  {
    Vector3 sizeEffectLaunchingAttack = _effectLaunchingAttack.localScale;

    AttackState = AttackState.Charging;
    isAttackFinished = false;
    _effectLaunchingAttack.localScale = Vector3.zero;

    bool chargeComplete = false;

    _effectLaunchingAttack.DOScale(sizeEffectLaunchingAttack, _delayBeforeAttack).SetEase(Ease.InQuad)
      .OnStart(() => _effectLaunchingAttack.gameObject.SetActive(true))
      .OnComplete(() => chargeComplete = true);
    
    yield return new WaitUntil(() => chargeComplete);

    AttackState = AttackState.Attacking;
    OnAttackChargeComplete?.Invoke();
  }

  //======================================
}