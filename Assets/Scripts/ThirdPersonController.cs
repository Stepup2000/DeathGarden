using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour, IPresser
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float rotationSmoothTime = 0.1f;
    public float runThreshold = 1;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Camera")]
    public Transform cameraTransform;

    private CharacterController controller;
    private InputSystem_Actions input;
    private Animator animator;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprinting;
    private bool jumpPressed;

    private float velocityY;
    private float rotationVelocity;

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
    }

    void OnEnable()
    {
        input.Enable();

        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += _ => moveInput = Vector2.zero;

        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += _ => lookInput = Vector2.zero;

        input.Player.Jump.performed += _ => jumpPressed = true;

        input.Player.Sprint.performed += _ => sprinting = true;
        input.Player.Sprint.canceled += _ => sprinting = false;
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        HandleMovement();
        HandleGravity();
    }

    void HandleMovement()
    {
        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        bool isMoving = inputDir.magnitude >= 0.1f;
        bool isRunning = sprinting && isMoving;

        if (isMoving)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward; float speed = isRunning ? runSpeed : moveSpeed;
            controller.Move(moveDir * speed * Time.deltaTime);
        }

        float currentSpeed = controller.velocity.magnitude;
        float animSpeed = currentSpeed / runSpeed; animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);
    }

    void HandleGravity()
    {
        if (controller.isGrounded && velocityY < 0)
            velocityY = -2f;

        if (jumpPressed && controller.isGrounded)
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;
        }

        velocityY += gravity * Time.deltaTime;
        controller.Move(Vector3.up * velocityY * Time.deltaTime);
    }
}