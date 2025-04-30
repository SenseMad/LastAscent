using System;
using UnityEngine;
using Zenject;

public sealed class PlayerMovement : MonoBehaviour, ITickable
{
  [Header("Speed")]
  [SerializeField, Min(0)] private float _speedWalking = 2.0f;
  [SerializeField, Min(0)] private float _speedRunning = 5.0f;
  [SerializeField, Min(0)] private float _speedAiming = 3.0f;

  [Header("Boost")]
  [SerializeField, Min(0)] private float _acceleration = 9.0f;
  [SerializeField, Min(0)] private float _deceleration = 11.0f;

  [Header("Gravity")]
  [SerializeField, Min(0)] private float _gravity = 35.0f;
  [SerializeField, Min(0)] private float _jumpForce = 1.5f;

  [Header("Rotation")]
  [SerializeField, Min(0)] private float _rotationSpeed = 5;

  //--------------------------------------

  private Player player;

  private float animationBlend;

  private Vector3 velocity;

  private bool isGrounded;
  private bool wasGrounded;

  private bool isJumping;
  private float lastJumpTime;

  private bool isRotatingToTarget = false;
  private Vector3 targetLookDirection;
  private Action onRotationComplete;

  private bool isRotationLocked;

  //======================================

  private void Awake()
  {
    player = GetComponent<Player>();
  }

  private void Update()
  {
    Tick();
  }

  public void Tick()
  {
    RotatingToTarget();

    isGrounded = player.CharacterController.isGrounded;

    if (isGrounded && !wasGrounded)
    {
      isJumping = false;
      lastJumpTime = 0.0f;
    }
    else if (wasGrounded && !isGrounded)
      lastJumpTime = Time.time;

    Move();

    wasGrounded = isGrounded;
  }

  //======================================

  public void RotateTowardsPoint(Vector3 parTargetPoint, Action parOnComplete = null)
  {
    targetLookDirection = parTargetPoint - transform.position;
    targetLookDirection.y = 0;

    if (targetLookDirection != Vector3.zero)
    {
      isRotatingToTarget = true;
      onRotationComplete = parOnComplete;
    }
  }

  public void Jump()
  {
    if (!isGrounded)
      return;

    isJumping = true;
    velocity = new Vector3(velocity.x, Mathf.Sqrt(2.0f * _jumpForce * _gravity), velocity.z);

    lastJumpTime = Time.time;
  }

  //======================================

  private void Move()
  {
    Vector2 frameInput = Vector2.ClampMagnitude(player.InputHandler.Move(), 1.0f);

    if (player.IsPlayerUnavailable())
      frameInput = Vector2.zero;

    Transform camTransform = player.CameraController.MainCinemachineCamera.transform;

    Vector3 camForward = camTransform.forward;
    camForward.y = 0f;
    camForward.Normalize();

    Vector3 camRight = camTransform.right;
    camRight.y = 0f;
    camRight.Normalize();

    Vector3 moveDirection = camRight * frameInput.x + camForward * frameInput.y;
    moveDirection.Normalize();

    float targetSpeed = (!player.IsRunning || player.IsAiming) ? _speedWalking : _speedRunning;
    Vector3 desiredVelocity = moveDirection * targetSpeed;

    if (moveDirection != Vector3.zero)
      animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * _acceleration);
    else
      animationBlend = Mathf.Lerp(animationBlend, 0f, Time.deltaTime * _deceleration);

    if (moveDirection.sqrMagnitude > 0.01f && !isRotationLocked)
    {
      Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
      transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
    }

    velocity = Vector3.Lerp(
        new Vector3(velocity.x, 0f, velocity.z),
        new Vector3(desiredVelocity.x, 0f, desiredVelocity.z),
        Time.deltaTime * (moveDirection.sqrMagnitude > 0.0f ? _acceleration : _deceleration)
    );

    if (!isGrounded)
    {
      if (wasGrounded && !isJumping)
        velocity.y = 0.0f;

      velocity.y -= _gravity * Time.deltaTime;
    }
    else
      velocity.y = -0.03f;

    if (player.CharacterController.enabled)
      player.CharacterController.Move(velocity * Time.deltaTime);

    player.Animator.SetFloat(PlayerAnimatorParams.SPEED, animationBlend);

    Vector3 localMoveDirection = transform.InverseTransformDirection(desiredVelocity);
    Vector2 moveInput = new Vector2(localMoveDirection.x, localMoveDirection.z);
    if (moveInput.magnitude > 1f)
      moveInput.Normalize();

    player.Animator.SetFloat(PlayerAnimatorParams.MOVE_X, moveInput.x, 0.1f, Time.deltaTime);
    player.Animator.SetFloat(PlayerAnimatorParams.MOVE_Y, moveInput.y, 0.1f, Time.deltaTime);
  }

  private void RotatingToTarget()
  {
    /*if (isRotatingToTarget)
    {
      Quaternion targetRotation = Quaternion.LookRotation(targetLookDirection);
      transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed * 5);

      float angle = Quaternion.Angle(transform.rotation, targetRotation);
      if (angle < 1f)
      {
        transform.rotation = targetRotation;
        isRotatingToTarget = false;
        onRotationComplete?.Invoke();
        onRotationComplete = null;
      }
    }*/

    if (isRotatingToTarget)
    {
      Quaternion targetRotation = Quaternion.LookRotation(targetLookDirection);
      transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1000.0f * Time.deltaTime);

      float angle = Quaternion.Angle(transform.rotation, targetRotation);
      if (angle < 1f)
      {
        transform.rotation = targetRotation;
        isRotatingToTarget = false;
        onRotationComplete?.Invoke();
        onRotationComplete = null;
      }
    }

    Weapon activeWeapon = player.WeaponInventory.ActiveWeapon;

    if (activeWeapon != null)
    {
      if (activeWeapon.IsAttack)
        isRotationLocked = true;

      if (isRotationLocked && Time.time - player.WeaponInventory.LastAttackTime > 1.0f)
        isRotationLocked = false;
    }
  }

  //======================================
}