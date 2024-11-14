using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    private GameObject[] playerActs;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        playerActs = GameObject.FindGameObjectsWithTag("PlayerTurn");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setUp()
    {
        //Setup enemy moves/data?
        playerTurn();
    }

    public void playerTurn()
    {
        for (int i = 0; i < playerActs.Length; i++)
        {
            playerActs[i].SetActive(true);
        }
    }

    public void playerAction(int attackID)
    {
        for (int i = 0; i < playerActs.Length; i++)
        {
            playerActs[i].SetActive(false);
        }
        switch (attackID)
        {
            case 0:
                CombatManager.Instance.RedAttack();
                break;
            case 1:
                CombatManager.Instance.BlueAttack();
                break;
            case 2:
                CombatManager.Instance.YellowAttack();
                break;
            default:
                Debug.Log("Not Implemented, or error");
                break;
        }
        switchTurn(true);
    }

    public void enemyAction()
    {
        player.GetComponent<CombatData>().recieveDamage(enemy.GetComponent<CombatData>().dealDamage());
        switchTurn(false);
    }

    public void switchTurn(bool enemyTurn)
    {
        // add end battle condition
        if (!enemyTurn)
        {
            playerTurn();
        }
        else
        {
            enemyAction();
        }
    }

}
