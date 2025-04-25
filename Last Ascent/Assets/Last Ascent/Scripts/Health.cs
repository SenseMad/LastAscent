using System;
using UnityEngine;

[Serializable]
public class Health
{
  [SerializeField] private int _maxHealth;

  //------------------------------------

  private int currentHealth;

  //====================================

  public int CurrentHealth
  {
    get => currentHealth;
    private set
    {
      currentHealth = value;
      OnChangeHealth?.Invoke();
    }
  }

  public int MaxHealth => _maxHealth;

  public bool IsDead { get; private set; }

  //====================================

  public Health() { }

  public Health(int parMaxHealth)
  {
    _maxHealth = parMaxHealth;
  }

  //====================================

  public event Action OnChangeHealth;

  public event Action<int> OnAddHealth;

  public event Action<int> OnTakeHealth;

  public event Action OnInstantlyKill;

  //====================================

  public void Initialize()
  {
    CurrentHealth = _maxHealth;
  }

  public void SetHealth(int parHealth)
  {
    if (parHealth < 0)
      throw new ArgumentOutOfRangeException(nameof(parHealth));

    CurrentHealth = parHealth;
  }

  public void SetMaxHealth(int parHealth)
  {
    if (parHealth < 0)
      throw new ArgumentOutOfRangeException(nameof(parHealth));

    _maxHealth = parHealth;
  }

  public void AddHealth(int parHealth)
  {
    if (parHealth < 0)
      throw new ArgumentOutOfRangeException(nameof(parHealth));

    int healthBefore = currentHealth;
    currentHealth += parHealth;

    if (currentHealth > _maxHealth)
      currentHealth = _maxHealth;

    CurrentHealth = currentHealth;

    int healthAmount = currentHealth - healthBefore;
    if (healthAmount > 0)
      OnAddHealth?.Invoke(healthAmount);
  }

  public virtual void TakeHealth(int parDamage)
  {
    if (parDamage < 0)
      throw new ArgumentOutOfRangeException(nameof(parDamage));

    int healthBefore = currentHealth;
    currentHealth -= parDamage;

    if (currentHealth < 0)
      currentHealth = 0;

    CurrentHealth = currentHealth;

    int damageAmount = healthBefore - CurrentHealth;
    if (damageAmount > 0)
      OnTakeHealth?.Invoke(damageAmount);

    if (currentHealth <= 0)
    {
      IsDead = true;
      OnInstantlyKill?.Invoke();
    }
  }

  public void InstantlyKill()
  {
    CurrentHealth = 0;
    IsDead = true;

    OnTakeHealth?.Invoke(_maxHealth);

    OnInstantlyKill?.Invoke();
  }

  //====================================
}