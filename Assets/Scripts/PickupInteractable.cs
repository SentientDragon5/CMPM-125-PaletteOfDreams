using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteractable : Interactable
{
    public string itemName = "Item";
    public override void Interact(PlayerCharacter interactor)
    {
        base.Interact(interactor);
        // add item to inventory
    }
}
