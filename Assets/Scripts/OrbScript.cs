using UnityEngine;

public class OrbScript : MonoBehaviour
{
    public float moveSpeed;
    public float forceDampingFactor;
    public Rigidbody2D myRigidbody;
    public float followDistance;
    public bool shouldFollow = true;

    void FixedUpdate()
    {
        if(shouldFollow)
        {
            GameObject playerManager = GameObject.FindGameObjectWithTag("PlayerManager");

            if (playerManager != null) {
                if (playerManager.GetComponent<PlayerManagerScript>().IsTimerRunning())
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        Vector3 aimedPos = player.transform.position + followDistance * Vector3.Normalize(transform.position - player.transform.position);

                        Vector3 stretch = aimedPos - transform.position;

                        Vector2 dir = new Vector2(stretch.x, stretch.y);
                        dir.Normalize();

                        float magnitude = Mathf.Log(stretch.magnitude + 1, forceDampingFactor) * moveSpeed;

                        myRigidbody.AddForce(dir * magnitude);
                    }
                }
            }
        }
    }
}
