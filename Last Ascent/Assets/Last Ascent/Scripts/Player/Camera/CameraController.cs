using Unity.Cinemachine;
using UnityEngine;

public sealed class CameraController : MonoBehaviour
{
  [Header("Camera")]
  [SerializeField] private CinemachineCamera _mainCinemachineCamera;

  [Header("Settings")]
  [SerializeField, Min(0)] private float _sensitivity = 1;
  [SerializeField] private Vector2 _angleRotation = new Vector2(-89, 89);

  [Header("Recoil Settings")]
  [SerializeField, Min(0)] private float _maxRecoil = 3f;
  [SerializeField, Min(0)] private float _recoilSpeed = 15f;
  [SerializeField, Min(0)] private float _recoilReturnSpeed = 15f;

  //--------------------------------------

  private Player player;

  private float cinemachineTargetYaw;
  private float cinemachineTargetPitch;

  private float currentPitch;

  private float recoilOffset = 0f;

  //======================================

  public CinemachineCamera MainCinemachineCamera => _mainCinemachineCamera;

  public Camera MainCamera { get; private set; }

  //======================================

  private void Awake()
  {
    player = GetComponentInParent<Player>();

    MainCamera = Camera.main;
  }

  private void Start()
  {
    cinemachineTargetYaw = transform.rotation.eulerAngles.y;
    currentPitch = cinemachineTargetPitch = transform.rotation.eulerAngles.x;
  }

  private void LateUpdate()
  {
    CameraRotation();
  }

  //======================================

  public void Initialize(CinemachineCamera parCinemachineCamera)
  {
    _mainCinemachineCamera = parCinemachineCamera;

    _mainCinemachineCamera.Target.TrackingTarget = transform;
  }

  public void ApplyRecoil(float parStrength)
  {
    //cinemachineTargetPitch = Mathf.Clamp(cinemachineTargetPitch - parStrength, _angleRotation.x, _angleRotation.y - _maxRecoil);
    recoilOffset += parStrength;
    recoilOffset = Mathf.Min(recoilOffset, _maxRecoil);
  }

  //======================================

  private void CameraRotation()
  {
    if (player.InputHandler == null)
      return;

    Vector2 look = player.InputHandler.Look();

    if (look.sqrMagnitude > 0.01f)
    {
      cinemachineTargetYaw += look.x * _sensitivity;
      cinemachineTargetPitch += look.y * _sensitivity;
    }

    recoilOffset = Mathf.MoveTowards(recoilOffset, 0f, _recoilReturnSpeed * Time.deltaTime);

    float pitchWithRecoil = cinemachineTargetPitch - recoilOffset;
    pitchWithRecoil = Mathf.Clamp(pitchWithRecoil, _angleRotation.x, _angleRotation.y);

    currentPitch = Mathf.Lerp(currentPitch, pitchWithRecoil, Time.deltaTime * _recoilSpeed);

    transform.rotation = Quaternion.Euler(currentPitch, cinemachineTargetYaw, 0f);

    /*cinemachineTargetPitch = Mathf.Clamp(cinemachineTargetPitch, _angleRotation.x, _angleRotation.y);

    currentPitch = Mathf.Lerp(currentPitch, cinemachineTargetPitch, Time.deltaTime * _recoilSpeed);

    transform.rotation = Quaternion.Euler(currentPitch, cinemachineTargetYaw, 0f);*/
  }

  private float ClampAngle(float lfAngle, float lfMin, float lfMax)
  {
    if (lfAngle < -360f) lfAngle += 360f;
    if (lfAngle > 360f) lfAngle -= 360f;
    return Mathf.Clamp(lfAngle, lfMin, lfMax);
  }

  //======================================
}