using UnityEngine;

/// <summary>
/// Editor-only script to procedurally generate the Level 1 Subway scene layout.
/// Attach this to an empty GameObject in the scene and click "Generate Subway" in the Inspector.
/// </summary>
[ExecuteAlways]
public class Level1_Subway_Setup : MonoBehaviour
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

    [Header("NPCs")]
    [SerializeField] private GameObject victimPrefab;
    [SerializeField] private GameObject perpetratorPrefab;
    [SerializeField] private int victimCount = 3;
    [SerializeField] private int perpetratorCount = 2;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;

    [ContextMenu("Generate Subway Car")]
    public void GenerateSubwayCar()
    {
        // Clear existing generated objects
        ClearGenerated();

        // Create container
        GameObject container = new GameObject("SubwayCar_Generated");
        container.transform.SetParent(transform, false);

        // Build floor
        CreateBox(container, "Floor", new Vector3(carWidth, 0.1f, carLength),
                  new Vector3(0, -carHeight / 2f, 0), floorMaterial);

        // Build ceiling
        CreateBox(container, "Ceiling", new Vector3(carWidth, 0.1f, carLength),
                  new Vector3(0, carHeight / 2f, 0), ceilingMaterial);

        // Build walls
        CreateBox(container, "Wall_Left", new Vector3(0.1f, carHeight, carLength),
                  new Vector3(-carWidth / 2f, 0, 0), wallMaterial);
        CreateBox(container, "Wall_Right", new Vector3(0.1f, carHeight, carLength),
                  new Vector3(carWidth / 2f, 0, 0), wallMaterial);
        CreateBox(container, "Wall_Back", new Vector3(carWidth, carHeight, 0.1f),
                  new Vector3(0, 0, -carLength / 2f), wallMaterial);
        CreateBox(container, "Wall_Front", new Vector3(carWidth, carHeight, 0.1f),
                  new Vector3(0, 0, carLength / 2f), wallMaterial);

        // Add seats along the walls
        for (int i = 0; i < 4; i++)
        {
            float zPos = -carLength / 2f + 2f + i * 4f;
            CreateBox(container, $"Seat_Left_{i}", new Vector3(0.6f, 0.4f, 1.5f),
                      new Vector3(-carWidth / 2f + 0.5f, -carHeight / 2f + 0.2f, zPos), seatMaterial);
            CreateBox(container, $"Seat_Right_{i}", new Vector3(0.6f, 0.4f, 1.5f),
                      new Vector3(carWidth / 2f - 0.5f, -carHeight / 2f + 0.2f, zPos), seatMaterial);
        }

        // Spawn NPCs
        SpawnNPCs(container);

        // Spawn player
        SpawnPlayer(container);
    }

    private void SpawnNPCs(GameObject container)
    {
        // Spawn victims
        for (int i = 0; i < victimCount; i++)
        {
            Vector3 pos = GetRandomPositionInCar();
            GameObject victim;

            if (victimPrefab != null)
                victim = Instantiate(victimPrefab, pos, Quaternion.identity, container.transform);
            else
                victim = CreatePlaceholderNPC(container, $"Victim_{i}", pos, Color.green, "Victim");

            victim.tag = "Victim";
        }

        // Spawn perpetrators
        for (int i = 0; i < perpetratorCount; i++)
        {
            Vector3 pos = GetRandomPositionInCar();
            GameObject perp;

            if (perpetratorPrefab != null)
                perp = Instantiate(perpetratorPrefab, pos, Quaternion.identity, container.transform);
            else
                perp = CreatePlaceholderNPC(container, $"Perpetrator_{i}", pos, Color.red, "Perpetrator");

            perp.tag = "Perpetrator";
        }
    }

    private GameObject CreatePlaceholderNPC(GameObject container, string name, Vector3 position, Color color, string tag)
    {
        GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        npc.name = name;
        npc.transform.SetParent(container.transform, false);
        npc.transform.position = position;
        npc.transform.localScale = new Vector3(0.5f, 1.2f, 0.5f);
        npc.tag = tag;

        Renderer renderer = npc.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = color;

        // Add appropriate NPC script
        if (tag == "Victim")
            npc.AddComponent<NPC_Victim>();
        else
            npc.AddComponent<NPC_Perpetrator>();

        return npc;
    }

    private void SpawnPlayer(GameObject container)
    {
        GameObject player;

        if (playerPrefab != null)
        {
            player = Instantiate(playerPrefab, new Vector3(0, 0.5f, -carLength / 2f + 2f),
                                 Quaternion.identity, container.transform);
        }
        else
        {
            // Create a basic first-person player
            player = new GameObject("Player");
            player.transform.SetParent(container.transform, false);
            player.transform.position = new Vector3(0, 0.5f, -carLength / 2f + 2f);

            Camera cam = player.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.fieldOfView = 70f;

            player.AddComponent<ThrowController>();
            player.AddComponent<AudioListener>();
        }

        player.tag = "Player";
    }

    private Vector3 GetRandomPositionInCar()
    {
        float x = Random.Range(-carWidth / 2f + 0.8f, carWidth / 2f - 0.8f);
        float z = Random.Range(-carLength / 2f + 2f, carLength / 2f - 2f);
        return new Vector3(x, 0f, z);
    }

    private GameObject CreateBox(GameObject parent, string name, Vector3 size, Vector3 position, Material material)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.SetParent(parent.transform, false);
        box.transform.localScale = size;
        box.transform.localPosition = position;

        Renderer renderer = box.GetComponent<Renderer>();
        if (renderer != null && material != null)
            renderer.material = material;

        // Add collider if not present
        if (box.GetComponent<Collider>() == null)
            box.AddComponent<BoxCollider>();

        return box;
    }

    [ContextMenu("Clear Generated")]
    public void ClearGenerated()
    {
        // Destroy previously generated subway car
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name == "SubwayCar_Generated")
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}
