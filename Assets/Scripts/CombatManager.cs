using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
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
    [HideInInspector] public int turnCounter = 0;
    public EnemyTemplate[] enemies; // Stores Enemy Scriptable Objects for testing
    private string currLevel;

    private Dictionary<string, int> abilityUses = new Dictionary<string, int>()
    {
        {"Red", 4},
        {"Blue", 4},
        {"Yellow", 4}
    };

    public List<int> AbilityUses
    {
        get
        {
            int[] uses = new int[3];
            uses[0] = (abilityUses["Red"]);
            uses[1] = (abilityUses["Yellow"]);
            uses[2] = (abilityUses["Blue"]);
            return uses.ToList();
        }
        set
        {
            abilityUses["Red"] = value[0];
            abilityUses["Yellow"] = value[1];
            abilityUses["Blue"] = value[2];
        }
    }

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

    public void OnEndTurn()
    {
        onRefreshUI.Invoke();
        turnCounter++; // Turn counter to keep track for Bottled Rage
        Debug.Log("turning");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Moves available to player
    public void UseClear()
    {
        
    }

    // Player's Palette Ability (After 4+ turns, pulls dream info from CombatSystem.cs)
    public string PaletteAttack(Dream dream)
    {
        string msg = "";
        print("Palette attack!");
        if (dream.name == "Bottled Rage")
        {
            int dmg = Enemy.GetComponent<CombatData>().recieveDamage(turnCounter * 10);
            msg += "You dealt " + dmg + " damage! ";
        }
        else if (dream.name == "Thrill Ride")
        {
            CombatData playerData = Player.GetComponent<CombatData>();
            if (playerData.currHealth == playerData.maxHealth)
            {
                int dmg = Enemy.GetComponent<CombatData>().recieveDamage((dream.damage + strength) * strengthMult);
                msg += "You dealt " + dmg + " damage! ";
            }
            else
            {
                int dmg = Player.GetComponent<CombatData>().recoverHealth((int)(10 * .3f));
                msg += "You healed " + dmg + " HP! ";
            }
        }
        else if (dream.name == "Whiplash")
        {
            CombatData playerData = Player.GetComponent<CombatData>();

            int missingHealth = playerData.maxHealth - playerData.currHealth;
            int damage = missingHealth;
            int dmg = Enemy.GetComponent<CombatData>().recieveDamage(damage);
            msg += "You dealt " + dmg + " damage! ";
        }
        else if (dream.name == "Vacation")
        {
            int dmg = Enemy.GetComponent<CombatData>().recieveDamage(Player.GetComponent<CombatData>().currHealth);
            msg += "You dealt " + dmg + " damage! ";
        }
        else if (dream.name == "Flight")
        {
            msg += "All abilities now deal 4 damage and heal 2 HP! ";
        }
        else
        {
            if (dream.damage > 0)
            {
                int dmg = Enemy.GetComponent<CombatData>().recieveDamage((dream.damage + strength) * strengthMult);
                msg += "You dealt " + dmg + " damage! ";
            }
            if (dream.weakenLength > 0)
            {
                int turnsWeak = Enemy.GetComponent<CombatData>().becomeWeak(dream.weaken, dream.weakenLength);
                msg += "You weakened the enemy for 99 turns! (" + turnsWeak + " left) ";
            }
            if (dream.heal > 0)
            {
                int dmg = Player.GetComponent<CombatData>().recoverHealth((int)(15 * .3f));
                msg += "You healed " + dmg + " HP! ";
            }
            if (dream.selfWeak > 0)
            {

            }
        }
        Debug.Log(msg);
        return msg;
    }

    // Player's Red Ability (Damages enemy)
    public string RedAttack()
    {
        if (abilityUses["Red"] > 0)
        {
            int dmg = Enemy.GetComponent<CombatData>().recieveDamage(strength * strengthMult);
            abilityUses["Red"]--;
            onRefreshUI.Invoke();
            return ("You dealt " + dmg + " damage!");
        }
        else { // Reload Ability Uses back to 4
            abilityUses["Red"] = 4;
            onRefreshUI.Invoke();
            return ("Reloading Red.");
        }
    }

    // Player's Blue Ability (Weakens enemy)
    public string BlueAttack()
    {
        if (abilityUses["Blue"] > 0)
        {
            int turnsWeak = Enemy.GetComponent<CombatData>().becomeWeak(.5f, 2);
            abilityUses["Blue"]--;
            onRefreshUI.Invoke();
            return ("You weakened the enemy, now deals less damage. (" + turnsWeak + " turns)");
        }
        else { // Reload Ability Uses back to 4
            abilityUses["Blue"] = 4;
            onRefreshUI.Invoke();
            return ("Reloading Blue.");
        }
    }

    // Player's Yellow Ability (Heals and potentially deals damage)
    public string YellowAttack()
    {
        if (abilityUses["Yellow"] > 0)
        {
            int amtHealed = Player.GetComponent<CombatData>().recoverHealth((int)(maxHealth * .3f));
            if (Enemy.GetComponent<CombatData>().currHealth == Enemy.GetComponent<CombatData>().maxHealth || Enemy.GetComponent<CombatData>().currHealth <= 15)
            {
                //Enemy.GetComponent<CombatData>().recieveDamage(strength * strengthMult);
            }
            abilityUses["Yellow"]--;
            onRefreshUI.Invoke();
            return ("You healed " + amtHealed + " HP");
        }
        else { // Reload Ability Uses back to 4
            abilityUses["Yellow"] = 4;
            onRefreshUI.Invoke();
            return ("Reloading Yeallow.");
        }
    }

    public void FlightMode()
    {
        // Runs if the Flight Dream has been used in combat yet
        Player.GetComponent<CombatData>().recoverHealth(2);
        Enemy.GetComponent<CombatData>().recieveDamage((4f + strength) * strengthMult);
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
}
