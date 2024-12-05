using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPoint : Interactable
{
    public override void Interact(PlayerCharacter interactor)
    {
        base.Interact(interactor);
        interactor.JumpTo(this);
    }
}
