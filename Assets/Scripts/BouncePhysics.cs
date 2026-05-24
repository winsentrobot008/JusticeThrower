using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls bounce physics for throwable projectiles.
/// - Angle of incidence = Angle of reflection (Vector3.Reflect)
/// - Can bounce exactly once, then stops bouncing
/// - Hits NaughtyNPC triggers onHitNaughty event
/// - Hits VictimNPC triggers onHitVictim event
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BouncePhysics : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private int maxBounces = 1;
    [SerializeField] private LayerMask bounceLayer = ~0;
    [SerializeField] private float lifetime = 5f;

    [Header("Events")]
    public UnityEvent<GameObject> onHitNaughty;
    public UnityEvent<GameObject> onHitVictim;
    public UnityEvent onBounce;

    private Rigidbody rb;
    private int bouncesRemaining;
    private float spawnTime;
    private bool hasHitTarget;

    public bool HasHitTarget => hasHitTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void Start()
    {
        spawnTime = Time.time;
        bouncesRemaining = maxBounces;
    }

    private void Update()
    {
        // Auto-destroy after lifetime
        if (Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Set max bounces (called by ThrowableItem).
    /// </summary>
    public void SetMaxBounces(int bounces)
    {
        maxBounces = bounces;
        bouncesRemaining = maxBounces;
    }

    /// <summary>
    /// Launch the projectile with given velocity.
    /// </summary>
    public void Launch(Vector3 velocity)
    {
        rb.velocity = velocity;
    }

    /// <summary>
    /// Launch with both velocity and angular velocity.
    /// </summary>
    public void Launch(Vector3 velocity, Vector3 angularVelocity)
    {
        rb.velocity = velocity;
        rb.angularVelocity = angularVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If already hit a target, ignore further collisions
        if (hasHitTarget) return;

        // Check for NPC hits first
        if (collision.gameObject.CompareTag("NaughtyNPC"))
        {
            hasHitTarget = true;
            onHitNaughty?.Invoke(collision.gameObject);
            HandleNPCHit(collision.gameObject);
            return;
        }

        if (collision.gameObject.CompareTag("VictimNPC"))
        {
            hasHitTarget = true;
            onHitVictim?.Invoke(collision.gameObject);
            HandleNPCHit(collision.gameObject);
            return;
        }

        // Bounce logic for environment surfaces
        if (bouncesRemaining > 0)
        {
            bouncesRemaining--;
            onBounce?.Invoke();

            // Perfect reflection: angle of incidence = angle of reflection
            Vector3 reflectDir = Vector3.Reflect(rb.velocity.normalized, collision.contacts[0].normal);
            float speed = rb.velocity.magnitude;
            rb.velocity = reflectDir * speed;

            // Rotate to face new direction
            transform.forward = reflectDir;
        }
        else
        {
            // No bounces left, destroy after a short delay
            Destroy(gameObject, 0.1f);
        }
    }

    private void HandleNPCHit(GameObject npc)
    {
        // Disable physics so it drops
        rb.isKinematic = true;

        // Notify the NPC
        INPCHitReaction reaction = npc.GetComponent<INPCHitReaction>();
        if (reaction != null)
        {
            reaction.OnHit();
        }

        // Notify LevelManager
        LevelManager lm = LevelManager.Instance;
        if (lm != null)
        {
            if (npc.CompareTag("NaughtyNPC"))
                lm.OnNaughtyHit(npc);
            else if (npc.CompareTag("VictimNPC"))
                lm.OnVictimHit(npc);
        }

        // Destroy projectile after brief delay
        Destroy(gameObject, 0.5f);
    }
}
