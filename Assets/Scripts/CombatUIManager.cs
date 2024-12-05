using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class Dream
{
    public string name = "default"; // Gave dream default variables
    public string description = "default";
    public int damage = 0;
    public float weaken = 0;
    public int weakenLength = 0;
    public int heal = 0;
    public float selfWeak = 0;
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

    public List<int> Uses => CombatManager.Instance.AbilityUses;
    public int maxSlots = 4;
    
    public List<Image> splotches;
    public Image previewSplotch;
    public UITorus previewTorus;
    public List<TextMeshProUGUI> previewText;
    public List<Color> pColors;
    
    public List<PColor> inputColors;

    public List<TextMeshProUGUI> rAmount;
    public List<TextMeshProUGUI> yAmount;
    public List<TextMeshProUGUI> bAmount;

    public List<BPMScaler> colorBpms;
    public List<Button> colorButtons;
    public BPMScaler attackBpm;
    public Button attackButton;
    
    public UICurve playerHeathBar;
    public List<TextMeshProUGUI> playerHealth;
    public UICurve enemyHeathBar;
    public List<TextMeshProUGUI> enemyHealth;
    public TextMeshProUGUI enemyName;

    public Canvas textboxCanvas;
    public TextMeshProUGUI textbox;
    public Button textboxConfirm;

    public Button redButton;
    
    public Animator playerAnimator;
    public CombatData playerData;
    public CombatData enemyData;
    public CombatSystem combatSystem;

    public EventSystem eventSystem;

    public Dream dream; // Declared dream, future references now update dream
    
    private void Awake()
    {
        combatSystem.onEnemyTurn.AddListener(OnEnemyTurn);
        combatSystem.onPlayerTurn.AddListener(OnPlayerSubmit);
        playerData.refreshUI.AddListener(RefreshUI);
        enemyData.refreshUI.AddListener(RefreshUI);
        PlayerProgressManager.instance.onLoad.AddListener(RefreshUI);
        
        textboxCanvas.enabled = false;
    }

    public void OnEnemyTurn(string text)
    {
        playerAnimator.CrossFade("Hit",0.1f);
        textbox.text = text;
        textboxCanvas.enabled = true;
        eventSystem.SetSelectedGameObject(textboxConfirm.gameObject);
        
        for (int i = 0; i < 3; i++)
        {
            colorBpms[i].enabled = false;
        }
    }

    public void OnPlayerSubmit(string text)
    {
        playerAnimator.CrossFade("Attack",0.1f);
        textbox.text = text;
        textboxCanvas.enabled = true;
        eventSystem.SetSelectedGameObject(textboxConfirm.gameObject);
        
        
        for (int i = 0; i < 3; i++)
        {
            colorBpms[i].enabled = false;
        }
    }
    public void OnConfirmTextbox()
    {
        textboxCanvas.enabled = false;
        combatSystem.ConfirmMenu();
        eventSystem.SetSelectedGameObject(redButton.gameObject);
        RefreshUI();
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
    
    void Start()
    {
        RefreshUI();
        
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
            dream = new Dream();
            dream.name = "None";
            dream.description = "None";
            dream.damage = 0;
            dream.weaken = 0;
            dream.weakenLength = 0;
            dream.heal = 0;
            dream.selfWeak = 0;
            return dream;
        }


        if (c.x >= 4)
        {
            // Volcano
            dream = new Dream();
            dream.name = "Volcano";
            dream.description = "Deal 25 damage.";
            dream.damage = 25;
            dream.weaken = 0;
            dream.weakenLength = 0;
            dream.heal = 0;
            dream.selfWeak = 0;
            return dream;
        }
        if (c.y >= 4)
        {
            // Thrill ride
            dream = new Dream();
            dream.name = "Thrill Ride";
            dream.description = "If you�re full Health, deal 35 Damage. Else, heal 10 Health.";
            dream.damage = 0;
            dream.weaken = 0;
            dream.weakenLength = 0;
            dream.heal = 0;
            dream.selfWeak = 0;
            return dream;
        }
        if (c.z >= 4)
        {
            // Lucidity
            dream = new Dream();
            dream.name = "Lucidity";
            dream.description = "Weaken the enemy for 99 turns. Deal 10 Damage";
            dream.weaken = .5f;
            dream.weakenLength = 99;
            dream.damage = 0;
            dream.heal = 0;
            dream.selfWeak = 0;
            return dream;
        }
        
        if (c.x >= 2 && c.z >= 2)
        {
            // Break up
            dream = new Dream();
            dream.name = "Break up";
            dream.description = "Heal up to half of your max Health.";
            dream.damage = 0;
            dream.weaken = 0;
            dream.weakenLength = 0;
            dream.heal = 0;
            dream.selfWeak = 0;
            return dream;
        }
        if (c.y >= 2  && c.x >= 2)
        {
            // Whiplash
            dream = new Dream();
            dream.name = "Whiplash";
            dream.description = "Deal Damage equal to twice your missing Health.";
            dream.damage = 0;
            dream.weaken = 0;
            dream.weakenLength = 0;
            dream.heal = 0;
            dream.selfWeak = 0;
            return dream;
        }
        if (c.z >= 2  && c.y >= 2)
        {
            // Relaxation 
            dream = new Dream();
            dream.name = "Relaxation";
            dream.description = "Heal 15 Health and deal 20 Damage. Weaken yourself for 2 turns.";
            dream.damage = 0;
            dream.weaken = 0;
            dream.heal = 15;
            dream.weakenLength = 2;
            dream.selfWeak = .5f;
            return dream;
        }
        
        if (c.x > c.y && c.x > c.z)
        {
            // Bottled rage
            dream = new Dream();
            dream.name = "Bottled Rage";
            dream.description = "Deal 10 Damage for each turn of combat.";
            dream.damage = 0;
            dream.weaken = 0;
            dream.weakenLength = 0;
            dream.heal = 0;
            dream.selfWeak = 0;
            return dream;
        }
        if (c.y > c.x && c.y > c.z)
        {
            //  Vacation
            dream = new Dream();
            dream.name = "Vacation";
            dream.description = "Deal Damage equal to your current Health.";
            dream.damage = 0;
            dream.weaken = 0;
            dream.weakenLength = 0;
            dream.heal = 0;
            dream.selfWeak = 0;
            return dream;
        }
        if (c.y > c.x && c.z > c.y)
        {
            // Flight
            dream = new Dream();
            dream.name = "Flight";
            dream.description = "Heal for 2 Health and deal 4 Damage at the end of each turn.";
            dream.damage = 0;
            dream.weaken = 0;
            dream.weakenLength = 0;
            dream.heal = 0;
            dream.selfWeak = 0;
            return dream;
        }
        
        // none
        dream = new Dream();
        dream.name = "None";
        dream.description = "None";
        dream.damage = 0;
        dream.weaken = 0;
        dream.weakenLength = 0;
        dream.heal = 0;
        dream.selfWeak = 0;
        return dream;
    }

    public void AddColor(int colorIndex)
    {
        if (Uses[colorIndex] <= 0)
        {
            combatSystem.StartCoroutine(combatSystem.playerAction(colorIndex, dream));
        }
        if (inputColors.Count < maxSlots)
        {
            inputColors.Add(((PColor)colorIndex));
            combatSystem.StartCoroutine(combatSystem.playerAction(colorIndex, dream));
        }
        RefreshUI();
    }

    public void RemoveLast()
    {
        inputColors.RemoveAt(inputColors.Count - 1);
        RefreshUI();
    }

    public void Attack()
    {
        combatSystem.StartCoroutine(combatSystem.playerAction(3, dream));
        inputColors.Clear();
        
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
        
        dream = FindDream(inputColors);
        
        // preview name
        foreach (var t in previewText)
        {
            t.text = dream.name;
        }
        
        // number of ink left
        foreach (var t in rAmount)
            t.text = Uses[0].ToString();
        foreach (var t in yAmount)
            t.text = Uses[1].ToString();
        foreach (var t in bAmount)
            t.text = Uses[2].ToString();
        
        // Health bars
        float enemyhp = enemyData.currHealth;
        float enemymhp = enemyData.maxHealth;
        for (int i = 0; i < enemyHealth.Count; i++)
        {
            enemyHealth[i].text = enemyhp.ToString() + "/" + enemymhp.ToString() + " HP";
        }
        enemyHeathBar.fillAmount = enemyhp/enemymhp;
        enemyHeathBar.SetAllDirty();

        enemyName.text = enemyData.enemyInfo.enemyName;
        
        float playerhp = playerData.currHealth;
        float playermhp = playerData.maxHealth;
        for (int i = 0; i < playerHealth.Count; i++)
        {
            playerHealth[i].text = playerhp.ToString() + "/" + playermhp.ToString() + " HP";
        }
        playerHeathBar.fillAmount = playerhp/playermhp;
        playerHeathBar.SetAllDirty();

        
        // Buttons
        bool paletteAttack = inputColors.Count >= 4;
        attackBpm.enabled = paletteAttack;
        attackButton.interactable = paletteAttack;
        //attackButton.gameObject.SetActive(paletteAttack);
        for (int i = 0; i < 3; i++)
        {
            colorBpms[i].enabled = !paletteAttack;
            colorButtons[i].interactable = !paletteAttack;
        }
    }
}
