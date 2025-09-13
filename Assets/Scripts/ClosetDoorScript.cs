using UnityEngine;

public class ClosetDoorScript : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy itself if it collides with something
        Destroy(gameObject);
    }
}
