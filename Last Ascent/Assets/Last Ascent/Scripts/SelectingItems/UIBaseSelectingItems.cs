using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIBaseSelectingItems : MonoBehaviour
{
  [SerializeField] private Transform _canvas;

  [Space]
  [SerializeField] protected Image _icon;

  [Header("Text")]
  [SerializeField] protected TextMeshProUGUI _titleText;

  //======================================



  //======================================

  private void Start()
  {
    Close();
  }

  //======================================

  public virtual void Open()
  {
    _canvas.gameObject.SetActive(true);
  }

  public virtual void Close()
  {
    _canvas.gameObject.SetActive(false);
  }

  //======================================
}