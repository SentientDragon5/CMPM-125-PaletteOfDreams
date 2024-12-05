using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// A Component that shows a path that a character can patrol.
/// </summary>
public class PatrolPath : MonoBehaviour
{
    [SerializeField] private bool refreshEveryFrame = false;
    [Header("Subdivisions")]
    [SerializeField, Range(0, 10)] private float subdivisionsPerMeter = 2;
    [SerializeField, Range(0, 10)] private float nearestPointSubdivisions = 1;

    private List<Transform> patrolPoints = new List<Transform>();
    private List<Vector3> subdivided = new List<Vector3>();
    public List<Vector3> Subdivided { get => subdivided; }

    [ContextMenu("Refresh")]
    private void Awake()
    {
        patrolPoints = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            patrolPoints.Add(transform.GetChild(i));
        }
        subdivided = new List<Vector3>();
        if (subdivisionsPerMeter > 0)
        {
            for (int i = 0; i < patrolPoints.Count; i++)
            {
                Vector3 current = patrolPoints[i].position;
                int nextInt = i + 1;
                if (nextInt >= patrolPoints.Count) nextInt = 0;
                Vector3 next = patrolPoints[nextInt].position;

                float times = subdivisionsPerMeter * Vector3.Distance(current, next);
                for (int s = 0; s < times; s++)
                {
                    subdivided.Add(Vector3.Lerp(current, next, s / times));
                }
            }
        }
        else
        {
            for (int i = 0; i < patrolPoints.Count; i++)
            {
                subdivided.Add(patrolPoints[i].position);
            }
        }
    }
    private void OnValidate() { Awake(); }

    /// <summary>
    /// Will return the nearest point on the path to compare, index is index in subdivided
    /// </summary>
    /// <param name="compare">The point to compare to</param>
    /// <returns>the nearest point on the path</returns>
    public Vector3 NearestPoint(Vector3 compare, out int index)
    {
        index = -1;
        List<Vector3> sorted = subdivided.OrderBy(i => Vector3.Distance(i, compare)).ToList();//using Linq

        Vector3 left = sorted[0];
        Vector3 right = sorted[1];

        List<Vector3> nearPoints = new List<Vector3>();
        float iterations = nearestPointSubdivisions * Vector3.Distance(left, right);
        for (int i = 0; i < iterations; i++)
        {
            nearPoints.Add(Vector3.Lerp(left, right, i / iterations));
        }
        nearPoints = nearPoints.OrderBy(i => Vector3.Distance(i, compare)).ToList();//using Linq

        for (int i = 0; i < subdivided.Count; i++)
        {
            if (subdivided[i] == sorted[0])
            {
                index = i;
            }
        }

        return nearPoints[0];
    }

    #region Debug
    [Header("Debug")]
    [Tooltip("Whether to show the subdivided path")]
    [SerializeField] bool showSubdivisions = false;
    [Tooltip("What object to find the nearest point on the path to")]
    [SerializeField] Transform compareObj;
    Vector3 compare { get { return compareObj.position; } }

    private void OnDrawGizmos()
    {
        if (refreshEveryFrame)
            Awake();
        Gizmos.color = Color.cyan;
        for (int i = 0; i < patrolPoints.Count; i++)
        {
            Gizmos.DrawWireSphere(patrolPoints[i].position, 0.1f);
            int next = i + 1;
            if (next >= patrolPoints.Count) next = 0;
            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[next].position);
        }

        if (compareObj != null)
        {
            Gizmos.color = Color.red;
            List<Vector3> sorted = subdivided.OrderBy(i => Vector3.Distance(i, compare)).ToList();//using Linq

            Vector3 left = sorted[0];
            Vector3 right = sorted[1];
            Vector3 size = Vector3.one * 0.1f;
            Gizmos.DrawWireCube(left, size);
            Gizmos.DrawWireCube(right, size);
            Gizmos.DrawWireCube(compare, size);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(NearestPoint(compare, out int index), size);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (showSubdivisions)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < subdivided.Count; i++)
            {
                Gizmos.DrawWireSphere(subdivided[i], 0.1f);
                int next = i + 1;
                if (next >= patrolPoints.Count) next = 0;
            }
        }
    }
    #endregion
}