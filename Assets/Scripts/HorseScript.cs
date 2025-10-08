using UnityEngine;

public class HorseScript : MonoBehaviour
{
    public float floorWidth;
    public float moveSpeed;
    public BoxCollider2D solidBoxCollider;
    private float originalX;
    private MovingDirection movingDirection;
    private SpriteRenderer spriteRenderer;

    public enum MovingDirection { LEFT, RIGHT }

    void Start()
    {
        // Initialize moveDirection variable
        movingDirection = MovingDirection.RIGHT;

        // Get SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Save original x-Coordinate
        originalX = transform.position.x;
    }

    void Update()
    {
        // Flip the sprite if the horse is moving left
        spriteRenderer.flipX = movingDirection == MovingDirection.LEFT;

        // Make the collider ignore collisions with the player
        Physics2D.IgnoreCollision(solidBoxCollider, GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>());
    }

    void FixedUpdate()
    {
        // If the horse is moving to the right
        if (movingDirection == MovingDirection.RIGHT)
        {
            // If the horse would step of the platform in this step, change the moveDirection
            if (transform.position.x + moveSpeed * 0.02f >= originalX + floorWidth / 2) movingDirection = MovingDirection.LEFT;

            // Otherwise walk to the right
            else transform.position = new Vector3(transform.position.x + moveSpeed * 0.02f, transform.position.y, transform.position.z);
        }

        // Otherwise
        else
        {
            // If the horse would step of the platform in this step, change the moveDirection
            if (transform.position.x - moveSpeed * 0.02f <= originalX - floorWidth / 2) movingDirection = MovingDirection.RIGHT;

            // Otherwise walk to the right
            else transform.position = new Vector3(transform.position.x - moveSpeed * 0.02f, transform.position.y, transform.position.z);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManagerScript>().Die();
        }
    }

    public void SetFloorWidth(float value)
    {
        floorWidth = value;
    }
}
