using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class HUDHealthBar : MonoBehaviour
{
    public UICurve playerHealthBar;
    void Start()
    {
        float playerhp = PlayerProgressManager.instance.hp;
        float playermhp = PlayerProgressManager.instance.maxHp;
        playerHealthBar.fillAmount = playerhp/playermhp;
    }

}
