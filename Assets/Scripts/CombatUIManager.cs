using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

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
    
    
    Color CombineColors(List<PColor> colors)
    {
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
        
        // preview name
        foreach (var t in previewText)
        {
            t.text = "Rollercoaster";
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
