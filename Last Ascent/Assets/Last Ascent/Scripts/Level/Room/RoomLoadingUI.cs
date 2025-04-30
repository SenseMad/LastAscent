using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class RoomLoadingUI : MonoBehaviour
{
  [SerializeField] private Image _transitionBetweenUIRooms;

  [SerializeField] private float _blackOutTime = 1.0f;

  //--------------------------------------

  private CanvasGroup canvasGroup;

  private Coroutine coroutine;

  //======================================

  public bool IsActive { get; private set; }

  //======================================

  private void Awake()
  {
    canvasGroup = _transitionBetweenUIRooms.GetComponent<CanvasGroup>();
  }

  //======================================

  public void SetActive(bool parValue)
  {
    if (coroutine != null)
      return;

    coroutine = parValue ? StartCoroutine(BlackOut(0, 1)) : StartCoroutine(BlackOut(1, 0));

    IsActive = true;
  }

  public IEnumerator BlackOut(float parA, float parB)
  {
    float currentBlackOutTime = 0;

    while (currentBlackOutTime < _blackOutTime)
    {
      currentBlackOutTime += Time.deltaTime;
      float t = currentBlackOutTime / _blackOutTime;

      canvasGroup.alpha = Mathf.Lerp(parA, parB, t);

      yield return null;
    }

    coroutine = null;
    IsActive = false;
  }

  //======================================
}