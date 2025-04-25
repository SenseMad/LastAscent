using UnityEngine;

public sealed class InputHandler : MonoBehaviour
{
  public InputSystem_Actions InputActions { get; private set; }

  public bool IsControlBlocked { get; private set; }

  //======================================

  private void Awake()
  {
    InputActions = new InputSystem_Actions();

    SetCursor(false);
  }

  private void OnEnable()
  {
    InputActions.Enable();
  }

  private void OnDisable()
  {
    InputActions.Disable();
  }

  //======================================

  public void SetCursor(bool parValue)
  {
    Cursor.visible = parValue;
    Cursor.lockState = parValue ? CursorLockMode.None : CursorLockMode.Locked;
  }

  //======================================

  public Vector2 Move()
  {
    return IsControlBlocked ? Vector2.zero : InputActions.Player.Move.ReadValue<Vector2>();
  }

  public Vector2 Look()
  {
    return InputActions.Player.Look.ReadValue<Vector2>();
  }

  //======================================
}