using UnityEngine;

/// <summary>
/// Base class for all throwable items.
/// Defines common properties and behavior.
/// </summary>
public abstract class ThrowableItem : MonoBehaviour
{
    [Header("Throwable Item Base")]
    [SerializeField] protected string itemName = "Throwable";
    [SerializeField] protected float baseThrowForce = 15f;
    [SerializeField] protected float damage = 1f;
    [SerializeField] protected Color itemColor = Color.white;
    [SerializeField] protected Vector3 itemScale = Vector3.one;

    [Header("Physics")]
    [SerializeField] protected float mass = 0.5f;
    [SerializeField] protected float drag = 0.5f;
    [SerializeField] protected float angularDrag = 0.5f;
    [SerializeField] protected float bounciness = 0.7f;

    [Header("Bounce Settings")]
    [SerializeField] protected int maxBounces = 1;

    protected Rigidbody rb;
    protected Renderer itemRenderer;
    protected BouncePhysics bouncePhysics;

    public string ItemName => itemName;
    public float BaseThrowForce => baseThrowForce;
    public float Damage => damage;
    public int MaxBounces => maxBounces;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        itemRenderer = GetComponent<Renderer>();
        bouncePhysics = GetComponent<BouncePhysics>();
        if (bouncePhysics == null)
            bouncePhysics = gameObject.AddComponent<BouncePhysics>();

        SetupPhysics();
        SetupVisual();
    }

    protected virtual void SetupPhysics()
    {
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Set bounciness on collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            PhysicMaterial physMat = new PhysicMaterial($"{itemName}_PhysMat");
            physMat.bounciness = bounciness;
            physMat.bounceCombine = PhysicMaterialCombine.Maximum;
            physMat.frictionCombine = PhysicMaterialCombine.Minimum;
            col.material = physMat;
        }

        // Configure BouncePhysics
        if (bouncePhysics != null)
        {
            bouncePhysics.SetMaxBounces(maxBounces);
        }
    }

    protected abstract void SetupVisual();

    /// <summary>
    /// Apply item-specific throw modifiers before launch.
    /// </summary>
    public virtual void OnBeforeThrow(ref Vector3 velocity, ref Vector3 angularVelocity)
    {
        // Base implementation: no modifiers
    }

    /// <summary>
    /// Launch the projectile.
    /// </summary>
    public virtual void Launch(Vector3 velocity, Vector3 angularVelocity)
    {
        if (rb != null)
        {
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }
}
