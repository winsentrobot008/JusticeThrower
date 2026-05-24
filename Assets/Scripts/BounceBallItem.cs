using UnityEngine;

/// <summary>
/// Bounce Ball throwable item.
/// Multi-bounce projectile that can hit multiple targets.
/// Level 7 unlock: Ultimate multi-bounce.
/// </summary>
public class BounceBallItem : ThrowableItem
{
    [Header("Bounce Ball Specific")]
    [SerializeField] private Color ballColor = new Color(1f, 0.5f, 0f); // Orange
    [SerializeField] private float ballRadius = 0.1f;
    [SerializeField] private int multiBounces = 5;
    [SerializeField] private float bounceSpeedRetention = 0.9f; // Retain 90% speed on bounce
    [SerializeField] private float glowIntensity = 0.5f;

    private int targetsHit = 0;

    public int TargetsHit => targetsHit;

    protected override void Awake()
    {
        itemName = "Bounce Ball";
        baseThrowForce = 18f;
        damage = 0.5f; // Lower damage per hit, but can hit multiple
        mass = 0.3f;
        drag = 0.2f;
        angularDrag = 0.1f;
        bounciness = 0.95f; // Very bouncy
        maxBounces = multiBounces;
        itemColor = ballColor;
        itemScale = Vector3.one * ballRadius * 2;

        base.Awake();
    }

    protected override void SetupVisual()
    {
        transform.localScale = Vector3.one * ballRadius * 2;

        if (itemRenderer == null)
        {
            itemRenderer = GetComponent<MeshRenderer>();
            if (itemRenderer == null)
            {
                itemRenderer = gameObject.AddComponent<MeshRenderer>();
                gameObject.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            }
        }

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = ballColor;
        mat.SetFloat("_Smoothness", 0.8f);
        mat.SetFloat("_Metallic", 0.3f);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", ballColor * glowIntensity);
        itemRenderer.material = mat;

        // Add trail renderer for visual effect
        TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 0.3f;
        trail.startWidth = 0.05f;
        trail.endWidth = 0.01f;
        Material trailMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        trailMat.color = new Color(ballColor.r, ballColor.g, ballColor.b, 0.5f);
        trail.material = trailMat;
    }

    public override void OnBeforeThrow(ref Vector3 velocity, ref Vector3 angularVelocity)
    {
        // Add strong forward spin for better bouncing
        angularVelocity = new Vector3(0, 0, 500f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Track targets hit
        if (collision.gameObject.CompareTag("NaughtyNPC") ||
            collision.gameObject.CompareTag("VictimNPC"))
        {
            targetsHit++;
            Debug.Log($"Bounce Ball hit target #{targetsHit}");

            // Create hit spark effect
            CreateHitSpark(collision.contacts[0].point);
        }
    }

    private void CreateHitSpark(Vector3 position)
    {
        GameObject spark = new GameObject("HitSpark");
        spark.transform.position = position;

        LineRenderer line = spark.AddComponent<LineRenderer>();
        line.startWidth = 0.03f;
        line.endWidth = 0.01f;
        line.positionCount = 5;
        line.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        line.startColor = ballColor;
        line.endColor = Color.white;

        // Random spark directions
        Vector3[] positions = new Vector3[5];
        for (int i = 0; i < 5; i++)
        {
            positions[i] = position + Random.onUnitSphere * 0.3f;
        }
        line.SetPositions(positions);

        Destroy(spark, 0.2f);
    }
}
