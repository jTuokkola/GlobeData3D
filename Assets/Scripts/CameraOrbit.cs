using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;               // The object to orbit around
    public float distance = 15f;           // Starting distance from the object
    public float minDistance = 7f;         // Minimum zoom distance
    public float maxDistance = 20f;        // Maximum zoom distance
    public float zoomSpeed = 10f;          // Zoom speed (higher for more responsive zoom)
    public float rotationSpeed = 100f;     // Drag rotation speed (lower for smoothness)
    public float smoothTime = 0.1f;        // Time to smooth the movement
    public Vector2 rotationYLimit = new Vector2(-20, 80);  // Vertical rotation limits (min, max)

    private float currentX = 0f;           // Current horizontal angle
    private float currentY = 20f;          // Current vertical angle
    private float targetX, targetY;        // Target angles for smooth movement
    private Vector2 rotationVelocity;      // Used for smoothing rotation

    void Start()
    {
        targetX = currentX;
        targetY = currentY;
    }

    void Update()
    {
        // Right-click or hold down to rotate the camera
        if (Input.GetMouseButton(1))
        {
            targetX += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            targetY -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            targetY = Mathf.Clamp(targetY, rotationYLimit.x, rotationYLimit.y);  // Clamp the vertical rotation
        }

        // Smooth the rotation to avoid abrupt changes
        currentX = Mathf.SmoothDamp(currentX, targetX, ref rotationVelocity.x, smoothTime);
        currentY = Mathf.SmoothDamp(currentY, targetY, ref rotationVelocity.y, smoothTime);

        // Scroll to zoom in or out (make it more responsive)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);  // Clamp zoom distance
    }

    void LateUpdate()
    {
        // Camera position update based on smoothed angles and zoom distance
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = new Vector3(0, 0, -distance);  // Camera offset by distance
        transform.position = target.position + rotation * direction;  // Move camera to calculated position
        transform.LookAt(target);  // Keep looking at the target
    }
}
