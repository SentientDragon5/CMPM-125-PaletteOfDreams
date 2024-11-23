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

    public IEnumerator playerAction(int attackID)
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
                CombatManager.Instance.PaletteAttack();
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

    public void switchTurn(bool enemyTurn)
    {
        // Battle end conditions
        if (player.GetComponent<CombatData>().currHealth <= 0)
        {
            playerTurn();
            SceneManager.LoadScene(PlayerProgressManager.instance.worldName);
        }
        if (enemy.GetComponent<CombatData>().currHealth <= 0)
        {
            playerTurn();
            CombatManager.Instance.OnExitBattle();
            SceneManager.LoadScene(PlayerProgressManager.instance.worldName);
        }

        print(enemyTurn);
        // Switch Turn
        if (enemyTurn)
        {
            StartCoroutine(enemyAction());
        }
        else
        {
            player.GetComponent<CombatData>().UpdateTurnCounts();
            enemy.GetComponent<CombatData>().UpdateTurnCounts();
            playerTurn();
        }
    }

}
