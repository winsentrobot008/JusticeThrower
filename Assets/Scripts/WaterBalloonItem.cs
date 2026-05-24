using UnityEngine;

/// <summary>
/// Water Balloon throwable item.
/// Splash AoE damage on impact.
/// Level 5 unlock: Splash Attack.
/// </summary>
public class WaterBalloonItem : ThrowableItem
{
    [Header("Water Balloon Specific")]
    [SerializeField] private Color balloonColor = new Color(0.2f, 0.6f, 0.9f); // Blue
    [SerializeField] private float balloonRadius = 0.12f;
    [SerializeField] private float splashRadius = 2.5f;
    [SerializeField] private float splashForce = 5f;
    [SerializeField] private float splashDuration = 0.5f;
    [SerializeField] private LayerMask npcLayer = -1;

    private bool hasSplashed = false;

    public float SplashRadius => splashRadius;

    protected override void Awake()
    {
        itemName = "Water Balloon";
        baseThrowForce = 12f;
        damage = 1.5f;
        mass = 0.6f;
        drag = 0.8f;
        angularDrag = 0.3f;
        bounciness = 0.1f; // Low bounce - it pops
        maxBounces = 0; // No bounce - explodes on contact
        itemColor = balloonColor;
        itemScale = Vector3.one * balloonRadius * 2;

        base.Awake();
    }

    protected override void SetupVisual()
    {
        transform.localScale = Vector3.one * balloonRadius * 2;

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
        mat.color = balloonColor;
        mat.SetFloat("_Smoothness", 0.3f);
        itemRenderer.material = mat;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasSplashed) return;
        hasSplashed = true;

        // Splash effect
        Splash();

        // Destroy after splash
        Destroy(gameObject, 0.1f);
    }

    private void Splash()
    {
        // Find all NPCs in splash radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, splashRadius, npcLayer);

        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject == gameObject) continue;

            // Check if it's an NPC
            INPCHitReaction reaction = hit.GetComponent<INPCHitReaction>();
            if (reaction != null)
            {
                reaction.OnHit();
                Debug.Log($"Water Balloon splash hit: {hit.gameObject.name}");
            }

            // Apply splash force
            Rigidbody hitRb = hit.GetComponent<Rigidbody>();
            if (hitRb != null)
            {
                Vector3 forceDir = (hit.transform.position - transform.position).normalized;
                hitRb.AddForce(forceDir * splashForce, ForceMode.Impulse);
            }
        }

        // Create splash visual effect
        CreateSplashEffect();
    }

    private void CreateSplashEffect()
    {
        // Create a simple expanding sphere as splash visual
        GameObject splashGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        splashGO.name = "SplashEffect";
        splashGO.transform.position = transform.position;
        splashGO.transform.localScale = Vector3.zero;

        Renderer splashRenderer = splashGO.GetComponent<Renderer>();
        Material splashMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        splashMat.color = new Color(balloonColor.r, balloonColor.g, balloonColor.b, 0.5f);
        splashMat.SetFloat("_Surface", 1f); // Transparent
        splashRenderer.material = splashMat;

        // Destroy default collider
        Destroy(splashGO.GetComponent<Collider>());

        // Animate splash
        SplashAnimation splashAnim = splashGO.AddComponent<SplashAnimation>();
        splashAnim.Initialize(splashRadius, splashDuration);
    }

    /// <summary>
    /// Simple splash animation component.
    /// </summary>
    private class SplashAnimation : MonoBehaviour
    {
        private float targetRadius;
        private float duration;
        private float elapsed = 0f;

        public void Initialize(float radius, float dur)
        {
            targetRadius = radius;
            duration = dur;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (t >= 1f)
            {
                Destroy(gameObject);
                return;
            }

            // Expand then fade
            float scale = Mathf.Lerp(0, targetRadius * 2, t);
            transform.localScale = Vector3.one * scale;

            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                Color c = renderer.material.color;
                c.a = Mathf.Lerp(0.5f, 0f, t);
                renderer.material.color = c;
            }
        }
    }
}
