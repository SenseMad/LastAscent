using System;

[Serializable]
public class Score
{
  public int CurrentScore { get; private set; }

  //======================================

  public event Action<int> OnAddScore;

  public event Action<int> OnTakeScore;

  public event Action<int> OnChangeScore;

  //======================================

  public Score(int parStartingScore)
  {
    AddScore(parStartingScore);
  }

  //======================================

  public void AddScore(int parScore)
  {
    if (parScore < 0)
      return;

    if (CurrentScore < 0)
      CurrentScore = 0;

    int scoreBefore = CurrentScore;
    CurrentScore += parScore;

    OnAddScore?.Invoke(CurrentScore - scoreBefore);
    OnChangeScore?.Invoke(CurrentScore);
  }

  public void TakeScore(int parScore)
  {
    if (parScore < 0)
      return;

    int scoreBefore = CurrentScore;
    CurrentScore -= parScore;

    if (CurrentScore < 0)
      CurrentScore = 0;

    OnTakeScore?.Invoke(scoreBefore - CurrentScore);
    OnChangeScore?.Invoke(CurrentScore);
  }

  //======================================

  public bool CanAfford(int parValue)
  {
    return CurrentScore >= parValue;
  }

  //======================================
}