using UnityEngine;

/// <summary>
/// Victim NPC - innocent person. Hitting them is bad!
/// On hit: flashes red, scales down, plays hit reaction animation.
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
    private NPCAnimationController animController;

    private void Awake()
    {
        npcRenderer = GetComponent<Renderer>();
        if (npcRenderer != null)
            originalColor = npcRenderer.material.color;

        originalScale = transform.localScale;

        // Auto-tag
        if (!gameObject.CompareTag("VictimNPC"))
            gameObject.tag = "VictimNPC";

        // Get animation controller
        animController = GetComponent<NPCAnimationController>();
        if (animController == null)
            animController = gameObject.AddComponent<NPCAnimationController>();
    }

    public void OnHit()
    {
        Debug.Log("VictimNPC hit! (Innocent - BAD!)");
        StopAllCoroutines();
        StartCoroutine(HitFeedbackCoroutine());

        // Play hit reaction animation
        if (animController != null)
        {
            // Assume hit from forward direction
            animController.PlayHitReaction(transform.forward);
        }

        // Trigger hit feedback
        if (HitFeedbackManager.Instance != null)
        {
            HitFeedbackManager.Instance.PlayVictimHitFeedback(transform.position);
        }
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
