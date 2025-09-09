using UnityEngine;

public class FloorMoveScript : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float maxDistance;
    private float originalYPos;
    private float minYPos;
    private float t;
    void Start()
    {
        // Save the original y position of the object in a variable
        originalYPos = transform.position.y;

        // Calculate the lowest position the object will have on its path
        minYPos = originalYPos - maxDistance;

        // Calculate a random phase offset depending on the x position, so Fartloading zones and fires have the same offset as their respective floors
        t = ((transform.position.x * 96349826) % (Mathf.PI * 2)) / speed;
    }

    void FixedUpdate()
    {
        // Calculate the new y position based on the time, original y pos, distance and speed
        float a = maxDistance / 2;
        float y = Mathf.Sin(t * 2 * Mathf.PI * speed) * a + a + minYPos;

        // Assign the calculated y position to the object
        transform.position = new Vector3(transform.position.x, y, transform.position.z);

        // Increase time variable (FixedUpdate() is called every 0.02 seconds, so the speed parameter means how many times the object bounces down and up in one second)
        t += 0.02f;
    }
}