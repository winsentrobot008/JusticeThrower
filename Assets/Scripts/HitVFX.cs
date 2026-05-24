using UnityEngine;

/// <summary>
/// Visual effects for hits and impacts.
/// Creates particle-like effects using simple primitives.
/// </summary>
public class HitVFX : MonoBehaviour
{
    [Header("VFX Settings")]
    [SerializeField] private Color naughtyHitColor = Color.green;
    [SerializeField] private Color victimHitColor = Color.red;
    [SerializeField] private Color bounceColor = Color.white;
    [SerializeField] private float particleLifetime = 0.5f;
    [SerializeField] private int particleCount = 8;
    [SerializeField] private float particleSpeed = 2f;

    private static HitVFX _instance;
    public static HitVFX Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    /// <summary>
    /// Spawn hit particles at position.
    /// </summary>
    public void SpawnHitParticles(Vector3 position, Color color)
    {
        for (int i = 0; i < particleCount; i++)
        {
            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            particle.name = "VFX_Particle";
            particle.transform.position = position;
            particle.transform.localScale = Vector3.one * Random.Range(0.03f, 0.08f);

            // Remove collider
            Destroy(particle.GetComponent<Collider>());

            // Set color
            Renderer renderer = particle.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.color = color;
            renderer.material = mat;

            // Add movement
            Rigidbody rb = particle.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.velocity = Random.onUnitSphere * particleSpeed;
            rb.mass = 0.1f;

            // Destroy after lifetime
            Destroy(particle, particleLifetime);
        }
    }

    /// <summary>
    /// Spawn hit particles for NaughtyNPC hit.
    /// </summary>
    public void SpawnNaughtyHitVFX(Vector3 position)
    {
        SpawnHitParticles(position, naughtyHitColor);
    }

    /// <summary>
    /// Spawn hit particles for VictimNPC hit.
    /// </summary>
    public void SpawnVictimHitVFX(Vector3 position)
    {
        SpawnHitParticles(position, victimHitColor);
    }

    /// <summary>
    /// Spawn bounce spark effect.
    /// </summary>
    public void SpawnBounceVFX(Vector3 position)
    {
        SpawnHitParticles(position, bounceColor);
    }
}
