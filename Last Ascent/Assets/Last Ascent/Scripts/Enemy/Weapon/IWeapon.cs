using UnityEngine;

public interface IWeapon
{
  bool Attack(Transform parTarget, GameObject parOwner);
}