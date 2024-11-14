using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    //[SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Enemy;
    [SerializeField] public int maxHealth = 12;
    [SerializeField] public int currHealth = 18;
    public float strength = 1;
    public float defense = 1;
    public float strengthMult = 1;
    public float defenseMult = 1;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RedAttack()
    {
        Enemy.GetComponent<CombatData>().recieveDamage(10 * strength * strengthMult);
    }

    public void updateMaxHealth(int pMaxHealth) { maxHealth = pMaxHealth; }
    public void updateCurrHealth(int pCurrHealth) { currHealth = pCurrHealth; }
    public void updateStrength(float pStrength) { strength = pStrength; }
    public void updateDefense(float pDefense) { defense = pDefense; }
    public void updateStrengthMult(float pStrengthMult) { strengthMult = pStrengthMult; }
    public void updateDefenseMult(float pDefenseMult) { defenseMult = pDefenseMult; }



}
