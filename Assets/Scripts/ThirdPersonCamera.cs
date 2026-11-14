using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target & Positioning")]
    public Transform target;
    public Vector3 targetOffset = new Vector3(0f, 1.4f, 0f);

    [Header("Orbit Speeds & Sensitivity")]
    public float mouseSensitivityX = 180f;
    public float mouseSensitivityY = 120f;
    public float minPitch = -20f;
    public float maxPitch = 70f;

    [Header("Zoom Settings")]
    public float defaultDistance = 5.0f;
    public float minDistance = 2.0f;
    public float maxDistance = 9.0f;
    public float zoomSpeed = 4.0f;

    [Header("Collision & Smoothing")]
    public float followDamping = 12.0f;
    public float collisionRadius = 0.25f;
    public LayerMask collisionLayers = ~0;

    private float currentYaw = 0f;
    private float currentPitch = 20f;
    private float targetDistance;
    private float currentDistance;

    private void Start()
    {
        targetDistance = defaultDistance;
        currentDistance = defaultDistance;

        if (target == null)
        {
            var player = FindObjectOfType<PlayerController>();
            if (player != null) target = player.transform;
        }

        Vector3 angles = transform.eulerAngles;
        currentYaw = angles.y;
        currentPitch = angles.x;

        // Ignore triggers for camera collision
        collisionLayers &= ~(1 << 2); // Ignore Raycast
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            var player = FindObjectOfType<PlayerController>();
            if (player != null) target = player.transform;
            else return;
        }

        // Only rotate camera if game is unpaused
        if (GameManager.Instance == null || !GameManager.Instance.isGamePaused)
        {
            // Mouse look input
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            currentYaw += mouseX * mouseSensitivityX * Time.deltaTime;
            currentPitch -= mouseY * mouseSensitivityY * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

            // Zoom input
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                targetDistance = Mathf.Clamp(targetDistance - scroll * zoomSpeed, minDistance, maxDistance);
            }
        }

        // Calculate target camera rotation
        Quaternion targetRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        // Desired camera position before collision
        Vector3 focusPoint = target.position + targetOffset;
        Vector3 desiredDirection = targetRotation * -Vector3.forward;

        // Obstacle collision detection
        float finalDist = targetDistance;
        RaycastHit hit;
        if (Physics.SphereCast(focusPoint, collisionRadius, desiredDirection, out hit, targetDistance, collisionLayers, QueryTriggerInteraction.Ignore))
        {
            finalDist = Mathf.Clamp(hit.distance - 0.1f, minDistance, targetDistance);
        }

        currentDistance = Mathf.Lerp(currentDistance, finalDist, followDamping * Time.deltaTime);

        Vector3 finalPosition = focusPoint + desiredDirection * currentDistance;

        transform.position = finalPosition;
        transform.rotation = targetRotation;
    }
}
