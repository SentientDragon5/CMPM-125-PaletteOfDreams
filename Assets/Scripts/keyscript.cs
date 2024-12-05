using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyscript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        this.gameObject.SetActive(false);
        OpenElevatorlevel2.keys += 1;
    }
}
