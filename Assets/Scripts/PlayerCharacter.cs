using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerCharacter : MonoBehaviour
{
    private Animator Animator;
    private CharacterController characterController;
    public static bool hasKey = false;

    protected virtual void Awake()
    {
        Debug.Assert(TryGetComponent(out Animator));
        Debug.Assert(TryGetComponent(out characterController));
        playerInput = GameObject.Find("Input").GetComponent<PlayerInput>();
    }

    #region aliases
    public PlayerInput playerInput;
    public InputAction GetAct(string n) => playerInput.actions[n];
    public bool GetDown(string n) => GetAct(n).WasPressedThisFrame();
    public bool GetUp(string n) => GetAct(n).WasReleasedThisFrame();
    public bool GetPressed(string n) => GetAct(n).IsPressed();
    public Vector2 GetV2(string n) => GetAct(n).ReadValue<Vector2>();

    public bool IsAnim(string name) => Animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    public bool IsAnim(string name, int layer) => Animator.GetCurrentAnimatorStateInfo(layer).IsName(name);
    #endregion

    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float turnSpeed = 360;
    [SerializeField, Min(0)] private float ySnapSpeed = 1;
    [Header("Raycasting")]
    public LayerMask enviromentLayer = 1;

    [Header("Interactor")]
    [SerializeField] private float interactionRadius = 1.5f;
    //public Vector3 offset = Vector3.zero;
    [SerializeField] private Vector3 offset = Vector3.up;

    [SerializeField] private bool lookAtTarget = true;
    public bool CanLookAtTarget { get => lookAtTarget; set => lookAtTarget = value; }
    float lookWeight = 0;
    Vector3 target;
    [SerializeField] private float lookSmoothRate = 5f;
    float maxDot = 0.2f;
    [SerializeField] private AnimationCurve lookCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public LayerMask interactableLayers = 8 << 1;

    [Header("Current Nearby Interactables")]
    [SerializeField] private List<Interactable> interactionQueue = new List<Interactable>();

    float upOffset = 0.5f;

    #region instance variables
    Vector3 GroundNormal;
    Vector3 moveInput;
    float TurnAmount;
    float ForwardAmount;
    float RightAmount;
    Vector3 velocity;
    #endregion


    #region properies
    public Vector3 CharacterVelocity { get => characterController.velocity; }
    public bool Sprinting { get { return GetPressed("Sprint") && ForwardAmount > 0.1f; } }
    #endregion

    bool GroundCast(float downDist, out RaycastHit hit) => Physics.SphereCast(transform.position + Vector3.up * upOffset, characterController.radius, Vector3.down, out hit, downDist, enviromentLayer);
    bool GroundCast(float downDist) => GroundCast(downDist, out RaycastHit hit);


    void Start()
    {
        PlayerProgressManager.instance.onPreSave.AddListener(Save);
        PlayerProgressManager.instance.onLoad.AddListener(Load);
        Load();
    }
    void Save()
    {
        PlayerProgressManager.instance.worldPosition = transform.position;
        PlayerProgressManager.instance.worldEuler = transform.eulerAngles;
        PlayerProgressManager.instance.worldName = SceneManager.GetActiveScene().name;
    }
    void Load()
    {
        transform.position = PlayerProgressManager.instance.worldPosition;
        transform.eulerAngles = PlayerProgressManager.instance.worldEuler;
    }

    // private void OnEnable()
    // {
    //     playerInput = GameObject.Find("Input").GetComponent<PlayerInput>();
    //     GetAct("Interact").performed += _ => Interact();
    // }

    // [ContextMenu("CHECK")]
    // void Check()
    // {
    //     Debug.Log(GetAct("Interact"));
    // }
    // public void OnDisable()
    // {
    //     if (playerInput != null)
    //     {
    //         GetAct("Interact").performed -= _ => Interact();
    //     }
    // }

    /// <summary>
    /// Call this to update the interaction Queue.
    /// </summary>
    public void CheckForInteractables()
    {
        if (gameObject == null) return;
        Collider[] colliders = Physics.OverlapSphere(transform.position + offset, interactionRadius, interactableLayers);
        interactionQueue.Clear();
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Interactable interactable) && interactable.IsActive)
            {
                if (interactable.transform != transform)
                {
                    interactionQueue.Add(interactable);
                }
                //interactable.show = true;
            }
        }
        interactionQueue = interactionQueue.OrderBy(i => Vector3.Distance(this.transform.position, i.transform.position)).ToList();//using Linq
    }
    /// <summary>
    /// Call this to interact with the nearest object.
    /// </summary>
    public void Interact()
    {
        if (gameObject == null) return;
        CheckForInteractables();

        if (interactionQueue.Count > 0)
        {
            AdjustThenInteract();
        }
    }

    void AdjustThenInteract()
    {
        Vector3 dir = (interactionQueue[0].transform.position - transform.position);
        Quaternion newRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));//Or else the player will be laying down or something random.
        transform.rotation = newRot;
        interactionQueue[0].Interact(this);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(offset + transform.position, interactionRadius);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        CheckForInteractables();
        bool anyToLookAt = false;
        for (int i = interactionQueue.Count - 1; i >= 0; i--)
        {
            if (interactionQueue[i].LookAt)
            {
                target = interactionQueue[i].transform.position;
                anyToLookAt = true;
            }
        }

        if (lookAtTarget && (anyToLookAt))
        {
            Vector3 head = Animator.GetBoneTransform(HumanBodyBones.Head).position;
            Vector3 directionOfInteractor = transform.forward;
            Vector3 directionFromTargetToInteractor = transform.position - target;
            bool facingTarget = Vector3.Dot(directionOfInteractor.normalized, directionFromTargetToInteractor.normalized) < maxDot;
            Debug.DrawRay(head, target - head, facingTarget ? Color.green : Color.magenta);
            if (facingTarget)
            {
                Animator.SetLookAtPosition(target);
                if (lookWeight < 1)
                    lookWeight += lookSmoothRate * Time.deltaTime;

            }
            else
            {
                if (lookWeight > 0)
                    lookWeight -= lookSmoothRate * Time.deltaTime;
            }
        }
        else
        {
            if (lookWeight > 0)
                lookWeight -= lookSmoothRate * Time.deltaTime;
        }

        Animator.SetLookAtWeight(lookCurve.Evaluate(lookWeight), lookCurve.Evaluate(lookWeight) * 0.1f, 1, 0, 0.8f);
        lookWeight = Mathf.Clamp01(lookWeight);
    }

    private void Update()
    {
        Vector2 m = GetAct("Move").ReadValue<Vector2>();
        m.x = Mathf.Clamp(m.x, -1, 1);
        m.y = Mathf.Clamp(m.y, -1, 1);

        if (!inAir)
        {

            Move(m);

            if (GetAct("Interact").triggered)
            {
                Interact();
            }
        }

    }

    public void Move(Vector2 input)
    {
        Transform camTransform = Camera.main.transform;

        Vector3 camForward = new Vector3(camTransform.forward.x, 0, camTransform.forward.z).normalized;
        Vector3 move = input.y * camForward + input.x * camTransform.right;

        if (move.magnitude > 1f) move.Normalize();
        moveInput = move;
        move = transform.InverseTransformDirection(move);

        move = Vector3.ProjectOnPlane(move, GroundNormal);
        TurnAmount = Mathf.Atan2(move.x, move.z);
        ForwardAmount = move.z;
        RightAmount = move.x;

        velocity.y = -1 * ySnapSpeed;

        UpdateAnimatior(move, GetPressed("Sprint"));
        transform.Rotate(0, TurnAmount * turnSpeed * Time.deltaTime, 0);
    }
    public float sprintMultiplier = 1.5f;
    private void OnAnimatorMove()
    {
        if (inAir) return;

        Vector3 move = Animator.deltaPosition * moveSpeed;

        if (GroundCast(1f, out RaycastHit hit))
        {
            move = Vector3.ProjectOnPlane(move, hit.normal);
            transform.position = hit.point;
        }
        //move *= moveSpeed;
        move = (velocity + moveInput * moveSpeed) * (Sprinting ? sprintMultiplier : 1);

        characterController.transform.rotation *= Animator.deltaRotation;
        move += velocity.y * Vector3.up;
        characterController.Move(move * Time.deltaTime);

    }

    public void FootR() { }
    public void FootL() { }
    public void Land() { }

    void UpdateAnimatior(Vector3 move, bool run)
    {
        float animForward = run && ForwardAmount > 0.1f ? 2 : ForwardAmount;
        Animator.SetFloat("Forward", animForward, 0.1f, Time.deltaTime);
        //Animator.SetFloat("Right", RightAmount, 0.1f, Time.deltaTime);
        //Animator.SetFloat("Turn", TurnAmount, 0.1f, Time.deltaTime);
        //Animator.SetBool("OnGround", true);

        //if (Grounded && move.magnitude > 0.1f && !(IsAnim("Grounded") || IsAnim("Crouching") || IsAnim("Strafing"))) Animator.CrossFade("Grounded", 0f, 0);
    }

    private bool inAir;

    public void JumpTo(JumpPoint jumpPoint)
    {
        Animator.CrossFade("Jumping", 0.1f, 0);
        StartCoroutine(Jump(jumpPoint));
    }

    public AnimationCurve jumpCurve; // Assign your curve in the inspector
    public float jumpSpeed = 5f; // Adjust jump speed as needed

    IEnumerator Jump(JumpPoint jumpPoint)
    {
        inAir = true;
        Vector3 startPos = transform.position;
        Vector3 targetPos = jumpPoint.transform.position;
        float distance = Vector3.Distance(startPos, targetPos);
        float timeElapsed = 0f;

        print(startPos + " " + targetPos);
        float t = 0;
        while (t < 1f)
        {
            timeElapsed += Time.deltaTime;
            t = timeElapsed * jumpSpeed / distance; // Calculate t based on speed

            // Apply the curve to the Y position
            float curveY = jumpCurve.Evaluate(t);
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            currentPos.y += curveY;

            characterController.Move(currentPos - transform.position);
            print(currentPos + " " + t);
            yield return new WaitForFixedUpdate();
        }
        transform.position = targetPos;
        inAir = false;
        Animator.CrossFade("Walking", 0.1f, 0);
    }
}

