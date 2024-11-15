using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenElevator : MonoBehaviour
{
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        anim.enabled = true;
        anim.Play("Elevator Door Animation");
    }
}
