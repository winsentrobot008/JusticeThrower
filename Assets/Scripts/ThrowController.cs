using UnityEngine;

/// <summary>
/// First-person fixed-view throw controller.
/// Shoots a projectile in a straight line with one bounce off surfaces.
/// </summary>
public class ThrowController : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float throwForce = 20f;
    [SerializeField] private int maxBounces = 1;
    [SerializeField] private LayerMask bounceLayerMask = ~0;

    [Header("References")]
    [SerializeField] private Transform throwOrigin;

    private Camera playerCamera;

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
        if (Input.GetButtonDown("Fire1"))
        {
            Throw();
        }
    }

    private void Throw()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("ThrowController: No projectilePrefab assigned!");
            return;
        }

        // Instantiate projectile at throw origin
        GameObject projectile = Instantiate(projectilePrefab, throwOrigin.position, throwOrigin.rotation);

        // Get or add the projectile component
        ThrowableProjectile tp = projectile.GetComponent<ThrowableProjectile>();
        if (tp == null)
            tp = projectile.AddComponent<ThrowableProjectile>();

        // Launch it
        Vector3 direction = throwOrigin.forward;
        tp.Launch(direction * throwForce, maxBounces, bounceLayerMask);
    }
}
