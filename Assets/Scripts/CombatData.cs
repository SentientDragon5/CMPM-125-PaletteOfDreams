using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatData : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] public int maxHealth = 10;
    [SerializeField] public int currHealth = 10;
    private float strength = 1;
    private float defense = 1;
    private float strengthMult = 1;
    private float defenseMult = 1;
    // Start is called before the first frame update
    void Start()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currHealth;
        if (gameObject.tag == "Player")
        {
            maxHealth = CombatManager.Instance.maxHealth;
            currHealth = CombatManager.Instance.currHealth;
            strength = CombatManager.Instance.strength;
            defense = CombatManager.Instance.defense;
            strengthMult = CombatManager.Instance.strengthMult;
            defenseMult = CombatManager.Instance.defenseMult;
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = currHealth; //UI Testing
    }

    public float dealDamage()
    {
        return (10 * strength * strengthMult);
    }

    public void recieveDamage(float damage)
    {
        currHealth -= (int)((damage/2) * (defense/4)) ;
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

    private void UpdateHealthUI()
    {
        healthSlider.value = currHealth;
    }
}
