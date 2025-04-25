using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ListEnemies", menuName = "Data/Enemy/ListEnemies")]
public sealed class ListEnemies : ScriptableObject
{
  [field: SerializeField] public List<Enemy> Enemies { get; private set; }
}