using UnityEngine;

public class CameraDragRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;
    public bool invertY = false; // Check this in the inspector if up/down feels backwards

    private float pitch = 0f;
    private float yaw = 0f;

    void Start()
    {
        // Capture the camera's starting rotation
        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;
    }

    void LateUpdate()
    {
        // 0 is the Left Mouse Button. Change to 1 if you prefer Right-Click to drag.
        if (Input.GetMouseButton(0))
        {
            // Read the mouse movement
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            yaw += mouseX * rotationSpeed;

            if (invertY)
                pitch += mouseY * rotationSpeed;
            else
                pitch -= mouseY * rotationSpeed;

            // Clamp the pitch so the camera doesn't flip completely upside down
            pitch = Mathf.Clamp(pitch, -90f, 90f);

            // Apply the new rotation to the camera
            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }
    }
}