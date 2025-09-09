using UnityEngine;

public class FireScript : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    private int health;
    private Vector3 originalScale;
    public PlayerManagerScript playerManager;

    void Start()
    {
        // Get the player manager
        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManagerScript>();

        // Set the left health to the max health
        health = maxHealth;

        // Set the original scale to the scale of the fire
        originalScale = transform.GetChild(0).transform.localScale;
    }

    void Update()
    {
        // Destroy the fire if it has no health left
        if (health <= 0) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // If a water projectile collides with the fire
        if (collision.CompareTag("WaterProjectile"))
        {
            // Ask the player manager to destroy the projectile
            playerManager.DestroyProjectile(collision.gameObject.GetComponent<WaterProjectileMoveScript>().GetListIndex());

            // Decrease the health by 1
            health--;

            // Ask the player manager to play the fire extinguish sound
            playerManager.PlayFireExtinguishSound(transform.position);

            // Lower the scale of the fire sprite
            transform.GetChild(0).transform.localScale = new Vector3(originalScale.x / (maxHealth - health + 1), originalScale.y / (maxHealth - health + 1), originalScale.z);
        }

        // If the player collides with the fire, ask the player manager to make the player die
        else if (collision.CompareTag("Player")) playerManager.Die();
    }
}
