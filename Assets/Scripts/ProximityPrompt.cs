using UnityEngine;
using UnityEngine.InputSystem;

public class ProximityPrompt : MonoBehaviour
{
    public string promptText = "Press E to interact";
    public float interactBufferTime = 0.15f;

    bool playerNearby = false;
    float bufferTimer = 0f;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            bufferTimer = interactBufferTime;

        if (bufferTimer > 0f) bufferTimer -= Time.deltaTime;

        if (playerNearby && bufferTimer > 0f)
        {
            bufferTimer = 0f;
            Interact();
        }
    }

    void Interact()
    {
        Debug.Log("Used " + gameObject.transform.parent.name);
        // later: open steering UI
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            Debug.Log(promptText);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}