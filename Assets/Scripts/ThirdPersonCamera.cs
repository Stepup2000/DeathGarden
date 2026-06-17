using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Rotation")]
    public float sensitivity = 2f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    [Header("Zoom")]
    public float minDistance = 2f;
    public float maxDistance = 8f;
    public float zoomSpeed = 0.1f;
    public float height = 2f;

    [Header("Collision")]
    public float collisionRadius = 0.3f;
    public float collisionSmoothSpeed = 10f;
    public LayerMask collisionMask;

    private InputSystem_Actions input;
    private Vector2 lookInput;

    private float yaw;
    private float pitch = 20f;

    private float currentDistance;
    private float targetDistance;

    void Awake()
    {
        input = new InputSystem_Actions();

        targetDistance = maxDistance;
        currentDistance = targetDistance;
    }

    void OnEnable()
    {
        input.Enable();

        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += _ => lookInput = Vector2.zero;
    }

    void OnDisable()
    {
        input.Disable();
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // Camera rotation
        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Mouse wheel zoom
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetDistance -= scroll * zoomSpeed;
            targetDistance = Mathf.Clamp(
                targetDistance,
                minDistance,
                maxDistance);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Point the camera rotates around
        Vector3 pivot = target.position + Vector3.up * height;

        // Desired camera position
        Vector3 desiredPosition =
            pivot + rotation * Vector3.back * targetDistance;

        float desiredDistance = targetDistance;

        // Camera collision
        Vector3 direction = (desiredPosition - pivot).normalized;

        if (Physics.SphereCast(
                pivot,
                collisionRadius,
                direction,
                out RaycastHit hit,
                targetDistance,
                collisionMask))
        {
            desiredDistance = Mathf.Max(
                minDistance,
                hit.distance - collisionRadius);
        }

        // Smooth movement
        currentDistance = Mathf.Lerp(
            currentDistance,
            desiredDistance,
            collisionSmoothSpeed * Time.deltaTime);

        Vector3 finalPosition =
            pivot + rotation * Vector3.back * currentDistance;

        transform.position = finalPosition;
        transform.rotation = rotation;
    }
}