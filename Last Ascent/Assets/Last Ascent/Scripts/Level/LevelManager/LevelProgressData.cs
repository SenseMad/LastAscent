using System;

public sealed class LevelProgressData
{
  private float critChance = 0f;

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
