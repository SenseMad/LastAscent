using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class FirearmsWeapon : Weapon
{
  [Header("Shoot")]
  [SerializeField, Min(0)] private int _shotCount = 1;
  [SerializeField, Min(0)] private int _shotPerMinutes = 200;
  [SerializeField, Min(0)] private float _shotSpeed = 8;
  [SerializeField] private List<Transform> _startShotPoints;

  [Header("Spread")]
  [SerializeField] private bool _useSpread = true;
  [SerializeField, Min(0)] private float _spreadFactor = 1.0f;

  [Header("Ammo")]
  [SerializeField] private bool _infitityAmmo = false;
  [SerializeField, Min(0)] private int _maxAmountAmmo = 0;
  [SerializeField, Min(0)] private int _maxAmountAmmoInMagazine = 0;

  [Header("Recharge")]
  [SerializeField] private bool _autoRecharge = false;
  [SerializeField, Min(0)] private float _rechargeTime = 1.0f;

  [Header("Effect")]
  [SerializeField] private Transform _muzzleEffectPrefab;
  [SerializeField, Min(0)] private float _hitEffectDestroyDelay = 2.0f;

  [Header("Sounds")]
  [SerializeField] private AudioClip _soundFire;

  [Header("Projectile")]
  [SerializeField] private BaseProjectile _projectilePrefab;

  //--------------------------------------

  private Coroutine coroutineRecharge;

  //======================================

  private void Start()
  {
    CurrentAmountAmmo = _maxAmountAmmo;
    CurrentAmountAmmoInMagazine = _maxAmountAmmoInMagazine;
  }

  private void Update()
  {
#if UNITY_EDITOR
    DebugShootRays();
#endif
  }

  //======================================

  public override bool Attack()
  {
    if (IsRecharge)
      return false;

    if (CurrentAmountAmmoInMagazine == 0 && !_infitityAmmo)
      return false;

    if (!(Time.time - LastAttackTime > 60.0f / _shotPerMinutes))
      return false;

    DirectionFire();
    MuzzlePerformEffects();

    LastAttackTime = Time.time;

    if (!_infitityAmmo)
      CurrentAmountAmmoInMagazine--;

    PlaySound(_soundFire, 1.0f);

    if (_autoRecharge && CurrentAmountAmmoInMagazine == 0)
      Recharge();

    base.Attack();

    return true;
  }

  public override void Recharge()
  {
    Recharge(_maxAmountAmmoInMagazine);
  }

  public override void EquipWeapons() { }
  public override void NotEquipWeapons()
  {
    if (coroutineRecharge != null)
    {
      StopCoroutine(coroutineRecharge);

      IsRecharge = false;
      coroutineRecharge = null;
    }
  }

  //======================================

  private void DirectionFire()
  {
    if (_projectilePrefab == null || _startShotPoints.Count == 0)
      return;

    Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    Ray cameraRay = Camera.main.ScreenPointToRay(screenCenter);

    Vector3 targetPoint = Physics.Raycast(cameraRay, out RaycastHit hit, 1000f) ? hit.point : cameraRay.GetPoint(1000f);
    Vector3 startPoint = _startShotPoints[0].position;
    Vector3 direction = (targetPoint - startPoint).normalized;
    Debug.DrawLine(startPoint, targetPoint, Color.cyan, 2f);

    CreateProjectile(startPoint, direction);
  }

  private void CreateProjectile(Vector3 parStartPoint, Vector3 parDirection)
  {
    Quaternion rotation = Quaternion.LookRotation(parDirection);
    BaseProjectile projectile = Instantiate(_projectilePrefab, parStartPoint, rotation);

    projectile.Initialize(_damage);

    projectile.Rigidbody.linearVelocity = parDirection * _shotSpeed;
  }

  private void DebugShootRays()
  {
    Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    Ray cameraRay = Camera.main.ScreenPointToRay(screenCenter);

    if (Physics.Raycast(cameraRay, out RaycastHit hit, 1000f))
    {
      Debug.DrawLine(cameraRay.origin, hit.point, Color.red, 0.01f);

      if (_startShotPoints.Count > 0)
      {
        Vector3 startPoint = _startShotPoints[0].position;
        Vector3 direction = (hit.point - startPoint).normalized;

        Debug.DrawLine(startPoint, hit.point, Color.green, 0.01f);
      }
    }
    else
    {
      Vector3 fallbackPoint = cameraRay.GetPoint(1000f);

      Debug.DrawLine(cameraRay.origin, fallbackPoint, Color.red, 0.01f);

      if (_startShotPoints.Count > 0)
      {
        Vector3 startPoint = _startShotPoints[0].position;
        Vector3 direction = (fallbackPoint - startPoint).normalized;

        Debug.DrawLine(startPoint, fallbackPoint, Color.green, 0.01f);
      }
    }
  }

  #region Recharge

  private void Recharge(int parValue)
  {
    if (coroutineRecharge != null)
    {
      Debug.LogWarning("The recharge is already running");
      return;
    }

    if (parValue < 0)
    {
      Debug.LogError("The recharge value cannot be less than 0");
      return;
    }

    if (CurrentAmountAmmo == 0)
    {
      Debug.LogWarning("The current number of rounds is 0, reloading is not possible");
      return;
    }

    if (CurrentAmountAmmoInMagazine >= _maxAmountAmmoInMagazine)
    {
      Debug.Log("The current number of rounds in the magazine >= the maximum value");
      return;
    }

    coroutineRecharge = StartCoroutine(CoroutineRecharge(parValue));
  }

  private IEnumerator CoroutineRecharge(int parValue)
  {
    IsRecharge = true;

    int amountAmmoBefore = CurrentAmountAmmo;
    int amountAmmoInMagazineBefore = CurrentAmountAmmoInMagazine;

    if (parValue + amountAmmoInMagazineBefore > _maxAmountAmmoInMagazine)
      parValue = _maxAmountAmmoInMagazine - amountAmmoInMagazineBefore;

    if (amountAmmoBefore - parValue <= 0)
      parValue = amountAmmoBefore;

    //PlaySound(_soundRecharge, 0.3f);

    yield return new WaitForSeconds(_rechargeTime);

    /*while (IsRecharge)
      yield return null;*/

    CurrentAmountAmmo -= parValue;
    CurrentAmountAmmoInMagazine += parValue;

    coroutineRecharge = null;
    IsRecharge = false;
  }

  #endregion

  private void MuzzlePerformEffects()
  {
    if (_muzzleEffectPrefab == null)
      return;

    foreach (var startShotPoint in _startShotPoints)
      Instantiate(_muzzleEffectPrefab, startShotPoint);
  }

  private void PlaySound(AudioClip parAudioClip, float parVolume)
  {
    if (parAudioClip == null)
      return;

    AudioSource.PlayClipAtPoint(parAudioClip, transform.position, parVolume);
  }

  //======================================

  private Vector3 CalculateSpread()
  {
    return new Vector3
    {
      x = Random.Range(-_spreadFactor, _spreadFactor),
      y = Random.Range(-_spreadFactor, _spreadFactor),
      z = Random.Range(-_spreadFactor, _spreadFactor)
    };
  }

  //======================================
}