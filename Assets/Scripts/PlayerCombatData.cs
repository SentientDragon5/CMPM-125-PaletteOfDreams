using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombatData : MonoBehaviour
{
    [SerializeField] private Slider pHealthSlider;
    [SerializeField] private int maxPlayerHealth = 10;
    [SerializeField] private int currPlayerHealth = 10;
    private int playerStrength = 1;
    private int playerDefense = 1;
    private int strengthMult = 1;
    private int defenseMult = 1;
    // Start is called before the first frame update
    void Start()
    {
        pHealthSlider.maxValue = maxPlayerHealth;
        pHealthSlider.value = currPlayerHealth;
    }

    // Update is called once per frame
    void Update()
    {
        pHealthSlider.value = currPlayerHealth; //UI Testing
    }

    /*private void UpdateHealthUI()
    {
        pHealthSlider.value = currPlayerHealth;
    }*/
}
