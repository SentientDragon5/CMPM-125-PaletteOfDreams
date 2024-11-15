using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteractable : Interactable
{
    public string itemName = "Item";
    public override void Interact(PlayerCharacter interactor)
    {
        base.Interact(interactor);
        Debug.Log("Key picked up");
        PlayerCharacter.hasKey = true;
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerCharacter interactor))
        {
            Interact(interactor);
        }
    }
}
