using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dream", menuName = "Dream")]
public class DreamSO : ScriptableObject
{
    public int damage = 25;
    public float weakenMult = 0.5f;
    public int weakenTurns = 99;
}
