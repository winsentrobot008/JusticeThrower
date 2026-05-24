using UnityEngine;

/// <summary>
/// Simple projectile visual prefab component.
/// Attach this to a Sphere GameObject to make it a throwable projectile.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class ProjectilePrefab : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Color projectileColor = Color.yellow;
    [SerializeField] private float scale = 0.15f;

    private void Awake()
    {
        transform.localScale = Vector3.one * scale;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = projectileColor;

        // Ensure proper physics setup
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        SphereCollider collider = GetComponent<SphereCollider>();
        collider.isTrigger = false;
    }
}
