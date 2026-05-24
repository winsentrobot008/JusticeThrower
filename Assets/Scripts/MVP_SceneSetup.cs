using UnityEngine;

/// <summary>
/// MVP scene setup script.
/// Attach to an empty GameObject in the scene.
/// Generates the entire Level 1 Subway scene at runtime:
/// - Subway car (floor, ceiling, walls, seats)
/// - First-person camera (fixed, no movement)
/// - VictimNPC and NaughtyNPC
/// - PlayerThrow controller
/// - LevelManager and UIManager
/// - Slipper prefab
/// </summary>
public class MVP_SceneSetup : MonoBehaviour
{
    [Header("Subway Car Dimensions")]
    [SerializeField] private float carLength = 20f;
    [SerializeField] private float carWidth = 3.5f;
    [SerializeField] private float carHeight = 2.8f;

    [Header("Materials")]
    [SerializeField] private Material floorMaterial;
    [SerializeField] private Material wallMaterial;
    [SerializeField] private Material ceilingMaterial;
    [SerializeField] private Material seatMaterial;

    [Header("Physics Material")]
    [SerializeField] private PhysicMaterial bouncyMaterial;

    [Header("NPC Colors")]
    [SerializeField] private Color victimColor = Color.green;
    [SerializeField] private Color naughtyColor = Color.red;

    [ContextMenu("Generate MVP Scene")]
    public void GenerateScene()
    {
        ClearGenerated();

        GameObject container = new GameObject("MVP_Scene_Generated");
        container.transform.SetParent(transform, false);

        // 1. Build subway car
        BuildSubwayCar(container);

        // 2. Create first-person camera (fixed)
        CreateFirstPersonCamera(container);

        // 3. Create NPCs
        CreateNPCs(container);

        // 4. Create slipper prefab
        GameObject slipperPrefab = CreateSlipperPrefab(container);

        // 5. Setup PlayerThrow on camera
        SetupPlayerThrow(slipperPrefab);

        // 6. Create LevelManager
        CreateLevelManager(container);

        // 7. Create UIManager
        CreateUIManager();

        Debug.Log("MVP Scene generated successfully!");
    }

    private void BuildSubwayCar(GameObject container)
    {
        // Floor
        CreateBox(container, "Floor", new Vector3(carWidth, 0.1f, carLength),
                  new Vector3(0, -carHeight / 2f, 0), floorMaterial, true);

        // Ceiling
        CreateBox(container, "Ceiling", new Vector3(carWidth, 0.1f, carLength),
                  new Vector3(0, carHeight / 2f, 0), ceilingMaterial, true);

        // Walls
        CreateBox(container, "Wall_Left", new Vector3(0.1f, carHeight, carLength),
                  new Vector3(-carWidth / 2f, 0, 0), wallMaterial, true);
        CreateBox(container, "Wall_Right", new Vector3(0.1f, carHeight, carLength),
                  new Vector3(carWidth / 2f, 0, 0), wallMaterial, true);
        CreateBox(container, "Wall_Back", new Vector3(carWidth, carHeight, 0.1f),
                  new Vector3(0, 0, -carLength / 2f), wallMaterial, true);
        CreateBox(container, "Wall_Front", new Vector3(carWidth, carHeight, 0.1f),
                  new Vector3(0, 0, carLength / 2f), wallMaterial, true);

        // Seats along walls
        for (int i = 0; i < 4; i++)
        {
            float zPos = -carLength / 2f + 2f + i * 4f;
            CreateBox(container, $"Seat_Left_{i}", new Vector3(0.6f, 0.4f, 1.5f),
                      new Vector3(-carWidth / 2f + 0.5f, -carHeight / 2f + 0.2f, zPos), seatMaterial, true);
            CreateBox(container, $"Seat_Right_{i}", new Vector3(0.6f, 0.4f, 1.5f),
                      new Vector3(carWidth / 2f - 0.5f, -carHeight / 2f + 0.2f, zPos), seatMaterial, true);
        }

        // Handrails (simple cylinders)
        for (int i = 0; i < 3; i++)
        {
            float zPos = -carLength / 2f + 3f + i * 6f;
            CreateCylinder(container, $"Handrail_{i}", new Vector3(0.05f, carHeight * 0.6f, 0.05f),
                          new Vector3(0, carHeight * 0.1f, zPos));
        }
    }

    private void CreateFirstPersonCamera(GameObject container)
    {
        GameObject cameraGO = new GameObject("FirstPersonCamera");
        cameraGO.transform.SetParent(container.transform, false);
        cameraGO.transform.position = new Vector3(0, 0.5f, -carLength / 2f + 1.5f);
        cameraGO.transform.rotation = Quaternion.Euler(0, 0, 0); // Looking down the aisle

        Camera cam = cameraGO.AddComponent<Camera>();
        cam.fieldOfView = 70f;
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 50f;

        cameraGO.AddComponent<AudioListener>();

        // Add a light to the player for basic illumination
        Light playerLight = cameraGO.AddComponent<Light>();
        playerLight.type = LightType.Directional;
        playerLight.intensity = 0.8f;
        playerLight.transform.rotation = Quaternion.Euler(50, -30, 0);
    }

    private void CreateNPCs(GameObject container)
    {
        // VictimNPC - placed in the middle, slightly to the left (visible directly)
        Vector3 victimPos = new Vector3(-0.8f, 0f, 0f);
        GameObject victim = CreateCapsuleNPC(container, "VictimNPC", victimPos, victimColor);
        victim.AddComponent<VictimNPC>();

        // NaughtyNPC - placed behind the victim (cannot be hit directly, needs bounce)
        Vector3 naughtyPos = new Vector3(0.8f, 0f, 3f);
        GameObject naughty = CreateCapsuleNPC(container, "NaughtyNPC", naughtyPos, naughtyColor);
        naughty.AddComponent<NaughtyNPC>();
    }

    private GameObject CreateCapsuleNPC(GameObject container, string name, Vector3 position, Color color)
    {
        GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        npc.name = name;
        npc.transform.SetParent(container.transform, false);
        npc.transform.position = position;
        npc.transform.localScale = new Vector3(0.5f, 1.2f, 0.5f);

        Renderer renderer = npc.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        renderer.material.color = color;

        // Add a box collider for better hit detection
        BoxCollider boxCol = npc.AddComponent<BoxCollider>();
        boxCol.size = new Vector3(1, 2, 1);
        boxCol.isTrigger = false;

        // Remove the default capsule collider
        DestroyImmediate(npc.GetComponent<CapsuleCollider>());

        return npc;
    }

    private GameObject CreateSlipperPrefab(GameObject container)
    {
        GameObject slipper = GameObject.CreatePrimitive(PrimitiveType.Cube);
        slipper.name = "Throwable_Slipper";
        slipper.transform.SetParent(container.transform, false);
        slipper.SetActive(false);

        // Remove default collider, we'll add proper one
        DestroyImmediate(slipper.GetComponent<BoxCollider>());

        // Add SlipperPrefab component (handles all setup)
        SlipperPrefab sp = slipper.AddComponent<SlipperPrefab>();

        // Add BouncePhysics
        BouncePhysics bp = slipper.AddComponent<BouncePhysics>();

        // Assign physics material
        if (bouncyMaterial != null)
        {
            Collider col = slipper.GetComponent<Collider>();
            if (col != null)
                col.material = bouncyMaterial;
        }

        return slipper;
    }

    private void SetupPlayerThrow(GameObject slipperPrefab)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        PlayerThrow pt = cam.gameObject.GetComponent<PlayerThrow>();
        if (pt == null)
            pt = cam.gameObject.AddComponent<PlayerThrow>();

        // Assign the slipper prefab
        pt.slipperPrefab = slipperPrefab;
    }

    private void CreateLevelManager(GameObject container)
    {
        GameObject lmGO = new GameObject("LevelManager");
        lmGO.transform.SetParent(container.transform, false);
        LevelManager lm = lmGO.AddComponent<LevelManager>();
    }

    private void CreateUIManager()
    {
        GameObject uiGO = new GameObject("UIManager");
        UIManager ui = uiGO.AddComponent<UIManager>();
    }

    private GameObject CreateBox(GameObject parent, string name, Vector3 scale, Vector3 position, Material material, bool addBouncy)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.SetParent(parent.transform, false);
        box.transform.localScale = scale;
        box.transform.localPosition = position;

        if (material != null)
        {
            Renderer renderer = box.GetComponent<Renderer>();
            renderer.material = material;
        }

        // Add bouncy physics material to collider
        if (addBouncy && bouncyMaterial != null)
        {
            Collider col = box.GetComponent<Collider>();
            if (col != null)
                col.material = bouncyMaterial;
        }

        return box;
    }

    private GameObject CreateCylinder(GameObject parent, string name, Vector3 scale, Vector3 position)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.name = name;
        cylinder.transform.SetParent(parent.transform, false);
        cylinder.transform.localScale = scale;
        cylinder.transform.localPosition = position;

        Renderer renderer = cylinder.GetComponent<Renderer>();
        renderer.material.color = new Color(0.7f, 0.7f, 0.7f);

        return cylinder;
    }

    [ContextMenu("Clear Generated")]
    public void ClearGenerated()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name == "MVP_Scene_Generated")
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}
