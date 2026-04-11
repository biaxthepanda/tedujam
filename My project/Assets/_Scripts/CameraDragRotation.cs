using UnityEngine;

public class CameraDragRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;
    public bool invertY = false;

    // 1. We declare our independent Vector3 exactly as the docs recommend
    private Vector3 currentEulerAngles;

    void Start()
    {
        // Capture the starting angles
        currentEulerAngles = transform.localEulerAngles;

        // Fix Unity's 360-degree wrapping so our clamp doesn't break on start
        if (currentEulerAngles.x > 180f)
        {
            currentEulerAngles.x -= 360f;
        }
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // 2. All rotational changes happen ONLY to our independent Vector3
            currentEulerAngles.y += mouseX * rotationSpeed;

            if (invertY)
                currentEulerAngles.x += mouseY * rotationSpeed;
            else
                currentEulerAngles.x -= mouseY * rotationSpeed;

            // 3. Clamp the X (pitch) to prevent looking past straight up/down
            currentEulerAngles.x = Mathf.Clamp(currentEulerAngles.x, -89f, 89f);

            // 4. Force Z to absolute 0 to guarantee no roll
            currentEulerAngles.z = 0f;

            // 5. Apply the clean Vector3 to the camera
            transform.localEulerAngles = currentEulerAngles;
        }
    }
}