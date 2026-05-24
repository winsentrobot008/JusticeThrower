using UnityEngine;

/// <summary>
/// Placeholder NPC representing a "perpetrator" — a shady/scummy person on the subway.
/// </summary>
public class NPC_Perpetrator : MonoBehaviour
{
    [Header("Perpetrator Settings")]
    [SerializeField] private string perpetratorName = "Shady Character";
    [SerializeField] private Color perpetratorColor = Color.red;

    private Renderer npcRenderer;

    private void Awake()
    {
        npcRenderer = GetComponent<Renderer>();
        if (npcRenderer != null)
            npcRenderer.material.color = perpetratorColor;

        // Auto-tag if not already tagged
        if (!gameObject.CompareTag("Perpetrator"))
            gameObject.tag = "Perpetrator";
    }

    public void OnHitByProjectile()
    {
        Debug.Log($"{perpetratorName} was hit! (Perpetrator)");
        // Placeholder: play reaction animation, sound, etc.
    }
}
