using TMPro;
using UnityEngine;
using Zenject;

public sealed class GameHealthUI : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI _healthText;

  [SerializeField] private RectTransform _healthRectTransform;

  //--------------------------------------

  private LevelManager levelManager;

  private float maxWidthProgressBar;

  //======================================

  [Inject]
  private void Construct(LevelManager parLevelManager)
  {
    levelManager = parLevelManager;
  }

  //======================================

  private void Awake()
  {
    maxWidthProgressBar = _healthRectTransform.rect.width;
  }

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
    Health_OnChangeHealth();

    levelManager.Player.Health.OnChangeHealth += Health_OnChangeHealth;
  }

  //======================================

  private void Health_OnChangeHealth()
  {
    int maxHealth = levelManager.Player.Health.MaxHealth;
    int currentHealth = levelManager.Player.Health.CurrentHealth;

    _healthText.text = $"{currentHealth}/{maxHealth}";

    float fillAmount = Mathf.Clamp01((float)currentHealth / (float)maxHealth);
    _healthRectTransform.sizeDelta = new Vector2(maxWidthProgressBar * fillAmount, _healthRectTransform.sizeDelta.y);
  }

  //======================================
}