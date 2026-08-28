using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour, IPresser
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float rotationSmoothTime = 0.1f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Jump Assist")]
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    [Header("Camera")]
    public Transform cameraTransform;

    public GameObject AttackHitbox;
    private float attackCooldown = 0.5f;
    private bool isAttacking;
    private float lastAttackTime;

    private CharacterController controller;
    private InputSystem_Actions input;
    private Animator animator;

    private Vector2 moveInput;
    private bool sprinting;

    private float velocityY;
    private float rotationVelocity;

    private float coyoteCounter;
    private float jumpBufferCounter;

    // True once we've left the ground.
    private bool wasAirborne;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = new InputSystem_Actions();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        DisableAttackHitbox();
    }

    void OnEnable()
    {
        input.Enable();

        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += _ => moveInput = Vector2.zero;

        input.Player.Jump.performed += _ => jumpBufferCounter = jumpBufferTime;

        input.Player.Sprint.performed += _ => sprinting = true;
        input.Player.Sprint.canceled += _ => sprinting = false;
        input.Player.Attack.performed += _ => Attack();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        jumpBufferCounter -= Time.deltaTime;

        if (controller.isGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        HandleMovementAndGravity();
    }

    void HandleMovementAndGravity()
    {
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        bool isMoving = inputDir.magnitude > 0.1f;
        bool isRunning = sprinting && isMoving;

        Vector3 moveDir = Vector3.zero;

        if (isMoving)
        {
            float targetAngle =
                Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg +
                cameraTransform.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref rotationVelocity,
                rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        // Jump
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);

            jumpBufferCounter = 0f;
            coyoteCounter = 0f;

            animator.SetTrigger("OnJump");
        }

        // Gravity
        velocityY += gravity * Time.deltaTime;

        float speed = isRunning ? runSpeed : moveSpeed;

        Vector3 velocity = moveDir * speed;
        velocity.y = velocityY;

        controller.Move(velocity * Time.deltaTime);

        bool isGrounded = controller.isGrounded;

        if (!isGrounded)
        {
            wasAirborne = true;
        }

        // Landing
        if (wasAirborne && isGrounded)
        {
            wasAirborne = false;
            animator.SetTrigger("OnLand");
        }

        // Stick to the ground
        if (isGrounded && velocityY < 0f)
        {
            velocityY = -2f;
        }

        animator.SetBool("IsFalling", !isGrounded && velocityY < 0f);

        float currentSpeed = new Vector3(controller.velocity.x, 0f, controller.velocity.z).magnitude;
        animator.SetFloat("Speed", currentSpeed / runSpeed, 0.1f, Time.deltaTime);
    }

    void Attack()
    {
        if (isAttacking)
            return;

        if (Time.time < lastAttackTime + attackCooldown)
            return;

        isAttacking = true;
        lastAttackTime = Time.time;

        animator.SetTrigger("OnAttack");
        EnableAttackHitbox();
    }

    public void EnableAttackHitbox()
    {
        if (AttackHitbox != null)
            AttackHitbox.gameObject.SetActive(true);
    }

    public void DisableAttackHitbox()
    {
        if (AttackHitbox != null)
            AttackHitbox.gameObject.SetActive(false);
    }

    public void EndAttack()
    {
        DisableAttackHitbox();
        isAttacking = false;
    }
}