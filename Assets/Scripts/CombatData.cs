using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CombatData : MonoBehaviour
{
    public EnemyTemplate enemyInfo;
    public GameObject enemyImage;
    [HideInInspector] public int maxHealth = 7;
    [HideInInspector] public int currHealth = 7;
    private float strength = 10;
    private float defense = 8;
    private float strengthMult = 1;
    private float defenseMult = 1;
    private int turnsBuffed = 0;
    private int turnsWeakened = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Loads player data from CombatManager
        if (gameObject.tag == "Player")
        {
            maxHealth = CombatManager.Instance.maxHealth;
            currHealth = CombatManager.Instance.currHealth;
            strength = CombatManager.Instance.strength;
            defense = CombatManager.Instance.defense;
            //strengthMult = CombatManager.Instance.strengthMult;
            //defenseMult = CombatManager.Instance.defenseMult;
        }

        // Loads enemy data
        if (gameObject.tag == "Enemy")
        {
            GetEnemyInfo();
        }

        Debug.Log(gameObject.tag + maxHealth);

    }

    public UnityEvent refreshUI = new();

    // Functions used by CombatSytem to affect stats of combatants
    public float dealDamage()
    {
        MoveTemplate move = enemyInfo.moves[Random.Range(0, enemyInfo.moves.Length)];
        return ((move.damage + strength) * strengthMult);
    }

    public int recieveDamage(float damage)
    {
        int dmg = (int)((damage / 2) - (defense / 4));
        if (dmg < 0)
        {
            dmg = 0;
        }
        currHealth -= dmg;
        if (currHealth < 0)
        {
            currHealth = 0;
        }
        refreshUI.Invoke();
        return dmg;
    }

    public int recoverHealth(int health)
    {
        int currCheck = currHealth;
        currHealth += health;
        if (currHealth > maxHealth)
        {
            currHealth = maxHealth;
            health = maxHealth - currCheck;
        }
        refreshUI.Invoke();
        return health;
    }

    public int becomeWeak(float wMult, int turnCount)
    {
        if (turnsWeakened == 0)
        {
            strengthMult -= wMult;
        }
        turnsWeakened += turnCount;
        return turnsWeakened;
    }

    public void UpdateTurnCounts()
    {
        if (turnsWeakened > 0)
        {
            turnsWeakened--;
        }
        if (turnsWeakened <= 0)
        {
            turnsWeakened = 0;
            strengthMult = 1;
        }
    }

    // Loads the enemyInfo from the CombatManager
    private void GetEnemyInfo()
    {
        enemyInfo = CombatManager.Instance.GetEnemyTemplate();
        maxHealth = enemyInfo.health;
        currHealth = enemyInfo.health;
        strength = enemyInfo.strength;
        defense = enemyInfo.defense;
        enemyImage.GetComponent<Image>().sprite = enemyInfo.icon;
        Debug.Log(maxHealth);
    }
}
