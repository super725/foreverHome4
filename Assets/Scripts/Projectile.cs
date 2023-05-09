using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    public float speed = 5f;
    private string playerTag = "Player";
    public float drag = 1f;
    private float descentSpeed = 0.05f;
    public float curveFactor = 0.2f;
    public float forceStrength = 10f;

    private Transform playerTransform;
    private Rigidbody rigidbodyattr;

    private void Start()
    {
        // Find the player object with the specified tag
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        // Set the player transform to track the player object
        playerTransform = player.transform;

        // Get the rigidbody component of the object
        rigidbodyattr = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Calculate the direction to the player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Add drag to the motion by curving towards the player
        Vector3 curve = Vector3.Lerp(directionToPlayer, transform.right, curveFactor);
        Vector3 targetVelocity = curve * speed * Time.deltaTime;

        // Apply a force towards the player
        rigidbodyattr.AddForce(targetVelocity.normalized * forceStrength, ForceMode.Force);

        // Continuously descend
        transform.position -= Vector3.up * descentSpeed * Time.deltaTime;

        // Rotate towards the player
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        // Destroy the object if it exists for more than 5 seconds
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Destroy the object if it collides with anything
        if(!(collision.gameObject.tag == "Enemy")){
            Destroy(gameObject);
        }
    }
}