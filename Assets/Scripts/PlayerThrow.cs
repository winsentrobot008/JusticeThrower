using UnityEngine;

/// <summary>
/// Player throw controller for MVP.
/// Left click to throw a slipper projectile forward.
/// 1 second cooldown between throws.
/// </summary>
public class PlayerThrow : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] private GameObject slipperPrefab;
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float cooldownTime = 1f;

    [Header("References")]
    [SerializeField] private Transform throwOrigin;

    private float lastThrowTime = -1f;
    private Camera playerCamera;

    // Reference for UI cooldown bar
    public float CooldownRemaining => Mathf.Max(0, cooldownTime - (Time.time - lastThrowTime));
    public float CooldownNormalized => CooldownRemaining / cooldownTime;
    public bool IsOnCooldown => Time.time - lastThrowTime < cooldownTime;

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (throwOrigin == null)
            throwOrigin = transform;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && !IsOnCooldown)
        {
            Throw();
        }
    }

    private void Throw()
    {
        if (slipperPrefab == null)
        {
            Debug.LogError("PlayerThrow: No slipperPrefab assigned!");
            return;
        }

        lastThrowTime = Time.time;

        // Instantiate slipper at throw origin
        GameObject slipper = Instantiate(slipperPrefab, throwOrigin.position, throwOrigin.rotation);

        // Get or add BouncePhysics
        BouncePhysics bounce = slipper.GetComponent<BouncePhysics>();
        if (bounce == null)
            bounce = slipper.AddComponent<BouncePhysics>();

        // Launch forward
        Vector3 direction = throwOrigin.forward;
        bounce.Launch(direction * throwForce);
    }
}
