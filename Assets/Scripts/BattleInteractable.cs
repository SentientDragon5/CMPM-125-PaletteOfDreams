using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleInteractable : Interactable
{
    public string sceneName = "Combat";
    public int myId = -1;

    private void Start()
    {
        if (PlayerProgressManager.instance.enemiesDefeated.Contains(myId))
        {
            gameObject.SetActive(false);
        }
    }

    public override void Interact(PlayerCharacter interactor)
    {
        base.Interact(interactor);
        // save position, world parameters
        // save battle parameters
        // load new scene
        PlayerProgressManager.instance.worldPosition = interactor.transform.position;
        PlayerProgressManager.instance.worldEuler = interactor.transform.eulerAngles;
        PlayerProgressManager.instance.worldName = SceneManager.GetActiveScene().name;
        PlayerProgressManager.instance.enemiesDefeated.Add(myId);

        SceneManager.LoadScene(sceneName);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out PlayerCharacter interactor))
        {
            Interact(interactor);
        }
    }
}
