using UnityEngine;

/// <summary>
/// Naughty NPC - the target. Hitting them is good!
/// On hit: flashes green, scales up, plays hit reaction animation.
/// Can dodge projectiles when NPCDodgeBehavior is enabled.
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
    private NPCAnimationController animController;
    private NPCDodgeBehavior dodgeBehavior;

    private void Awake()
    {
        npcRenderer = GetComponent<Renderer>();
        if (npcRenderer != null)
            originalColor = npcRenderer.material.color;

        originalScale = transform.localScale;

        // Auto-tag
        if (!gameObject.CompareTag("NaughtyNPC"))
            gameObject.tag = "NaughtyNPC";

        // Get animation controller
        animController = GetComponent<NPCAnimationController>();
        if (animController == null)
            animController = gameObject.AddComponent<NPCAnimationController>();

        // Get dodge behavior
        dodgeBehavior = GetComponent<NPCDodgeBehavior>();
        if (dodgeBehavior == null)
            dodgeBehavior = gameObject.AddComponent<NPCDodgeBehavior>();
    }

    private void Start()
    {
        // Dodge is disabled by default (unlocked at Level 7)
        if (dodgeBehavior != null)
            dodgeBehavior.IsEnabled = false;
    }

    public void OnHit()
    {
        Debug.Log("NaughtyNPC hit! (Guilty - GOOD!)");
        StopAllCoroutines();
        StartCoroutine(HitFeedbackCoroutine());

        // Play hit reaction animation
        if (animController != null)
        {
            // Assume hit from forward direction
            animController.PlayHitReaction(-transform.forward);
        }

        // Trigger hit feedback
        if (HitFeedbackManager.Instance != null)
        {
            HitFeedbackManager.Instance.PlayNaughtyHitFeedback(transform.position);
        }
    }

    /// <summary>
    /// Enable or disable dodge behavior.
    /// </summary>
    public void SetDodgeEnabled(bool enabled)
    {
        if (dodgeBehavior != null)
            dodgeBehavior.IsEnabled = enabled;
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
