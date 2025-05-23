using UnityEngine;

public class DestroyParticleSystem : MonoBehaviour
{
  private void OnDisable()
  {
    Destroy(gameObject);
  }
}