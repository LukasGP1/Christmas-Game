using UnityEngine;

public class FireProjectileScript : MonoBehaviour
{
    public AudioClip guardDeath;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("GuardDeathPlane"))
        {
            AudioSource.PlayClipAtPoint(guardDeath, transform.position);
            collision.transform.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
