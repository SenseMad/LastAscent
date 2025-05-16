using UnityEngine;

public class UpgradeManager : MonoBehaviour
{


  //======================================

  public void ApplyUpgrade(Player parPlayer, UpgradeData parUpgradeData)
  {
    Debug.Log($"Applied upgrade: {parUpgradeData.Title}");
  }

  //======================================
}