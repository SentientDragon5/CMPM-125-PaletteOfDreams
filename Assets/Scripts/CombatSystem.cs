using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        setUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Pre-Combat set-up before calling for player's turn
    public void setUp()
    {
        //Setup enemy moves/data?
        playerTurn();
    }

    //  Displays Player UI before awaiting input
    public void playerTurn()
    {
        for (int i = 0; i < playerActs.Length; i++)
        {
            playerActs[i].SetActive(true);
        }
    }

    // Recieves input and dream from CombatUIManager based on player selection, then calls CombatManager
    public IEnumerator playerAction(int attackID, Dream dream)
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
            case 2:// yellow is 1, blue is 2
                CombatManager.Instance.BlueAttack();
                break;
            case 1:
                CombatManager.Instance.YellowAttack();
                break;
            case 3:
                CombatManager.Instance.PaletteAttack(dream);
                break;
            default:
                Debug.Log("Not Implemented, or error");
                break;
        }
        
        confirmed = false;
        onPlayerTurn.Invoke("You did " + "<dmg> "+ " damage");
        // wait until confirmed
        yield return new WaitUntil(()=>confirmed);
        switchTurn(true);
    }

    public UnityEvent<string> onPlayerTurn;
    [SerializeField] bool confirmed = false;
    public void ConfirmMenu() => confirmed = true;

    public UnityEvent<string> onEnemyTurn;
    
    // Controls enemy actions (WIP - Will work on implementing variety of moves)
    public IEnumerator enemyAction()
    {
        var dmg = enemy.GetComponent<CombatData>().dealDamage();
        player.GetComponent<CombatData>().recieveDamage(dmg);

        confirmed = false;
        onEnemyTurn.Invoke("You took " + dmg +" damage");
        // wait until confirmed
        yield return new WaitUntil(() => confirmed);
        switchTurn(false);
    }

    // Checks for end conditions, then swaps to the opposite combatant's turn
    public void switchTurn(bool enemyTurn)
    {
        bool endCheck = false;
        // Battle end conditions based on health
        if (player.GetComponent<CombatData>().currHealth <= 0)
        {
            SceneManager.LoadScene(PlayerProgressManager.instance.worldName);
            endCheck = true;
        }
        if (enemy.GetComponent<CombatData>().currHealth <= 0)
        {
            CombatManager.Instance.OnExitBattle();
            SceneManager.LoadScene(PlayerProgressManager.instance.worldName);
            endCheck = true;
        }

        // Switches Combatant Turn
        if (enemyTurn && endCheck == false)
        {
            StartCoroutine(enemyAction());
        }
        //  On player's turn, a new "round" or combat begins, updating turn counters for effects
        else
        {
            player.GetComponent<CombatData>().UpdateTurnCounts();
            enemy.GetComponent<CombatData>().UpdateTurnCounts();
            playerTurn();
        }
    }

}
