using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleInteractable : Interactable
{
    public string sceneName = "Battle";
    public override void Interact(PlayerCharacter interactor)
    {
        base.Interact(interactor);
        // save position, world parameters
        // save battle parameters
        // load new scene
        SceneManager.LoadScene(sceneName);
    }
}
