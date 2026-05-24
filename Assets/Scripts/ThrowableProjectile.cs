using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A projectile that travels in a straight line and can bounce off surfaces.
/// After reaching maxBounces, it is destroyed.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ThrowableProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private bool destroyOnHit = true;

    [Header("Events")]
    public UnityEvent<Collision> onBounce;
    public UnityEvent<Collision> onHitTarget;

    private Rigidbody rb;
    private int bouncesRemaining;
    private float spawnTime;
    private LayerMask bounceMask;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Straight-line projectile
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void Start()
    {
        spawnTime = Time.time;
    }

    private void Update()
    {
        // Destroy after lifetime expires
        if (Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Launches the projectile with the given velocity.
    /// </summary>
    public void Launch(Vector3 velocity, int bounces, LayerMask layerMask)
    {
        bouncesRemaining = bounces;
        bounceMask = layerMask;
        rb.velocity = velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if we hit a valid bounce layer
        if ((bounceMask.value & (1 << collision.gameObject.layer)) == 0)
            return;

        // Check tags for special targets
        if (collision.gameObject.CompareTag("Victim") || collision.gameObject.CompareTag("Perpetrator"))
        {
            onHitTarget?.Invoke(collision);
            HandleHitTarget(collision);
            return;
        }

        // Bounce logic
        if (bouncesRemaining > 0)
        {
            bouncesRemaining--;
            onBounce?.Invoke(collision);

            // Reflect velocity for perfect bounce
            Vector3 reflectedVelocity = Vector3.Reflect(rb.velocity, collision.contacts[0].normal);
            rb.velocity = reflectedVelocity;

            // Optional: play bounce sound / effect here
        }
        else
        {
            // No bounces left — destroy
            if (destroyOnHit)
                Destroy(gameObject);
        }
    }

    private void HandleHitTarget(Collision collision)
    {
        // Placeholder: handle hitting a victim or perpetrator
        Debug.Log($"{gameObject.name} hit {collision.gameObject.name}");

        // You can add score, effects, etc. here later
        Destroy(gameObject);
    }
}
