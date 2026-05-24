using UnityEngine;

/// <summary>
/// Slipper throwable prefab component.
/// Creates a flat cube visual for the slipper with proper physics setup.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class SlipperPrefab : MonoBehaviour
{
    [Header("Slipper Visual")]
    [SerializeField] private Color slipperColor = new Color(0.8f, 0.6f, 0.3f); // Brownish
    [SerializeField] private Vector3 slipperScale = new Vector3(0.2f, 0.05f, 0.35f);

    private void Awake()
    {
        transform.localScale = slipperScale;

        // Set up renderer
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        }
        renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        renderer.material.color = slipperColor;

        // Set up collider
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.size = Vector3.one;

        // Set up rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.mass = 0.5f;
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }
}
