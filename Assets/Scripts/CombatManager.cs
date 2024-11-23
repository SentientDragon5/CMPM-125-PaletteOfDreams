using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

public class CombatManager : MonoBehaviour
{
    private static CombatManager _instance;
    public static CombatManager Instance { get { return _instance; } }

    // Start is called before the first frame update
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Enemy;
    [HideInInspector] public int maxHealth = 22;
    [HideInInspector] public int currHealth = 22;
    [HideInInspector] public float strength = 8;
    [HideInInspector] public float defense = 6;
    [HideInInspector] public float strengthMult = 1;
    [HideInInspector] public float defenseMult = 1;
    public EnemyTemplate[] enemies; // Stores Enemy Scriptable Objects for testing
    private string currLevel;

    private Dictionary<string, int> abilityUses = new Dictionary<string, int>()
    {
        {"Red", 4},
        {"Blue", 4},
        {"Yellow", 4}
    };

    public UnityEvent onRefreshUI;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        onRefreshUI.Invoke();

        PlayerProgressManager.instance.onLoad.AddListener(LoadSaveData);
    }

    public void OnEnemyTurn()
    {
        
        onRefreshUI.Invoke();
    }

    void OnEndTurn()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Moves available to player
    public void RedAttack()
    {
        if (abilityUses["Red"] > 0)
        {
            Enemy.GetComponent<CombatData>().recieveDamage(strength * strengthMult);
            abilityUses["Red"]--;
        }
        else { // Reload Ability Uses back to 4
            abilityUses["Red"] = 4;
        }
        onRefreshUI.Invoke();
    }

    public void BlueAttack()
    {
        if (abilityUses["Blue"] > 0)
        {
            Enemy.GetComponent<CombatData>().becomeWeak(.5f, 2);
            abilityUses["Blue"]--;
        }
        else { // Reload Ability Uses back to 4
            abilityUses["Blue"] = 4;
        }
        onRefreshUI.Invoke();
    }

    public void YellowAttack()
    {
        if (abilityUses["Yellow"] > 0)
        {
            Player.GetComponent<CombatData>().recoverHealth((int)(maxHealth * .3f));
            if (Enemy.GetComponent<CombatData>().currHealth == Enemy.GetComponent<CombatData>().maxHealth || Enemy.GetComponent<CombatData>().currHealth <= 15)
            {
                Enemy.GetComponent<CombatData>().recieveDamage(strength * strengthMult);
            }
            abilityUses["Yellow"]--;
        }
        else { // Reload Ability Uses back to 4
            abilityUses["Yellow"] = 4;
        }
        onRefreshUI.Invoke();
    }

    // Returns enemy template based on location
    public EnemyTemplate GetEnemyTemplate()
    {
        // TODO: Use location to sort enemy pools and later pull from said pools
        return enemies[0]; // Test returning first EnemyTemplate - Capsule
    }

    // Loads and saves player data from PlayerProgressManager
    public void LoadSaveData()
    {
        maxHealth = PlayerProgressManager.instance.maxHp;
        currHealth = PlayerProgressManager.instance.hp;
        strength = PlayerProgressManager.instance.strength;
        defense = PlayerProgressManager.instance.defense;
        currLevel = PlayerProgressManager.instance.worldName; // Way to pull location for enemy pool
    }
    public void OnExitBattle()
    {
        PlayerProgressManager.instance.maxHp = maxHealth;
        PlayerProgressManager.instance.hp = currHealth;
        PlayerProgressManager.instance.strength = strength;
        PlayerProgressManager.instance.defense = defense;
    }

    /*public void updateMaxHealth(int pMaxHealth) { maxHealth = pMaxHealth; }
    public void updateCurrHealth(int pCurrHealth) { currHealth = pCurrHealth; }
    public void updateStrength(float pStrength) { strength = pStrength; }
    public void updateDefense(float pDefense) { defense = pDefense; }
    public void updateStrengthMult(float pStrengthMult) { strengthMult = pStrengthMult; }
    public void updateDefenseMult(float pDefenseMult) { defenseMult = pDefenseMult; }
    */


}
