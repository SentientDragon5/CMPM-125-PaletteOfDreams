using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenElevator : MonoBehaviour
{
    public Animator anim;
    public bool keyNeeded;

    // Start is called before the first frame update
    void Start()
    {
        anim.enabled = false;
    }


    void OnTriggerEnter(Collider other)
    {
        if (keyNeeded)
        {
            if (PlayerCharacter.hasKey)
            {
                anim.enabled = true;
                anim.Play("Elevator Door Animation");
            }
        }
        else
        {
            anim.enabled = true;
            anim.Play("Elevator Door Animation");
        }
    }
}
