using NUnit.Framework.Internal.Filters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class FirearmsWeapon : Weapon
{
  [Header("Shoot")]
  [SerializeField, Min(0)] private int _shotCount = 1;
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

  [Header("Sounds")]
  [SerializeField] private AudioClip _soundFire;

  [Header("Projectile")]
  [SerializeField] private BaseProjectile _projectilePrefab;

  [Header("Mask")]
  [SerializeField] private LayerMask _ignoreMask;

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

  public override bool Attack(GameObject parOwner)
  {
    if (IsRecharge)
      return false;

    if (CurrentAmountAmmoInMagazine == 0 && !_infitityAmmo)
      return false;

    if (!(Time.time - LastAttackTime > 60.0f / _weaponData.AttackPerMinutes))
      return false;

    DirectionFire(parOwner);

    LastAttackTime = Time.time;

    if (!_infitityAmmo)
      CurrentAmountAmmoInMagazine--;

    PlaySoundWithPitch(_soundFire, 1.0f);
    //PlaySound(_soundFire, 1.0f);

    if (_autoRecharge && CurrentAmountAmmoInMagazine == 0)
      Recharge();

    base.Attack(parOwner);

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

  private void DirectionFire(GameObject parOwner)
  {
    if (_projectilePrefab == null || _startShotPoints.Count == 0)
      return;

    Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    Ray cameraRay = Camera.main.ScreenPointToRay(screenCenter);

    Vector3 targetPoint = Physics.Raycast(cameraRay, out RaycastHit hit, 1000f, ~_ignoreMask) ? hit.point : cameraRay.GetPoint(1000f);

    foreach (var shotPoint in _startShotPoints)
    {
      Vector3 startPoint = shotPoint.position;
      Vector3 direction = (targetPoint - startPoint).normalized;

      if (_useSpread)
      {
        direction += CalculateSpread();
        direction.Normalize();
      }

      Debug.DrawLine(startPoint, targetPoint, Color.cyan, 2f);

      CreateProjectile(shotPoint, direction, parOwner);
    }

    /*Vector3 startPoint = _startShotPoints[0].position;
    Vector3 direction = (targetPoint - startPoint).normalized;

    if (_useSpread)
    {
      direction += CalculateSpread();
      direction.Normalize();
    }

    Debug.DrawLine(startPoint, targetPoint, Color.cyan, 2f);

    CreateProjectile(startPoint, direction, parOwner);*/
  }

  private void CreateProjectile(Transform parShotPoint, Vector3 parDirection, GameObject parOwner)
  {
    Quaternion rotation = Quaternion.LookRotation(parDirection);
    BaseProjectile projectile = Instantiate(_projectilePrefab, parShotPoint.position, rotation);

    int finalDamage = _weaponData.Damage;
    if (Random.value < critChance)
    {
      finalDamage *= Mathf.RoundToInt(critChance);
      //Debug.Log("КРИТ! Урон: " + finalDamage);
    }
    /*else
    {
      Debug.Log("Обычный урон: " + finalDamage);
    }*/

    projectile.CreateMuzzleParticle(parShotPoint.position, parShotPoint);
    projectile.Initialize(finalDamage, parOwner);
    projectile.Launch(parDirection, _weaponData.AttackSpeed, _weaponData.Force);
  }

  private void DebugShootRays()
  {
    Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    Ray cameraRay = Camera.main.ScreenPointToRay(screenCenter);

    if (Physics.Raycast(cameraRay, out RaycastHit hit, 1000f, ~_ignoreMask))
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

  private void PlaySoundWithPitch(AudioClip parAudioClip, float parVolume, float parMinPitch = 0.95f, float parMaxPitch = 1.05f)
  {
    if (parAudioClip == null)
      return;

    GameObject tempGameObject = new GameObject("TempAudio");
    tempGameObject.transform.position = transform.position;

    AudioSource audioSource = tempGameObject.AddComponent<AudioSource>();
    audioSource.clip = parAudioClip;
    audioSource.volume = parVolume;
    audioSource.pitch = Random.Range(parMinPitch, parMaxPitch);
    audioSource.spatialBlend = 1f;
    audioSource.dopplerLevel = 0f;
    audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

    audioSource.Play();

    Destroy(tempGameObject, parAudioClip.length / audioSource.pitch);
  }

  //======================================

  private Vector3 CalculateSpread()
  {
    return Random.insideUnitSphere * _spreadFactor;
    /*return new Vector3
    {
      x = Random.Range(-_spreadFactor, _spreadFactor),
      y = Random.Range(-_spreadFactor, _spreadFactor),
      z = Random.Range(-_spreadFactor, _spreadFactor)
    };*/
  }

  //======================================
}