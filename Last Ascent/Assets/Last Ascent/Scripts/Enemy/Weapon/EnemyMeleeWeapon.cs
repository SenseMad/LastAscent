using System.Collections;
using UnityEngine;

public class EnemyMeleeWeapon : EnemyWeapon
{
  protected override IEnumerator AttackRoutine()
  {
    yield return base.AttackRoutine();

    enemy.Animator.SetTrigger(EnemyAnimatorParams.IS_ATTACK);

    yield return new WaitUntil(() => isAttackFinished);

    _effectLaunchingAttack.gameObject.SetActive(false);
    AttackState = AttackState.Cooldown;

    yield return new WaitForSeconds(_delayAfterAttack);

    lastAttackTime = Time.time;
    AttackState = AttackState.Ready;
    attackRoutine = null;
  }
}