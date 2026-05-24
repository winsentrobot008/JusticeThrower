using UnityEngine;

/// <summary>
/// Placeholder NPC representing a "victim" — an innocent person on the subway.
/// </summary>
public class NPC_Victim : MonoBehaviour
{
    [Header("Victim Settings")]
    [SerializeField] private string victimName = "Innocent Passenger";
    [SerializeField] private Color victimColor = Color.green;

    private Renderer npcRenderer;

    private void Awake()
    {
        npcRenderer = GetComponent<Renderer>();
        if (npcRenderer != null)
            npcRenderer.material.color = victimColor;

        // Auto-tag if not already tagged
        if (!gameObject.CompareTag("Victim"))
            gameObject.tag = "Victim";
    }

    public void OnHitByProjectile()
    {
        Debug.Log($"{victimName} was hit! (Victim)");
        // Placeholder: play reaction animation, sound, etc.
    }
}
