using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMove", menuName = "Combatants/EnemyMove")]
public class MoveTemplate : ScriptableObject
{
    public string moveName;
    public int damage;
}
