using UnityEngine;

/// <summary>
/// Dodge behavior for NaughtyNPC.
/// NPCs can dodge incoming projectiles when they detect them.
/// Level 7 unlock: AI Dodge Counter.
/// </summary>
public class NPCDodgeBehavior : MonoBehaviour
{
    [Header("Dodge Settings")]
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private float dodgeForce = 5f;
    [SerializeField] private float dodgeCooldown = 1.5f;
    [SerializeField] private float reactionTime = 0.2f;
    [SerializeField] private LayerMask projectileLayer = -1;

    [Header("Dodge Chance")]
    [SerializeField] [Range(0f, 1f)] private float dodgeChance = 0.5f;

    private Rigidbody rb;
    private float lastDodgeTime = -1f;
    private bool isEnabled = false;

    public bool IsEnabled
    {
        get => isEnabled;
        set => isEnabled = value;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = true; // NPCs don't use physics normally
    }

    private void Update()
    {
        if (!isEnabled) return;

        // Check for incoming projectiles
        DetectIncomingProjectile();
    }

    private void DetectIncomingProjectile()
    {
        if (Time.time - lastDodgeTime < dodgeCooldown) return;

        // Find all projectiles in range
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, detectionRadius, projectileLayer);

        foreach (Collider col in nearbyColliders)
        {
            Rigidbody projectileRb = col.GetComponent<Rigidbody>();
            if (projectileRb == null) continue;

            // Check if projectile is moving toward this NPC
            Vector3 dirToProjectile = (col.transform.position - transform.position).normalized;
            Vector3 projectileVelocity = projectileRb.velocity;

            // If projectile is moving toward us
            if (Vector3.Dot(projectileVelocity.normalized, -dirToProjectile) > 0.5f)
            {
                // Predict if it will hit us
                float timeToImpact = Vector3.Distance(transform.position, col.transform.position) / Mathf.Max(projectileVelocity.magnitude, 0.1f);

                if (timeToImpact < reactionTime && timeToImpact > 0.1f)
                {
                    // Roll dodge chance
                    if (Random.value < dodgeChance)
                    {
                        PerformDodge(projectileVelocity);
                        return;
                    }
                }
            }
        }
    }

    private void PerformDodge(Vector3 incomingDirection)
    {
        lastDodgeTime = Time.time;

        // Dodge perpendicular to incoming direction
        Vector3 dodgeDir = Vector3.Cross(incomingDirection.normalized, Vector3.up).normalized;

        // Randomize left or right
        if (Random.value < 0.5f)
            dodgeDir = -dodgeDir;

        // Add slight backward movement
        dodgeDir += -incomingDirection.normalized * 0.3f;
        dodgeDir.Normalize();

        // Apply dodge (temporarily disable kinematic)
        rb.isKinematic = false;
        rb.AddForce(dodgeDir * dodgeForce, ForceMode.Impulse);

        // Re-enable kinematic after short delay
        Invoke(nameof(ResetKinematic), 0.3f);

        Debug.Log($"{gameObject.name} dodged! Direction: {dodgeDir}");
    }

    private void ResetKinematic()
    {
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
