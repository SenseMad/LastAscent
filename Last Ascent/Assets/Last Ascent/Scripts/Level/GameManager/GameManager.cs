using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
  public GlobalProgressData GlobalProgressData { get; private set; }

  //======================================



  //======================================

  private void Awake()
  {
    GlobalProgressData = new GlobalProgressData();
  }

  //======================================



  //======================================
}