using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TreePlacer : MonoBehaviour
{
    public string filter;
    public Transform fromParent;
    public Transform toParent;
    public List<Vector3> points;
    public List<Quaternion> rotations; // Use Quaternions for rotations

    public GameObject tree;

    [ContextMenu("Recreate")]
    void Recreate()
    {
        DestroyAllChildren(toParent);
        Create();
    }
    
    [ContextMenu("DestroyOriginals")]
    void DestroyOriginals()
    {
        DestroyAllChildren(fromParent);
    }

    [ContextMenu("Create")]
    void Create()
    {
        if (points.Count != rotations.Count)
        {
            Debug.LogError("Error: The number of points and rotations must be the same.");
            return;
        }

        for (int i = 0; i < points.Count; i++)
        {
            GameObject g = (GameObject)PrefabUtility.InstantiatePrefab(tree);
            g.transform.parent = toParent;
            g.transform.position = points[i];
            g.transform.rotation = rotations[i]; 
        }
    }

    [ContextMenu("Get Points and Rotations")]
    void GetPointsAndRotations()
    {
        points.Clear();
        rotations.Clear(); 
        for (int i = 0; i < fromParent.childCount; i++)
        {
            if(fromParent.GetChild(i).gameObject.name.Contains(filter)){
                points.Add(fromParent.GetChild(i).position);
                rotations.Add(fromParent.GetChild(i).rotation); // Get rotations as Quaternions
            }
        }
    }
    
    public void DestroyAllChildren(Transform t)
    {
        while (t.childCount > 0)
        {
            DestroyImmediate(t.GetChild(0).gameObject);
        }
    }
}