using UnityEngine;

/// <summary>
/// Throwing Knife throwable item.
/// Fast, strong spin, sharp bounce angles.
/// Level 2 unlock: Spin Throw.
/// </summary>
public class ThrowingKnifeItem : ThrowableItem
{
    [Header("Knife Specific")]
    [SerializeField] private Color knifeColor = new Color(0.7f, 0.7f, 0.8f); // Silver
    [SerializeField] private Vector3 knifeScale = new Vector3(0.05f, 0.3f, 0.02f);
    [SerializeField] private float spinMultiplier = 3f;

    protected override void Awake()
    {
        itemName = "Throwing Knife";
        baseThrowForce = 20f;
        damage = 2f;
        mass = 0.3f;
        drag = 0.2f;
        angularDrag = 0.1f;
        bounciness = 0.5f;
        maxBounces = 2;
        itemColor = knifeColor;
        itemScale = knifeScale;

        base.Awake();
    }

    protected override void SetupVisual()
    {
        transform.localScale = knifeScale;

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
        mat.color = knifeColor;
        itemRenderer.material = mat;
    }

    public override void OnBeforeThrow(ref Vector3 velocity, ref Vector3 angularVelocity)
    {
        // Knife spins fast along its forward axis
        angularVelocity = transform.forward * spinMultiplier * 10f;
    }
}
