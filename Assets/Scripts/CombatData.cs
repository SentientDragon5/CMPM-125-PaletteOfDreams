using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatData : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
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
        if (gameObject.tag == "Player")
        {
            maxHealth = CombatManager.Instance.maxHealth;
            currHealth = CombatManager.Instance.currHealth;
            strength = CombatManager.Instance.strength;
            defense = CombatManager.Instance.defense;
            strengthMult = CombatManager.Instance.strengthMult;
            defenseMult = CombatManager.Instance.defenseMult;
        }
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //healthSlider.value = currHealth; //UI Testing
    }

    public float dealDamage()
    {
        return (1 * strength * strengthMult);
    }

    public void recieveDamage(float damage)
    {
        currHealth -= (int)((damage/2) - (defense/4));
        if (currHealth < 0)
        {
            currHealth = 0;
        }
        UpdateHealthUI();
    }

    public void recoverHealth(int health)
    {
        currHealth += health;
        if (currHealth > maxHealth)
        {
            currHealth = maxHealth;
        }
        UpdateHealthUI();
    }

    public void becomeWeak(float wMult, int turnCount)
    {
        if (turnsWeakened == 0)
        {
            strengthMult -= wMult;
        }
        turnsWeakened += turnCount;
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

    private void UpdateHealthUI()
    {
        healthSlider.value = currHealth;
    }
}
