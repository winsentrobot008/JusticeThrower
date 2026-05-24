using UnityEngine;

/// <summary>
/// Slipper throwable item.
/// Light, good bounce, basic visual.
/// </summary>
public class SlipperItem : ThrowableItem
{
    [Header("Slipper Specific")]
    [SerializeField] private Color slipperColor = new Color(0.8f, 0.6f, 0.3f); // Brownish
    [SerializeField] private Vector3 slipperScale = new Vector3(0.2f, 0.05f, 0.35f);

    protected override void Awake()
    {
        itemName = "Slipper";
        baseThrowForce = 15f;
        damage = 1f;
        mass = 0.5f;
        drag = 0.5f;
        angularDrag = 0.5f;
        bounciness = 0.7f;
        maxBounces = 1;
        itemColor = slipperColor;
        itemScale = slipperScale;

        base.Awake();
    }

    protected override void SetupVisual()
    {
        transform.localScale = slipperScale;

        if (itemRenderer == null)
        {
            itemRenderer = GetComponent<MeshRenderer>();
            if (itemRenderer == null)
            {
                itemRenderer = gameObject.AddComponent<MeshRenderer>();
                gameObject.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            }
        }

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = slipperColor;
        itemRenderer.material = mat;
    }
}
