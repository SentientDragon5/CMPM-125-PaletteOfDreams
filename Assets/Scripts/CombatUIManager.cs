using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class Dream
{
    public string name;
    public int damage;
    public float weaken;
    public int weakenLength;
}
public class CombatUIManager : MonoBehaviour
{
    
    public enum PColor
    {
        Red,
        Yellow,
        Blue,
        Orange, // Secondary color
        Green,  // Secondary color
        Purple  // Secondary color
    }
    
    public List<Image> splotches;
    public Image previewSplotch;
    public UITorus previewTorus;
    public List<TextMeshProUGUI> previewText;
    public List<Color> pColors;
    
    public List<PColor> inputColors;

    public List<List<TextMeshProUGUI>> colorAmounts;
    
    public UICurve playerHeathBar;
    public List<TextMeshProUGUI> playerHealth;
    public UICurve enemyHeathBar;
    public List<TextMeshProUGUI> enemyHealth;
    
    public Animator playerAnimator;
    public CombatData playerData;
    public CombatData enemyData;
    public CombatSystem combatSystem;

    public List<DreamSO> dreams;

    private void Awake()
    {
        
    }

    Color CombineColors(List<PColor> colors)
    {
        if (colors == null || colors.Count == 0)
            return Color.gray;
        
        Color result = new Color(1, 1, 1); // Start with white
        colors = new(colors);
        // 1. Combine any primary colors into secondary colors
        for (int i = 0; i < colors.Count - 1; i++)
        {
            for (int j = i + 1; j < colors.Count; j++)
            {
                if ((colors[i] == PColor.Red && colors[j] == PColor.Yellow) ||
                    (colors[i] == PColor.Yellow && colors[j] == PColor.Red))
                {
                    colors[i] = PColor.Orange;
                    colors.RemoveAt(j);
                    j--; // Adjust index after removing an element
                }
                else if ((colors[i] == PColor.Red && colors[j] == PColor.Blue) ||
                         (colors[i] == PColor.Blue && colors[j] == PColor.Red))
                {
                    colors[i] = PColor.Purple;
                    colors.RemoveAt(j);
                    j--;
                }
                else if ((colors[i] == PColor.Yellow && colors[j] == PColor.Blue) ||
                         (colors[i] == PColor.Blue && colors[j] == PColor.Yellow))
                {
                    colors[i] = PColor.Green;
                    colors.RemoveAt(j);
                    j--;
                }
            }
        }

        // 2. Combine the remaining colors (including any newly formed secondary colors)
        for (int i = 0; i < colors.Count; i++)
        {
            result *= Color.white - pColors[(int)colors[i]];
        }

        return Color.white - result; 
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    Dream FindDream(List<PColor> input)
    {
        Vector3Int c = new Vector3Int(0,0,0);
        // count our colors
        for (int i = 0; i < input.Count; i++)
        {
            if (input[i] == PColor.Red) c.x++;
            if (input[i] == PColor.Yellow) c.y++;
            if (input[i] == PColor.Blue) c.z++;
        }
        
        
        
        if (c.x == c.y && c.z == c.x)
        {
            // all same
            var dream = new Dream();
            dream.name = "None";
            
            return dream;
        }


        if (c.x >= 4)
        {
            // Volcano
            var dream = new Dream();
            dream.name = "Volcano";
            
            return dream;
        }
        if (c.y >= 4)
        {
            // Thrill ride
            var dream = new Dream();
            dream.name = "Thrill Ride";
            
            return dream;
        }
        if (c.z >= 4)
        {
            // Lucidity
            var dream = new Dream();
            dream.name = "Lucidity";
            
            return dream;
        }
        
        if (c.x >= 2 && c.z >= 2)
        {
            // Break up
            var dream = new Dream();
            dream.name = "Break up";
            
            return dream;
        }
        if (c.y >= 2  && c.x >= 2)
        {
            // Whiplash
            var dream = new Dream();
            dream.name = "Whiplash";
            
            return dream;
        }
        if (c.z >= 2  && c.y >= 2)
        {
            // Relaxation 
            var dream = new Dream();
            dream.name = "Relaxation";
            
            return dream;
        }
        
        if (c.x > c.y && c.x > c.z)
        {
            // Bottled rage
            var dream = new Dream();
            dream.name = "Bottled Rage";
            
            return dream;
        }
        if (c.y > c.x && c.y > c.z)
        {
            //  Vacacion
            var dream = new Dream();
            dream.name = "Vacation";
            
            return dream;
        }
        if (c.y > c.x && c.z > c.y)
        {
            // Flight
            var dream = new Dream();
            dream.name = "Flight";
            
            return dream;
        }
        
        // none
        
        var defaultDream = new Dream();
        defaultDream.name = "None";
            
        return defaultDream;
    }

    public void AddColor(int colorIndex)
    {
        inputColors.Add(((PColor)colorIndex));
        RefreshUI();
    }

    public void RemoveLast()
    {
        inputColors.RemoveAt(inputColors.Count - 1);
        RefreshUI();
    }

    public void Attack()
    {
        // ADD On Player attack effects here
        playerData.dealDamage();
        //CombatSystem.playerAction();
        RefreshUI();
        
    }

    // hook this up to any time the player or enemy gets hit
    public void Hit()
    {
        RefreshUI();
    }

    [ContextMenu("Refresh")]
    public void RefreshUI()
    {
        // Splotches and preview color
        Color previewColor = CombineColors(inputColors);
        previewSplotch.color = previewColor;
        previewTorus.color = previewColor;
        for (int i = 0; i < splotches.Count; i++)
        {
            if (i < inputColors.Count)
            {
                splotches[i].color = pColors[(int)inputColors[i]];
            }
            else
            {
                splotches[i].color = Color.clear;
            }
        }
        
        var dream = FindDream(inputColors);
        
        // preview name
        foreach (var t in previewText)
        {
            t.text = dream.name;
        }
        
        // number of ink left
        for (int i = 0; i < colorAmounts.Count; i++)
        {
            for (int j = 0; j < colorAmounts[i].Count; j++)
            {
                colorAmounts[i][j].text = 0.ToString(); // Get the PP
            }
        }
        
        // Health bars
        float enemyhp = enemyData.currHealth;
        float enemymhp = enemyData.maxHealth;
        for (int i = 0; i < enemyHealth.Count; i++)
        {
            enemyHealth[i].text = enemyhp.ToString() + "/" + enemymhp.ToString() + " HP";
        }
        enemyHeathBar.fillAmount = enemyhp/enemymhp;
        
        float playerhp = playerData.currHealth;
        float playermhp = playerData.maxHealth;
        for (int i = 0; i < playerHealth.Count; i++)
        {
            playerHealth[i].text = playerhp.ToString() + "/" + playermhp.ToString() + " HP";
        }
        playerHeathBar.fillAmount = playerhp/playermhp;

    }
}
