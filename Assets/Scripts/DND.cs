using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DND : MonoBehaviour
{
    private void Awake()
    {
        var name = gameObject.name;
        gameObject.name = "dummy";
        var obs = GameObject.Find(name);
        if (obs != null)
        {
            print("object exists, destroying " + name);
            Destroy(gameObject);
        }
        else
        {
            print("Making indstance "+name);
            gameObject.name = name;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
    }
}
