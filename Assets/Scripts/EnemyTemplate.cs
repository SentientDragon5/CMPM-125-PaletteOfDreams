using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Combatants/EnemyInfo")]
public class EnemyTemplate : ScriptableObject
{
    public string enemyName;
    public int health;
    public float strength;
    public float defense;
    public MoveTemplate[] moves;
    public Sprite icon;
}
