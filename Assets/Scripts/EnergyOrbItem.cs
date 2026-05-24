using UnityEngine;

/// <summary>
/// Energy Orb throwable item.
/// Strong arc, chargeable, glows.
/// Level 3 unlock: Arc Throw.
/// </summary>
public class EnergyOrbItem : ThrowableItem
{
    [Header("Orb Specific")]
    [SerializeField] private Color orbColor = new Color(0.2f, 0.6f, 1.0f); // Blue glow
    [SerializeField] private float orbRadius = 0.15f;
    [SerializeField] private float chargeMultiplier = 1.5f;

    private float chargeTime = 0f;
    private bool isCharging = false;

    public float ChargeTime => chargeTime;
    public bool IsCharging => isCharging;

    protected override void Awake()
    {
        itemName = "Energy Orb";
        baseThrowForce = 12f;
        damage = 3f;
        mass = 0.8f;
        drag = 0.3f;
        angularDrag = 0.5f;
        bounciness = 0.3f;
        maxBounces = 3;
        itemColor = orbColor;
        itemScale = Vector3.one * orbRadius * 2;

        base.Awake();
    }

    protected override void SetupVisual()
    {
        transform.localScale = Vector3.one * orbRadius * 2;

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
        mat.color = orbColor;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", orbColor * 0.5f);
        itemRenderer.material = mat;
    }

    public void StartCharging()
    {
        isCharging = true;
        chargeTime = 0f;
    }

    public void StopCharging()
    {
        isCharging = false;
    }

    public override void OnBeforeThrow(ref Vector3 velocity, ref Vector3 angularVelocity)
    {
        // Apply charge to throw force (upward arc)
        float chargeFactor = 1f + chargeTime * chargeMultiplier;
        chargeFactor = Mathf.Clamp(chargeFactor, 1f, 3f);

        // Add upward component for arc
        velocity.y += chargeTime * 5f;
        velocity *= chargeFactor;

        // Scale visual based on charge
        transform.localScale = Vector3.one * orbRadius * 2 * (1f + chargeTime * 0.5f);

        chargeTime = 0f;
        isCharging = false;
    }

    private void Update()
    {
        if (isCharging)
        {
            chargeTime += Time.deltaTime;

            // Pulse effect while charging
            float pulse = 1f + Mathf.Sin(chargeTime * 10f) * 0.1f;
            transform.localScale = Vector3.one * orbRadius * 2 * pulse;

            // Glow brighter with charge
            if (itemRenderer != null && itemRenderer.material != null)
            {
                float glowIntensity = 0.5f + chargeTime * 0.3f;
                itemRenderer.material.SetColor("_EmissionColor", orbColor * glowIntensity);
            }
        }
    }
}
