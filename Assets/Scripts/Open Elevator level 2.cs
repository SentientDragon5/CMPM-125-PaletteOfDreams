using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenElevatorlevel2 : MonoBehaviour
{
    public Animator anim;
    public static int keys;
    public int keysReq;

    // Start is called before the first frame update
    void Start()
    {
        anim.enabled = false;
    }


    void OnTriggerEnter(Collider other)
    {
        if (keys >= keysReq)
        {
            anim.enabled = true;
            anim.Play("Elevator Door Animation");
        }
    }
}
