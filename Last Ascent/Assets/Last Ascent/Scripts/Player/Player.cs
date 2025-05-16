using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public sealed class Player : MonoBehaviour, IDamageable
{
  private GameObject currentSkinObject;

  //======================================

  public PlayerMovement PlayerMovement { get; private set; }

  public InputHandler InputHandler { get; private set; }

  public CharacterController CharacterController { get; private set; }

  public CameraController CameraController { get; private set; }

  public WeaponInventory WeaponInventory { get; private set; }

  public UpgradeManager UpgradeManager { get; private set; }

  public Animator Animator { get; private set; }

  public PlayerSkinData PlayerSkinData { get; private set; }

  public Health Health { get; private set; }

  public bool IsRunning { get; private set; }
  public bool IsAiming { get; private set; }

  //======================================

  private void Awake()
  {
    PlayerMovement = GetComponent<PlayerMovement>();

    InputHandler = gameObject.AddComponent<InputHandler>();

    CharacterController = GetComponent<CharacterController>();
    CharacterController.enabled = false;

    CameraController = GetComponentInChildren<CameraController>();

    WeaponInventory = GetComponentInChildren<WeaponInventory>();

    Animator = GetComponent<Animator>();

    Initialize();
  }

  private void OnEnable()
  {
    InputHandler.InputActions.Player.Sprint.started += OnRun;
    InputHandler.InputActions.Player.Sprint.canceled += OnRun;
  }

  private void OnDisable()
  {
    InputHandler.InputActions.Player.Sprint.started -= OnRun;
    InputHandler.InputActions.Player.Sprint.canceled -= OnRun;
  }

  private void OnDestroy()
  {
    Health.OnInstantlyKill -= Health_OnInstantlyKill;
  }

  //======================================

  public void Initialize()
  {
    CharacterController.enabled = true;

    HealthInitialize();

    SetAnimator();
  }

  public void HealthInitialize()
  {
    Health = new Health();
    Health.SetMaxHealth(5);
    Health.Initialize();

    Health.OnInstantlyKill += Health_OnInstantlyKill;
  }

  public void UpgradeManagerInitialize(UpgradeManager parUpgradeManager)
  {
    UpgradeManager = parUpgradeManager;
  }

  public void SetPlayerSkinData(PlayerSkinData parPlayerSkinData)
  {
    PlayerSkinData = parPlayerSkinData;
  }

  #region Animator Layer

  public void SetAnimatorLayer(string parLayerName, float parWeight)
  {
    Animator.SetLayerWeight(Animator.GetLayerIndex(parLayerName), parWeight);
  }

  public void SetAnimatorLayer(int parLayerIndex, float parWeight)
  {
    Animator.SetLayerWeight(parLayerIndex, parWeight);
  }

  #endregion

  public void TakeDamage(int parDamage, Vector3 parForce, Vector3 parHitPoint)
  {
    Health.TakeHealth(parDamage);
  }

  //======================================

  public bool IsPlayerUnavailable()
  {
    if (Health == null)
      return true;

    return !CharacterController.enabled || Health.IsDead;
  }

  //======================================

  private void Health_OnInstantlyKill()
  {
    CharacterController.enabled = false;

    Animator.SetTrigger($"{PlayerAnimatorParams.IS_DIE}");
  }

  private void CreateSkin()
  {
    if (currentSkinObject != null)
      return;

    currentSkinObject = Instantiate(PlayerSkinData.SkinModel, transform);
    currentSkinObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
  }

  private void SetAnimator()
  {
    if (currentSkinObject == null)
      return;

    if (!currentSkinObject.TryGetComponent(out Animator parSkinAnimator))
      return;

    Animator.avatar = parSkinAnimator.avatar;
    Animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
  }

  //======================================

  private void OnRun(InputAction.CallbackContext context)
  {
    switch (context.phase)
    {
      case InputActionPhase.Started:
        IsRunning = true;
        break;
      case InputActionPhase.Canceled:
        IsRunning = false;
        break;
    }
  }

  //======================================
}