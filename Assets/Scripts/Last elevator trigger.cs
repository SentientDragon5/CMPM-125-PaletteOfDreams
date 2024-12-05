using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lastelevatortrigger : MonoBehaviour
{
    void OnTriggerEnter()
    {
        CombatManager.Instance.OnExitBattle();
        // end game
        GameObject g = new GameObject("WIN"); // Rename to WIN for win
        print("died, created object");
        DontDestroyOnLoad(g);
        SceneManager.LoadScene("MainMenu");
        // done
    }
}
