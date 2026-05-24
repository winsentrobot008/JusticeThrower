using UnityEngine;

/// <summary>
/// Naughty NPC - the target. Hitting them is good!
/// On hit: flashes green and scales up briefly.
/// </summary>
public class NaughtyNPC : MonoBehaviour, INPCHitReaction
{
    [Header("Naughty Settings")]
    [SerializeField] private Color hitFlashColor = Color.green;
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private float scalePulse = 1.3f;

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
        if (!gameObject.CompareTag("NaughtyNPC"))
            gameObject.tag = "NaughtyNPC";
    }

    public void OnHit()
    {
        Debug.Log("NaughtyNPC hit! (Guilty - GOOD!)");
        StopAllCoroutines();
        StartCoroutine(HitFeedbackCoroutine());
    }

    private System.Collections.IEnumerator HitFeedbackCoroutine()
    {
        // Flash green
        if (npcRenderer != null)
            npcRenderer.material.color = hitFlashColor;

        // Pulse bigger
        transform.localScale = originalScale * scalePulse;

        yield return new WaitForSeconds(flashDuration);

        // Restore
        if (npcRenderer != null)
            npcRenderer.material.color = originalColor;

        transform.localScale = originalScale;
    }
}
