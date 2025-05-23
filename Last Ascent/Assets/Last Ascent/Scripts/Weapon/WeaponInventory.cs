using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class WeaponInventory : MonoBehaviour
{
  [SerializeField, Min(0)] private int _maxAmountStoredWeapons = 2;
  
  [SerializeField] private Transform _container;

  [SerializeField] private LayerMask _ignoreMask;

  //--------------------------------------

  private const float ResetAimAfterDelay = 2.0f;

  private Player player;

  public List<Weapon> listWeapons = new();

  //======================================

  [field: SerializeField] public Weapon ActiveWeapon { get; private set; }

  public bool IsInShootinStance { get; private set; }
  public bool IsAiming { get; private set; }

  public float LastAttackTime { get; private set; }

  //======================================

  public event Action OnListWeaponsEmpty;

  public event Action<Weapon> OnAddWeapon;
  public event Action<Weapon> OnReplaceActiveWeapon;
  public event Action<Weapon> OnWeaponChanged;

  public event Action OnAmmoChanged;

  //======================================

  private void Awake()
  {
    player = GetComponentInParent<Player>();
  }

  private void Start()
  {
    Initialize();
    SetWeaponLayer();

    player.InputHandler.InputActions.Player.Attack.started += OnAttack;
    player.InputHandler.InputActions.Player.Attack.canceled += OnAttack;

    player.InputHandler.InputActions.Player.Aiming.performed += OnAiming;

    player.InputHandler.InputActions.UI.ScrollWheel.performed += ScrollWeaponInventory;
  }

  private void OnDisable()
  {
    player.InputHandler.InputActions.Player.Attack.started -= OnAttack;
    player.InputHandler.InputActions.Player.Attack.canceled -= OnAttack;

    player.InputHandler.InputActions.Player.Aiming.performed -= OnAiming;

    player.InputHandler.InputActions.UI.ScrollWheel.performed -= ScrollWeaponInventory;

    player.Health.OnInstantlyKill -= Health_OnInstantlyKill;
  }

  private void Update()
  {
    if (player.IsPlayerUnavailable())
      return;

    Attack();

    if (IsInShootinStance && Time.time - LastAttackTime > ResetAimAfterDelay)
    {
      player.Animator.SetBool(PlayerAnimatorParams.IS_ATTACK, false);
      IsInShootinStance = false;
    }
  }

  //======================================

  public void Initialize()
  {
    if (ActiveWeapon != null)
    {
      ActiveWeapon.SetSize();
      ActiveWeapon.SetPosition();
      ActiveWeapon.SetRotation();
    }

    player.Health.OnInstantlyKill += Health_OnInstantlyKill;
  }

  public void Equip(Weapon parWeapon)
  {
    if (parWeapon == null)
    {
      Debug.Log("(Equip) No weapon found");
      return;
    }

    if (listWeapons.Count == 0)
      return;

    if (!listWeapons.Contains(parWeapon))
      return;

    if (parWeapon == ActiveWeapon)
      return;

    if (ActiveWeapon != null)
    {
      ActiveWeapon.NotEquipWeapons();
      ActiveWeapon.gameObject.SetActive(false);
    }

    ActiveWeapon = parWeapon;
    ActiveWeapon.EquipWeapons();
    ActiveWeapon.gameObject.SetActive(true);

    OnWeaponChanged?.Invoke(ActiveWeapon);

    Initialize();

    SetWeaponLayer();
  }

  public void Add(Weapon parWeapon)
  {
    if (parWeapon == null)
    {
      Debug.Log("(Add) No weapon found");
      return;
    }

    if (listWeapons.Count >= _maxAmountStoredWeapons)
    {
      ReplaceActive(parWeapon);
      return;
    }

    OnAddWeapon?.Invoke(parWeapon);

    parWeapon.transform.SetParent(_container);
    listWeapons.Add(parWeapon);
    Equip(parWeapon);
  }

  public void Remove(Weapon parWeapon)
  {
    if (parWeapon == null)
    {
      Debug.Log("(Remove) No weapon found");
      return;
    }

    if (!listWeapons.Contains(parWeapon))
      return;

    listWeapons.Remove(parWeapon);
    Destroy(parWeapon);

    ActiveWeapon = null;
    EquipLastWeapon();
  }

  public void ReplaceActive(Weapon parWeapon)
  {
    if (parWeapon == null)
    {
      Debug.LogError("(ReplaceActive) No weapon found");
      return;
    }

    listWeapons.Remove(ActiveWeapon);
    Destroy(ActiveWeapon);

    parWeapon.transform.SetParent(_container);
    listWeapons.Add(parWeapon);

    OnReplaceActiveWeapon?.Invoke(parWeapon);

    Equip(parWeapon);
  }

  public void EquipLastWeapon()
  {
    if (listWeapons.Count == 0)
    {
      OnListWeaponsEmpty?.Invoke();
      return;
    }

    Weapon lastWeapon = listWeapons[^1];
    Equip(lastWeapon);

    if (ActiveWeapon == null)
    {
      for (int i = 0; i < player.Animator.layerCount; i++)
        player.Animator.SetLayerWeight(i, 0);
    }
  }

  public void NextWeapon()
  {
    Equip(GetNextWeapon());
  }

  public void PreviousWeapon()
  {
    Equip(GetLastWeapon());
  }

  public void SetWeaponLayer()
  {
    if (ActiveWeapon == null || player.IsPlayerUnavailable())
    {
      player.SetAnimatorLayer($"{PlayerAnimatorLayers.UPPED_BODY_LAYER}", 0);
      return;
    }

    player.SetAnimatorLayer($"{PlayerAnimatorLayers.UPPED_BODY_LAYER}", 1);
  }

  //======================================

  public Weapon GetLastWeapon()
  {
    if (listWeapons.Count == 0)
      return null;

    int previousWeaponIndex = listWeapons.IndexOf(ActiveWeapon) - 1;
    if (previousWeaponIndex < 0)
      previousWeaponIndex = listWeapons.Count - 1;

    return listWeapons[previousWeaponIndex];
  }

  public Weapon GetNextWeapon()
  {
    if (listWeapons.Count == 0)
      return null;

    int nextWeaponIndex = listWeapons.IndexOf(ActiveWeapon) + 1;
    if (nextWeaponIndex > listWeapons.Count - 1)
      nextWeaponIndex = 0;

    return listWeapons[nextWeaponIndex];
  }

  //======================================

  private void Attack()
  {
    if (ActiveWeapon == null || !ActiveWeapon.IsAttack)
      return;

    Vector2 screenCenter = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
    Ray ray = player.CameraController.MainCamera.ScreenPointToRay(screenCenter);
    Vector3 targetPoint;

    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~_ignoreMask))
      targetPoint = hit.point;
    else
      targetPoint = ray.origin + ray.direction * 100f;

    player.Animator.SetBool(PlayerAnimatorParams.IS_ATTACK, true);

    player.PlayerMovement.RotateTowardsPoint(targetPoint, () =>
    {
      if (ActiveWeapon.Attack(player.gameObject))
      {
        IsInShootinStance = true;
        LastAttackTime = Time.time;

        ActiveWeapon.GetChanceCritDamage(player.LevelManager.CalculateTotalCritChance());

        if (ActiveWeapon.TryGetComponent(out WeaponRecoil parWeaponRecoil))
          player.CameraController.ApplyRecoil(parWeaponRecoil.RecoilStrength);
      }
    });
  }

  private void Health_OnInstantlyKill()
  {
    SetWeaponLayer();
  }

  //======================================

  private void OnAttack(InputAction.CallbackContext obj)
  {
    if (player.IsPlayerUnavailable())
      return;

    if (ActiveWeapon == null)
      return;

    switch (obj.phase)
    {
      case InputActionPhase.Started:
        LastAttackTime = Time.time;
        ActiveWeapon.SetAttack(true);
        break;
      case InputActionPhase.Canceled:
        ActiveWeapon.SetAttack(false);
        break;
    }
  }

  private void OnAiming(InputAction.CallbackContext obj)
  {
    if (player.IsPlayerUnavailable())
    {
      player.CameraController.MainCinemachineCamera.Lens.FieldOfView = 40;
      return;
    }

    IsAiming = !IsAiming;

    if (ActiveWeapon == null)
      IsAiming = false;

    player.CameraController.MainCinemachineCamera.Lens.FieldOfView = IsAiming ? 30 : 40;
  }

  private void ScrollWeaponInventory(InputAction.CallbackContext context)
  {
    if (player.IsPlayerUnavailable())
      return;

    float scrollValue = context.valueType.IsEquivalentTo(typeof(Vector2)) ? Mathf.Sign(context.ReadValue<Vector2>().y) : 1.0f;

    Weapon nextWeapon = scrollValue > 0 ? GetNextWeapon() : GetLastWeapon();
    Equip(nextWeapon);
  }

  //======================================
}