using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] public int maxHealth = 12;
    [SerializeField] public int currHealth = 10;
    public float strength = 1;
    public float defense = 1;
    public float strengthMult = 1;
    public float defenseMult = 1;

    private Dictionary<string, int> abilityUses = new Dictionary<string, int>()
    {
        {"Red", 4},
        {"Blue", 4},
        {"Yellow", 4}
    };
    [SerializeField] private TextMeshProUGUI redUsesText;
    [SerializeField] private TextMeshProUGUI blueUsesText;
    [SerializeField] private TextMeshProUGUI yellowUsesText;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        PlayerProgressManager.instance.onLoad.AddListener(LoadSaveData);

        UpdateRedUsesUI();
        UpdateBlueUsesUI();
        UpdateYellowUsesUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RedAttack()
    {
        if (abilityUses["Red"] > 0)
        {
            Enemy.GetComponent<CombatData>().recieveDamage(10 * strength * strengthMult);
            abilityUses["Red"]--;
        }
        else { // Reload Ability Uses back to 4
            abilityUses["Red"] = 4;
        }
        UpdateRedUsesUI();
    }

    public void BlueAttack()
    {
        if (abilityUses["Blue"] > 0)
        {
            abilityUses["Blue"]--;
            return; // Temporary
            
        }
        else { // Reload Ability Uses back to 4
            abilityUses["Blue"] = 4;
        }
        UpdateBlueUsesUI();
    }

    public void YellowAttack()
    {
        if (abilityUses["Yellow"] > 0)
        {
            Player.GetComponent<CombatData>().recoverHealth(3);
            if (Enemy.GetComponent<CombatData>().currHealth == Enemy.GetComponent<CombatData>().maxHealth || Enemy.GetComponent<CombatData>().currHealth <= 15)
            {
                Enemy.GetComponent<CombatData>().recieveDamage(15 * strength * strengthMult);
            }
            abilityUses["Yellow"]--;
        }
        else { // Reload Ability Uses back to 4
            abilityUses["Yellow"] = 4;
        }
        UpdateYellowUsesUI();
    }

    private void UpdateRedUsesUI()
    {
        redUsesText.text = "Uses: " + abilityUses["Red"];
    }

    private void UpdateBlueUsesUI()
    {
        blueUsesText.text = "Uses: " + abilityUses["Blue"];
    }

    private void UpdateYellowUsesUI()
    {
        yellowUsesText.text = "Uses: " + abilityUses["Yellow"];
    }

    public void LoadSaveData()
    {
        maxHealth = PlayerProgressManager.instance.maxHp;
        currHealth = PlayerProgressManager.instance.hp;
        strength = PlayerProgressManager.instance.strength;
        defense = PlayerProgressManager.instance.defense;
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
