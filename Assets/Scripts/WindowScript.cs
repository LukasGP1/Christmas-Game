using UnityEngine;

public class WindowScript : MonoBehaviour
{
    [SerializeField] public GameObject closetDoor;
    [SerializeField] public float dropInterval;
    private GameObject instantiatedDoor;
    private float timeSinceLastDrop;

    void Start()
    {
        // Reset the time
        timeSinceLastDrop = 0;
    }

    void FixedUpdate()
    {
        // If the time since the last drop is bigger or equal to the dropInterval, destroy the old door, instantiate a new one and reset the time
        if (timeSinceLastDrop >= dropInterval)
        {
            if (instantiatedDoor != null) Destroy(instantiatedDoor);
            instantiatedDoor = Instantiate(closetDoor, transform.position, new Quaternion());
            timeSinceLastDrop = 0;
        }

        // Increment the time
        timeSinceLastDrop += 0.02f;
    }
}
