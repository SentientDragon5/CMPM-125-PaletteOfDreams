using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textUI;
    private int maxHealth;
    private int currHealth;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        maxHealth = gameObject.GetComponent<CombatData>().maxHealth;
        currHealth = gameObject.GetComponent<CombatData>().currHealth;
        textUI.text = currHealth + "/" + maxHealth;
    }
}
