using UnityEngine;
using Zenject;

public class UpgradeManager : MonoBehaviour
{
  private LevelManager levelManager;

  //======================================

  [Inject]
  private void Construct(LevelManager parLevelManager)
  {
	levelManager = parLevelManager;
  }

  //======================================

  public void ApplyUpgrade(UpgradeData parUpgradeData)
  {
	switch (parUpgradeData.UpgradeType)
	{
	  case UpgradeType.CritChance:
		levelManager.LevelProgressData.CritChance += parUpgradeData.Value;
        break;
	}

	Debug.Log($"Applied upgrade: {parUpgradeData.Title}");
  }

  //======================================
}