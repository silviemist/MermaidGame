using UnityEngine;
using UnityEngine.InputSystem;

public class ProximityInteractable : MonoBehaviour
{
    bool playerNearby = false;

    // buffer the E press for a short moment
    public float interactBufferTime = 0.15f;
    float interactBufferTimer = 0f;

    void Update()
    {
        // record E press
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            interactBufferTimer = interactBufferTime;
        }

        // tick down buffer
        if (interactBufferTimer > 0f)
            interactBufferTimer -= Time.deltaTime;

        // if player is nearby and we have a buffered press, interact
        if (playerNearby && interactBufferTimer > 0f)
        {
            interactBufferTimer = 0f;
            Interact();
        }
    }

    void Interact()
    {
        Debug.Log("Used " + gameObject.name);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            Debug.Log("Press E to interact");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}