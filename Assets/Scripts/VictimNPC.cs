using UnityEngine;

/// <summary>
/// Victim NPC - innocent person. Hitting them is bad!
/// On hit: flashes red and scales down briefly.
/// </summary>
public class VictimNPC : MonoBehaviour, INPCHitReaction
{
    [Header("Victim Settings")]
    [SerializeField] private Color hitFlashColor = Color.red;
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private float scaleShrink = 0.5f;

    private Renderer npcRenderer;
    private Color originalColor;
    private Vector3 originalScale;

    private void Awake()
    {
        npcRenderer = GetComponent<Renderer>();
        if (npcRenderer != null)
            originalColor = npcRenderer.material.color;

        originalScale = transform.localScale;

        // Auto-tag
        if (!gameObject.CompareTag("VictimNPC"))
            gameObject.tag = "VictimNPC";
    }

    public void OnHit()
    {
        Debug.Log("VictimNPC hit! (Innocent - BAD!)");
        StopAllCoroutines();
        StartCoroutine(HitFeedbackCoroutine());
    }

    private System.Collections.IEnumerator HitFeedbackCoroutine()
    {
        // Flash red
        if (npcRenderer != null)
            npcRenderer.material.color = hitFlashColor;

        // Shrink
        transform.localScale = originalScale * scaleShrink;

        yield return new WaitForSeconds(flashDuration);

        // Restore
        if (npcRenderer != null)
            npcRenderer.material.color = originalColor;

        transform.localScale = originalScale;
    }
}
