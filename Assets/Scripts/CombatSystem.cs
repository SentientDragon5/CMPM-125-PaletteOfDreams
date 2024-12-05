using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CombatSystem : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    [HideInInspector] public bool flightActive = false;
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
        if (flightActive)
        {
            CombatManager.Instance.FlightMode();
            onPlayerTurn.Invoke("The winds heal for 2 Health and deal 4 Damage");
            confirmed = false;
            // wait until confirmed
            yield return new WaitUntil(() => confirmed);
        }
        for (int i = 0; i < playerActs.Length; i++)
        {
            playerActs[i].SetActive(false);
        }
        switch (attackID)
        {
            case 0:
                string msg = CombatManager.Instance.RedAttack();
                onPlayerTurn.Invoke(msg);
                break;
            case 2:// yellow is 1, blue is 2
                msg = CombatManager.Instance.BlueAttack();
                onPlayerTurn.Invoke(msg);
                break;
            case 1:
                msg = CombatManager.Instance.YellowAttack();
                onPlayerTurn.Invoke(msg);
                break;
            case 3:
                onPlayerTurn.Invoke(dream.description);
                confirmed = false;
                // wait until confirmed
                yield return new WaitUntil(() => confirmed);
                msg = CombatManager.Instance.PaletteAttack(dream);
                onPlayerTurn.Invoke(msg);
                break;
            default:
                Debug.Log("Not Implemented, or error");
                break;
        }
        if (dream.name == "Flight")
        {
            flightActive = true;
        }
        confirmed = false;
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
        var enemyAttack = enemy.GetComponent<CombatData>().dealDamage();
        int dmg = player.GetComponent<CombatData>().recieveDamage(enemyAttack);

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
            CombatManager.Instance.OnExitBattle();
            // end game
            GameObject g = new GameObject("DIED");
            print("died, created object");
            DontDestroyOnLoad(g);
            SceneManager.LoadScene("MainMenu");
            // done
            endCheck = true;
            /*SceneManager.LoadScene(PlayerProgressManager.instance.worldName);
            endCheck = true;*/
        }
        if (enemy.GetComponent<CombatData>().currHealth <= 0)
        {
            SceneManager.LoadScene(PlayerProgressManager.instance.worldName);
            endCheck = true;
            /*CombatManager.Instance.OnExitBattle();
            // end game
            GameObject g = new GameObject("DIED"); // Rename to WIN for win
            print("died, created object");
            DontDestroyOnLoad(g);
            SceneManager.LoadScene("MainMenu");
            // done
            endCheck = true;*/
        }

        // Switches Combatant Turn
        if (enemyTurn && endCheck == false)
        {
            CombatManager.Instance.OnEndTurn();
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
