using UnityEngine;

public class WaterProjectileMoveScript : MonoBehaviour
{
    public float moveSpeed;
    public float maxTime;
    private PlayerManagerScript playerManager;
    private PlayerMoveScript.LookDirection direction = PlayerMoveScript.LookDirection.RIGHT;
    private int listIndex;
    private float time;

    void Start()
    {
        // Set the left time variable to the maxTime Parameter
        time = maxTime;

        // Get the PlayerManager
        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManagerScript>();
    }

    void FixedUpdate()
    {
        // Move the projectile according to direction and speed (FixedUpdate() is called every 0.02 seconds, so the speed parameter means how fast the projectile is in units per second)
        transform.position = new Vector3(transform.position.x + moveSpeed * 0.02f * (direction == PlayerMoveScript.LookDirection.LEFT ? -1 : 1), transform.position.y, transform.position.z);

        // Decrease the left time (FixedUpdate() is called every 0.02 seconds, so the time i sin seconds)
        time -= 0.02f;

        // If there is no time left, ask the playerManager to destroy the projectile. The projectile to be destroyed is specified by the list index
        if (time <= 0) playerManager.DestroyProjectile(listIndex);
    }

    public void SetListIndex(int value)
    {
        listIndex = value;
    }

    public int GetListIndex()
    {
        return listIndex;
    }

    public void SetDirection(PlayerMoveScript.LookDirection value)
    {
        direction = value;
    }
}