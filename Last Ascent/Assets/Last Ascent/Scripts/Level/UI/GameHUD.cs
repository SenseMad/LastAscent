using TMPro;
using UnityEngine;
using Zenject;

public sealed class GameHUD : MonoBehaviour
{
  /*[SerializeField] private TextMeshProUGUI _healthText;

  //--------------------------------------

  private LevelManager levelManager;

  //======================================

  [Inject]
  private void Construct(LevelManager parLevelManager)
  {
    levelManager = parLevelManager;
  }

  //======================================

  private void OnEnable()
  {
    levelManager.OnInitialize += Initialize;
  }

  private void OnDestroy()
  {
    levelManager.OnInitialize -= Initialize;

    levelManager.Player.Health.OnChangeHealth -= Health_OnChangeHealth;
  }

  //======================================

  public void Initialize()
  {
    levelManager.Player.Health.OnChangeHealth += Health_OnChangeHealth;
  }

  //======================================

  private void Health_OnChangeHealth()
  {
    _healthText.text = $"{levelManager.Player.Health.CurrentHealth}/{levelManager.Player.Health.MaxHealth}";
  }

  //======================================*/
}