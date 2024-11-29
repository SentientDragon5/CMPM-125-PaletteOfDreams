using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonAction : MonoBehaviour
{
    [SerializeField] private Button loadButton;
    [SerializeField] private GameObject combatManagement;
    [SerializeField] private int attackID;
    // Start is called before the first frame update
    void Start()
    {
        loadButton.onClick.AddListener(taskOnClick);
    }


    void taskOnClick()
    {
        //combatManagement.GetComponent<CombatSystem>().playerAction(attackID);
    }
}
