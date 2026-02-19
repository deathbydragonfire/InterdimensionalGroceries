using UnityEngine;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform door; // The door object to rotate
    public float openAngle = 115f; // Rotation when open
    public float closeAngle = 0f;  // Rotation when closed
    public float speed = 2f;       // Swing speed

    [Header("Interaction Settings")]
    public string playerTag = "Player"; // Make sure your player has this tag
    public KeyCode interactKey = KeyCode.E;
    public Text promptText; // Optional UI text for "Press E"

    private bool isOpen = false;
    private bool playerNearby = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = Quaternion.Euler(door.localEulerAngles.x, closeAngle, door.localEulerAngles.z);
        openRotation = Quaternion.Euler(door.localEulerAngles.x, openAngle, door.localEulerAngles.z);

        if (promptText != null)
            promptText.enabled = false;
    }

    void Update()
    {
        // Swing the door
        if (isOpen)
            door.localRotation = Quaternion.Slerp(door.localRotation, openRotation, speed * Time.deltaTime);
        else
            door.localRotation = Quaternion.Slerp(door.localRotation, closedRotation, speed * Time.deltaTime);

        // Check for interaction
        if (playerNearby && Input.GetKeyDown(interactKey))
            ToggleDoor();

        // Update prompt
        if (promptText != null)
            promptText.enabled = playerNearby;
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
    }

    // Trigger detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            playerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            playerNearby = false;
    }
}
