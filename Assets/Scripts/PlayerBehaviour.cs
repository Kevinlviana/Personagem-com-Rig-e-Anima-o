using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] CharacterController playerCC;
    [SerializeField] Animator animator;


    [SerializeField] float speed = 3f;
    [SerializeField] float turnSpeed = 720f;     
    [SerializeField] bool snapTo8Directions = true;

  
    [SerializeField] float jumpHeight = 1.2f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float coyoteTime = 0.1f;      
    [SerializeField] float jumpBufferTime = 0.15f; 

    Vector2 moveInput;

    bool emoting;

    float verticalSpeed;
    float lastGroundedTime;
    float jumpBufferedUntil;

    InputAction jumpAction;

    readonly HashSet<string> animatorParams = new HashSet<string>();

    void Awake()
    {
        if (playerCC == null) playerCC = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        foreach (var p in animator.parameters) animatorParams.Add(p.name);
       
    }

    void Start()
    {
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            jumpAction = playerInput.actions.FindAction("Player/Jump");
            if (jumpAction != null) jumpAction.performed += OnJump;
        }
    }

    void OnDestroy()
    {
        if (jumpAction != null) jumpAction.performed -= OnJump;
    }


    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 raw = ctx.ReadValue<Vector2>();
        moveInput = snapTo8Directions ? SnapTo8(raw) : raw.normalized;
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        emoting = ctx.ReadValueAsButton();
        SetBool("IsEmoting", emoting);
        animator.applyRootMotion = emoting;

        if (ctx.canceled)
        {
            animator.transform.position = transform.position;
            animator.transform.rotation = transform.rotation;
        }
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        jumpBufferedUntil = Time.time + jumpBufferTime;
    }

    static Vector2 SnapTo8(Vector2 v)
    {
        if (v.sqrMagnitude < 0.04f) return Vector2.zero;
        float step = Mathf.PI / 4f;
        float angle = Mathf.Round(Mathf.Atan2(v.y, v.x) / step) * step;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    void SetBool(string name, bool value)
    {
        if (animatorParams.Contains(name)) animator.SetBool(name, value);
    }


    void Update()
    {
        if (jumpAction == null)
        {
            var kb = Keyboard.current;
            var pad = Gamepad.current;
            if ((kb != null && kb.spaceKey.wasPressedThisFrame) || (pad != null && pad.buttonSouth.wasPressedThisFrame))
                jumpBufferedUntil = Time.time + jumpBufferTime;
        }

   
        bool grounded = playerCC.isGrounded;
        if (grounded)
        {
            lastGroundedTime = Time.time;
            if (verticalSpeed < 0f) verticalSpeed = -1f;
        }

        bool canJump = Time.time - lastGroundedTime <= coyoteTime;
        if (jumpBufferedUntil > Time.time && canJump && !emoting)
        {
            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferedUntil = 0f;
            lastGroundedTime = -10f;
        }

        if (!grounded || verticalSpeed > 0f)
            verticalSpeed += gravity * Time.deltaTime;

        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 horizontal = emoting ? Vector3.zero : direction * speed;
        playerCC.Move((horizontal + Vector3.up * verticalSpeed) * Time.deltaTime);

        if (!emoting && direction != Vector3.zero)
        {
            Quaternion target = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
        }

        bool airborne = verticalSpeed > 0.1f || (!grounded && Time.time - lastGroundedTime > 0.1f);
        SetBool("IsGrounded", !airborne);
        SetBool("IsWalking", !emoting && direction != Vector3.zero);
    }
}
