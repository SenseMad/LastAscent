using UnityEngine;

public abstract class ChestBase : MonoBehaviour, IInteractable
{
  protected bool isOpened = false;

  //======================================

  public void Interact(Player parPlayer)
  {
    if (isOpened)
      return;

    isOpened = true;

    Open();
  }

  //======================================

  protected virtual void ItemChosen()
  {
    isOpened = true;
  }

  //======================================

  protected abstract void Open();

  //======================================
}