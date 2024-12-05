using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Linq;

public enum AIState
{
    Chase,
    Patrol,
    Stand,
    DoNothing
}

public class BattleInteractable : Interactable
{
    [Header("SET ID TO BE UNIQUE")]
    public int myId = -1;
    
    
    private NavMeshAgent agent;
    [Header("State")]
    [SerializeField] private AIState state;
    [SerializeField] private AIState goalState;
    public AIState State { get => state; set => goalState = value; }

    [Header("Targets")]
    [SerializeField] private List<Transform> targets;
    [SerializeField] private Transform targetActor;
    public Transform Target
    {
        get
        {
            if (targetActor != null)
                return targetActor;
            if (targets.Count > 0)
            {
                targetActor = targets[0];
                return targetActor;
            }
            return null;
        }
        set => targetActor = value;
    }
    
    public LayerMask enviromentLayer;
    public LayerMask characterLayer;

    [Header("Chase")]
    [SerializeField] private float eyeHeight = 1.5f;
    private Vector3 Eye_ls => transform.up * eyeHeight;
    [SerializeField] private float lookRadius = 5;
    [SerializeField] private float lookDotMin = 0.8f;
    [SerializeField] private float chaseRadius = 8;
    [SerializeField] private float attackRadius = 2;
    [SerializeField] private float stoppingDistance = 0.1f;


    [Header("Patrol")]
    [SerializeField] private PatrolPath path;
    int currentPathIndex = 0;
    bool OnPath;
    [SerializeField] private float onPathThreshold = 1f;

    [Header("Stand")]
    [SerializeField] private Vector3 goalPosition;
    [SerializeField] private Quaternion goalRotation;
    public Vector3 GoalPosition { get => goalPosition; set => goalPosition = value; }
    public Quaternion GoalRotation { get => goalRotation; set => goalRotation = value; }

    
    public string sceneName = "Combat";

    private void Start()
    {
        if (myId <= 0)
            Debug.LogError("ID for enemy is likely not assigned!");
        agent = GetComponent<NavMeshAgent>();
        if (PlayerProgressManager.instance.enemiesDefeated.Contains(myId))
        {
            gameObject.SetActive(false);
        }
    }

    public override void Interact(PlayerCharacter interactor)
    {
        base.Interact(interactor);
        // save position, world parameters
        // save battle parameters
        // load new scene
        PlayerProgressManager.instance.worldPosition = interactor.transform.position;
        PlayerProgressManager.instance.worldEuler = interactor.transform.eulerAngles;
        PlayerProgressManager.instance.worldName = SceneManager.GetActiveScene().name;
        PlayerProgressManager.instance.enemiesDefeated.Add(myId);

        SceneManager.LoadScene(sceneName);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
        if(collision.gameObject.TryGetComponent(out PlayerCharacter interactor))
        {
            Interact(interactor);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.TryGetComponent(out PlayerCharacter interactor))
        {
            Interact(interactor);
        }
    }

    void Attack()
    {
        // should be in range and the trigger will enter.
    }
    
    private void FixedUpdate()
    {
        targets = FindVisibleEnemies();

        if (Target != null)
        {
            var otherRadius = Target.GetComponent<CharacterController>().radius;
            var myRadius = GetComponent<CapsuleCollider>().radius;
            var sd = otherRadius + myRadius + stoppingDistance;
            agent.stoppingDistance = sd;

            float d = Vector3.Distance(Target.transform.position, transform.position);
            if (d < chaseRadius && d > sd)
            {
                agent.SetDestination(Target.transform.position);
                state = AIState.Chase;
                if (d < attackRadius)
                {
                    Attack();
                }
                return;
            }
        }

        if (path != null)
        {
            state = AIState.Patrol;
            return;
        }

        if (goalState == AIState.Stand)
        {
            state = AIState.Stand;
            return;
        }

        state = AIState.DoNothing;
    }

    void Update()
    {
        if (state == AIState.Patrol)
            FollowPath();
    }

    public bool showGizmos = false;
    private void OnDrawGizmos()
    {
        //if (Target == null)
        //    return;

        //FindVisiblity(Target.transform, out float d, out float dot, out Ray ray, out bool see);
        FindVisibleEnemies(showGizmos);

        if (path != null && showGizmos)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawCube(path.NearestPoint(transform.position, out int index), Vector3.one * 0.2f);
            Gizmos.color = Color.green;
            Gizmos.DrawCube(path.Subdivided[nextPathIndex], Vector3.one * 0.2f);
        }
    }

    public List<Transform> FindVisibleEnemies(bool gizmos = false)
    {
        List<Transform> near = FindNearEnemies(lookRadius);
        List<Transform> o = new List<Transform>();
        foreach (Transform t in near)
        {
            float distance = Vector3.Distance(t.transform.position, transform.position + Eye_ls);
            float dot = Vector3.Dot((t.transform.position - (transform.position+ Eye_ls)).normalized, transform.forward);
            Ray ray = new Ray(transform.position + Eye_ls, ((t.transform.position + t.transform.up) - (transform.position + Eye_ls)).normalized * lookRadius);
            bool lineOfSight = Physics.Raycast(ray, lookRadius);

            if (gizmos)
            {
                Gizmos.color = distance < lookRadius && dot > lookDotMin ? Color.green : Color.red;
                Gizmos.DrawWireSphere(transform.position + Eye_ls, lookRadius);
                Gizmos.color = lineOfSight ? Color.green : Color.red;
                Gizmos.DrawRay(transform.position + Eye_ls, ((t.transform.position + t.transform.up) - (transform.position + transform.up)).normalized * lookRadius);

                Gizmos.color = distance < chaseRadius ? Color.green : Color.red;
                Gizmos.DrawWireSphere(transform.position + Eye_ls, chaseRadius);
            }

            //TODO: fix the raycast. it is somehow looking through walls. check the raycasthit
            if (distance < lookRadius && dot > lookDotMin && lineOfSight)
                o.Add(t);
        }
        if(near.Count<1 && gizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Eye_ls, lookRadius);
            Gizmos.DrawWireSphere(transform.position + Eye_ls, chaseRadius);

            Gizmos.DrawRay(transform.position + Eye_ls, transform.forward * lookRadius);
            float angle = Mathf.Acos(lookDotMin)*Mathf.Rad2Deg;
            Gizmos.DrawRay(transform.position + Eye_ls, Quaternion.AngleAxis(angle, transform.up) * transform.forward * lookRadius);
            Gizmos.DrawRay(transform.position + Eye_ls, Quaternion.AngleAxis(-angle, transform.up) * transform.forward * lookRadius);
            Gizmos.DrawRay(transform.position + Eye_ls, Quaternion.AngleAxis(angle, transform.right) * transform.forward * lookRadius);
            Gizmos.DrawRay(transform.position + Eye_ls, Quaternion.AngleAxis(-angle, transform.right) * transform.forward * lookRadius);

        }
        return o.OrderBy(i => Vector3.Distance(transform.position, i.position)).ToList();
    }


    int nextPathIndex
    {
        get
        {
            int o = currentPathIndex + 1;
            if (o >= path.Subdivided.Count)
            {
                return 0;
            }
            return o;
        }
    }
    public void FollowPath()
    {
        if (path == null)
            return;

        int index = -2;
        Vector3 nearestPoint = path.NearestPoint(transform.position, out index);
        OnPath = Vector3.Distance(nearestPoint, transform.position) < onPathThreshold;
        Debug.DrawLine(nearestPoint, transform.position, Color.red);
        //Debug.Log(Vector3.Distance(path.NearestPoint(transform.position), transform.position));
        if (!OnPath)//Off path
        {
            //print("return " + Vector3.Distance(nearestPoint, transform.position) + " " + index);
            currentPathIndex = index;
            //go to path
            //find nearest point on path.
            agent.SetDestination(nearestPoint);
        }
        else
        {
            //GO to the next point
            //print(currentPathIndex + " " + nextPathIndex + " " + path.Subdivided.Count + " " + Vector3.Distance(transform.position, path.Subdivided[nextPathIndex]));// path.Subdivided[currentPathIndex]
            if (Vector3.Distance(transform.position, path.Subdivided[nextPathIndex]) < onPathThreshold)
            {
                //print("AT SPot");
                //Is at point
                currentPathIndex = nextPathIndex;
            }
            else
            {
                //print("Go" + Vector3.Distance(path.Subdivided[nextPathIndex], transform.position));
                //Go Closer
                agent.SetDestination(path.Subdivided[nextPathIndex]);
            }
        }
    }
    public List<Transform> FindNearEnemies(float radius = 5)
    {
        List<Transform> o = new List<Transform>();


        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in hits)
            if (collider.TryGetComponent(out PlayerCharacter a))
                o.Add(a.transform);

        List<Transform> priority = o.OrderBy(i => Vector3.Distance(transform.position, i.position)).ToList();
        return priority;
    }

    public void StandGuard()
    {
        if (Vector3.Distance(transform.position, goalPosition) > agent.stoppingDistance)
            agent.SetDestination(goalPosition);
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, goalRotation, 90);
        }
    }
}
