using System;

public sealed class GlobalProgressData
{
  private float critChance = 0.05f;

  //======================================

  public float CritChance
  {
    get => critChance;
    set
    {
      critChance = value;
      OnChangeCritChance?.Invoke(value);
    }
  }

  //======================================

  public event Action<float> OnChangeCritChance;

  //======================================
}