using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCameraRMB : MonoBehaviour
{
    public Transform target;                 // Drag Player here
    public Vector3 targetOffset = new Vector3(0f, 1.6f, 0f);

    public float distance = 6f;
    public float minDistance = 2.5f;
    public float maxDistance = 10f;

    public float sensitivity = 0.15f;       // Mouse feel
    public float minPitch = -30f;
    public float maxPitch = 70f;

    public bool lockCursorWhileRotating = true;

    float yaw;
    float pitch;

    void Start()
    {
        if (target != null)
        {
            // Start from current camera angle so it doesn't snap weirdly
            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        bool rmbHeld = Mouse.current != null && Mouse.current.rightButton.isPressed;

        if (rmbHeld)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            yaw += delta.x * sensitivity;
            pitch -= delta.y * sensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            if (lockCursorWhileRotating)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        else if (lockCursorWhileRotating)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Optional zoom with mouse wheel
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                distance -= scroll * 0.01f;
                distance = Mathf.Clamp(distance, minDistance, maxDistance);
            }
        }

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 focusPoint = target.position + targetOffset;

        Vector3 camPos = focusPoint - (rot * Vector3.forward * distance);
        transform.position = camPos;
        transform.rotation = rot;
    }
}