using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    public enum LookDirection { RIGHT, LEFT }

    [SerializeField] public float moveSpeed;
    [SerializeField] public float maxFartTime;
    [SerializeField] public Sprite normalSprite;
    [SerializeField] public Sprite fartingSprite;
    [SerializeField] public GameObject waterProjectile;
    [SerializeField] public float waterShootCooldown;
    [SerializeField] public float waterSlowDownFactor;
    [SerializeField] public int maxWaterShots;
    private Vector3 storedVelocity;
    private float secondsSinceLastWaterShot;
    private LookDirection lookDirection;
    private float fartTimeLeft;
    private Rigidbody2D myRigidbody;
    private float horizontalInput;
    private ParticleSystem myParticleSystem;
    private bool isFarting;
    private bool isInFartLoadingZone;
    private PlayerManagerScript playerManager;
    private bool canMove;
    private bool inWater;
    private int waterShotsLeft;

    void Start()
    {
        // Don't call the rest of the method if there is no player manager
        if (GameObject.FindGameObjectWithTag("PlayerManager") != null)
        {
            // Get the Rigidbody2D
            myRigidbody = GetComponent<Rigidbody2D>();

            // Get the particle system
            myParticleSystem = transform.GetComponent<ParticleSystem>();

            // Get the player manager
            playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManagerScript>();

            // Reset the variables
            isFarting = false;
            canMove = true;
            inWater = false;
            secondsSinceLastWaterShot = waterShootCooldown;
            fartTimeLeft = maxFartTime;
            lookDirection = LookDirection.RIGHT;
            waterShotsLeft = 0;
        }
    }

    void Update()
    {
        // Get the horizontal keyboard input
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // If the player can move, change the look direction based on the horizontal input (don't change it when there is no input)
        if (canMove)
        {
            if (horizontalInput > 0) lookDirection = LookDirection.RIGHT;
            else if (horizontalInput < 0) lookDirection = LookDirection.LEFT;
        }

        // Determine whether the player is farting based on the keyboard input, whether he has fart time left and whether he can move
        isFarting = Input.GetKey(KeyCode.Space) && fartTimeLeft > 0 && canMove;

        // Enable the emission of fart particles if the player is farting (and disable it if he isn't)
        var emission = myParticleSystem.emission;
        emission.enabled = isFarting;

        // Clamp the fart time between 0 and the maximum
        if (fartTimeLeft < 0) fartTimeLeft = 0;
        if (fartTimeLeft > maxFartTime) fartTimeLeft = maxFartTime;

        // Change the player sprite according to whether the player is farting
        if (isFarting) GetComponent<SpriteRenderer>().sprite = fartingSprite;
        else GetComponent<SpriteRenderer>().sprite = normalSprite;

        // Change whether the sprite is flipped, according to the looking direction
        GetComponent<SpriteRenderer>().flipX = lookDirection == LookDirection.LEFT;
    }

    void FixedUpdate()
    {
        // Increase the time since the last water shot (FixedUpdate() is called once every 0.02s, so the time is in seconds)
        secondsSinceLastWaterShot += 0.02f;

        // Set the x velocity based on the input, the movement speed and whether the player is in water
        if (canMove) myRigidbody.linearVelocity = new Vector2(horizontalInput * moveSpeed / (inWater ? waterSlowDownFactor : 1), myRigidbody.linearVelocity.y);

        // If the Player is farting lower the fart time left and plly upwards momentum (slow both of these down if the player is in water)
        if (isFarting)
        {
            fartTimeLeft -= 0.02f / (inWater ? waterSlowDownFactor : 1);
            myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, moveSpeed / (inWater ? waterSlowDownFactor : 1));
        }

        // If the player is not farting, is in a fart loading zone, has not the maximum fart time left and can move, increase the left fart time (Slow down if he is in water)
        else if (isInFartLoadingZone && fartTimeLeft < maxFartTime && canMove) fartTimeLeft += 0.02f / (inWater ? waterSlowDownFactor : 1);

        // If the player can move, the W key is pressed, more time than the cooldown has passed since the last shot and there are more than zero shots left
        if (canMove && Input.GetKey(KeyCode.W) && secondsSinceLastWaterShot >= waterShootCooldown && waterShotsLeft > 0)
        {
            // Instantiate a projectile at the player's position
            GameObject instantiatedWaterProjectile = Instantiate(waterProjectile, transform.position, new Quaternion());

            // Set the listIndex of the projectile to its future index in the projectile list
            instantiatedWaterProjectile.GetComponent<WaterProjectileMoveScript>().SetListIndex(playerManager.GetWaterProjectiles().Count);

            // Set the direction of the projectile to the look direction of the player
            instantiatedWaterProjectile.GetComponent<WaterProjectileMoveScript>().SetDirection(lookDirection);

            // Add the projectile to the player manager's list of projectiles
            playerManager.AddProjectile(instantiatedWaterProjectile);

            // Ask the player mananger to play the shoot sound
            playerManager.PlayWaterShootSound();

            // Reset the time since the last shot
            secondsSinceLastWaterShot = 0;

            // Decrease the water shots left by 1
            waterShotsLeft--;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // If the player enters a fart loading zone set the isInFartLoadingZone Variable to true
        if (collision.CompareTag("FartLoadingZone")) isInFartLoadingZone = true;

        // If the player enters a death plane or a closet door, ask the player manager to die
        else if (collision.CompareTag("DeathPlane")) playerManager.Die();

        // If the player enters a win zone, ask the player manager to win
        else if (collision.CompareTag("WinZone")) playerManager.LevelDone();

        // If the player enters water set the inWater Variable to true, reduce the velocity and the gravity and set the water shots left to the maximum
        else if (collision.CompareTag("Water"))
        {
            inWater = true;
            myRigidbody.linearVelocity /= waterSlowDownFactor;
            myRigidbody.gravityScale /= waterSlowDownFactor;
            waterShotsLeft = maxWaterShots;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // If the player collides with a closet door, ask the player manager to die
        if (collision.collider.CompareTag("ClosetDoor")) playerManager.Die();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // If the player exits a fart loading zone set the isInFartLoadingZone Variable to false
        if (collision.CompareTag("FartLoadingZone")) isInFartLoadingZone = false;

        // If the player exits water set the inWater Variable to false and increase the velocity and the gravity
        else if (collision.CompareTag("Water"))
        {
            inWater = false;
            myRigidbody.linearVelocity *= waterSlowDownFactor;
            myRigidbody.gravityScale *= waterSlowDownFactor;
        }
    }

    public void StopMovement()
    {
        // Set the canMove variable to false
        canMove = false;

        // Store the velocity
        storedVelocity = myRigidbody.linearVelocity;

        // Freeze the player
        myRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void StartMovement()
    {
        // Set the canMove variable to true
        canMove = true;

        // Load the stored velocity
        myRigidbody.linearVelocity = storedVelocity;

        // Unfreeze the player
        myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public float GetFartTimeLeft()
    {
        return fartTimeLeft;
    }

    public bool IsFarting()
    {
        return isFarting;
    }

    public int GetWaterShotsLeft()
    {
        return waterShotsLeft;
    }
}