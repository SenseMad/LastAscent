using UnityEngine;
using UnityEngine.InputSystem;

public class DetectInteracted : MonoBehaviour
{
  [SerializeField] private Transform _pointDetection;

  [SerializeField, Min(0)] private float _radius = 2.0f;

  [SerializeField] private LayerMask _interactMask;

  //--------------------------------------

  private Player player;

  private readonly Collider[] detectedColliders = new Collider[4];

  //======================================

  public IInteractable NearestObject { get; private set; }

  public IDetectable NearestDetect { get; private set; }

  //======================================

  private void Awake()
  {
    player = GetComponent<Player>();
  }

  private void OnEnable()
  {
    player.InputHandler.InputActions.Player.Interact.performed += Interact_performed;
  }

  private void OnDisable()
  {
    player.InputHandler.InputActions.Player.Interact.performed -= Interact_performed;
  }

  private void Update()
  {
    DetectInteractiveOject();
  }

  //======================================

  public void DetectInteractiveOject()
  {
    NearestDetect?.UnDetect();

    NearestObject = null;
    NearestDetect = null;

    int count = Physics.OverlapSphereNonAlloc(_pointDetection == null ? transform.position : _pointDetection.position, _radius, detectedColliders, _interactMask);
    if (count <= 0)
      return;

    float nearestDistance = Mathf.Infinity;
    IInteractable interactable = null;
    IDetectable detectable = null;

    foreach (var collider in detectedColliders)
    {
      if (collider == null)
        continue;

      if (!collider.TryGetComponent(out IInteractable parInteractable))
        continue;

      float distance = Vector3.Distance(transform.position, collider.transform.position);

      if (distance < nearestDistance)
      {
        nearestDistance = distance;
        interactable = parInteractable;

        if (collider.TryGetComponent(out IDetectable parDetectable))
          detectable = parDetectable;
      }
    }

    NearestObject = interactable;
    NearestDetect = detectable;

    NearestDetect?.Detect();
  }

  //======================================

  private void Interact_performed(InputAction.CallbackContext obj)
  {
    if (NearestObject == null)
      return;

    NearestObject.Interact(player);
  }

  //======================================

  private void OnDrawGizmos()
  {
    Gizmos.color = Color.yellow;    
    Gizmos.DrawWireSphere(_pointDetection == null ? transform.position : _pointDetection.position, _radius);
  }

  //======================================
}