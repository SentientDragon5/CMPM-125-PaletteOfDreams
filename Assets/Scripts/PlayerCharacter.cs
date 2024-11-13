using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{
    private Animator Animator;
    private CharacterController characterController;

    protected virtual void Awake()
    {
        Debug.Assert(TryGetComponent(out Animator));
        Debug.Assert(TryGetComponent(out characterController));
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

    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float turnSpeed = 180;
    [Header("Raycasting")]
    public LayerMask enviromentLayer = 1;

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

    bool GroundCast(float downDist, out RaycastHit hit) => Physics.Raycast(transform.position + Vector3.up * upOffset, Vector3.down, out hit, downDist, enviromentLayer);
    bool GroundCast(float downDist) => GroundCast(downDist, out RaycastHit hit);


    void Start()
    {
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

        UpdateAnimatior(move, GetPressed("Sprint"));
        transform.Rotate(0, TurnAmount * turnSpeed * Time.deltaTime, 0);
    }

    private void OnAnimatorMove()
    {
        Vector3 move = Animator.deltaPosition * moveSpeed;

        if (characterController.isGrounded && GroundCast(1f, out RaycastHit hit))
            move = Vector3.ProjectOnPlane(move, hit.normal);
        move = (velocity + moveInput * moveSpeed);

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
        Animator.SetFloat("Turn", TurnAmount, 0.1f, Time.deltaTime);
        Animator.SetBool("OnGround", true);

        //if (Grounded && move.magnitude > 0.1f && !(IsAnim("Grounded") || IsAnim("Crouching") || IsAnim("Strafing"))) Animator.CrossFade("Grounded", 0f, 0);
    }
}

